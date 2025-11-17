using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    [Header("Stats")]
    public int vidas = 3;
    public TextMeshProUGUI VidasGameplay;
    public int puntuacion = 0;
    public TextMeshProUGUI PuntuacionGameplay;

    [Header("Movement")]
    public float speed = 8.0f;
    public float mouseSensitivity = 2.0f;
    public float limitX = 45.0f;
    public Transform cam;
    private float rotationX;
    private Rigidbody rb;
    public Transform spawnPoint;
    public UIManager uiManager;

    [Header("Audio")]
    public AudioSource PildoraSound;
    public AudioSource DamageSound;

    [Header("Damage")]
    private bool canTakeDamage = true;
    public float damageCooldown = 2f;

    [Header("Mobile Controls")]
    public FixedJoystick joystick;
    public bool isMobile;
    private int cameraFingerId = -1;

   public void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Detectar plataforma correctamente para WebGL
        isMobile = DetectMobilePlatform();

        // Configurar cursor según plataforma
        if (!isMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        uiManager = UIManager.inst;

        if (VidasGameplay != null)
            VidasGameplay.text = "Vidas: " + vidas;
        if (PuntuacionGameplay != null)
            PuntuacionGameplay.text = "Puntos: " + puntuacion;

        if (joystick != null)
        {
            joystick.gameObject.SetActive(isMobile);
        }

        Debug.Log("Plataforma detectada como móvil: " + isMobile);
    }

    public bool DetectMobilePlatform()
    {
        // Para builds nativos de Android/iOS
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }

        // Para WebGL, verificar si es móvil
#if UNITY_WEBGL && !UNITY_EDITOR
            return Application.isMobilePlatform;
#elif UNITY_EDITOR
        // En el editor, usar touchSupported para testing
        return Input.touchSupported;
#else
            return false;
#endif
    }

    public void Update()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            if (!isMobile)
            {
                // PC: Mouse para mirar
                rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                rotationX = Mathf.Clamp(rotationX, -limitX, limitX);
                cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
            }
            else
            {
                // Móvil: Touch para mirar
                HandleTouchCamera();
            }

            if (puntuacion >= 10)
            {
                uiManager.ShowWinScreen();
                if (!isMobile)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !isMobile)
            {
                uiManager.ShowPauseScreen();
            }
        }
    }

    public void FixedUpdate()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            float x, y;

            if (isMobile && joystick != null)
            {
                x = joystick.Horizontal;
                y = joystick.Vertical;
            }
            else
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }

            Vector3 input = new Vector3(x, 0, y).normalized;
            Vector3 movement = input * speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + transform.TransformDirection(movement));
        }
    }

    public void HandleTouchCamera()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began)
            {
                if (!IsPointerOverUIObject(touch.fingerId))
                {
                    cameraFingerId = touch.fingerId;
                }
            }
            else if (touch.fingerId == cameraFingerId && touch.phase == TouchPhase.Moved)
            {
                float touchSensitivity = mouseSensitivity * 1f;

                rotationX -= touch.deltaPosition.y * touchSensitivity * 0.1f;
                rotationX = Mathf.Clamp(rotationX, -limitX, limitX);
                cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, touch.deltaPosition.x * touchSensitivity * 0.1f, 0);
            }
            else if (touch.fingerId == cameraFingerId &&
                    (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                cameraFingerId = -1;
            }
        }
    }

    public bool IsPointerOverUIObject(int fingerId)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.GetTouch(fingerId).position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pildora"))
        {
            if (PildoraSound != null)
                PildoraSound.Play();

            Destroy(collision.gameObject);
            puntuacion++;

            if (PuntuacionGameplay != null)
                PuntuacionGameplay.text = "Puntos: " + puntuacion;
        }

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage && vidas > 0)
        {
            vidas -= damage;

            if (DamageSound != null)
                DamageSound.Play();

            if (VidasGameplay != null)
                VidasGameplay.text = "Vidas: " + vidas;

            if (vidas <= 0)
            {
                uiManager.ShowLoseScreen();
            }
            else
            {
                canTakeDamage = false;
                StartCoroutine(DamageCooldown());
            }
        }
    }

    public IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}