using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private UIManager uiManager;

    [Header("Audio")]
    public AudioSource PildoraSound;
    public AudioSource DamageSound;

    [Header("Damage")]
    private bool canTakeDamage = true;
    public float damageCooldown = 2f;

    [Header("Mobile Controls")]
    public Joystick joystick; // Arrastrar joystick desde Canvas (solo para móvil)
    private bool isMobile;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Detectar plataforma
        isMobile = (Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer);

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

        // Posición inicial
        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        // UI Manager
        uiManager = UIManager.inst;

        // UI inicial
        if (VidasGameplay != null)
            VidasGameplay.text = "Vidas: " + vidas;
        if (PuntuacionGameplay != null)
            PuntuacionGameplay.text = "Puntos: " + puntuacion;

        // Activar/desactivar joystick según plataforma
        if (joystick != null)
        {
            joystick.gameObject.SetActive(isMobile);
        }
    }

    void Update()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            // Control de cámara según plataforma
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
                // Móvil: Touch para mirar (pantalla táctil)
                HandleTouchCamera();
            }

            // Win condition
            if (puntuacion >= 10)
            {
                uiManager.ShowWinScreen();
                if (!isMobile)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            // Pause (solo en PC)
            if (Input.GetKeyDown(KeyCode.Escape) && !isMobile)
            {
                uiManager.ShowPauseScreen();
            }
        }
    }

    void FixedUpdate()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            float x, y;

            // Obtener input según plataforma
            if (isMobile && joystick != null)
            {
                // Móvil: Usar joystick
                x = joystick.Horizontal;
                y = joystick.Vertical;
            }
            else
            {
                // PC: Usar teclado (WASD o flechas)
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }

            // Aplicar movimiento
            Vector3 input = new Vector3(x, 0, y).normalized;
            Vector3 movement = input * speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + transform.TransformDirection(movement));
        }
    }

    void HandleTouchCamera()
    {
        // Control de cámara con touch para móvil
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Solo mover cámara si el touch está fuera del área del joystick
            if (joystick != null)
            {
                RectTransform joystickRect = joystick.GetComponent<RectTransform>();
                if (joystickRect != null && !RectTransformUtility.RectangleContainsScreenPoint(joystickRect, touch.position))
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        float touchSensitivity = mouseSensitivity * 0.5f; // Ajustar sensibilidad para touch

                        rotationX -= touch.deltaPosition.y * touchSensitivity * 0.1f;
                        rotationX = Mathf.Clamp(rotationX, -limitX, limitX);
                        cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
                        transform.rotation *= Quaternion.Euler(0, touch.deltaPosition.x * touchSensitivity * 0.1f, 0);
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
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

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}