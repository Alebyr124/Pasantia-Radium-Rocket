using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public int vidas = 3;
    public TextMeshProUGUI VidasGameplay;

    public int puntuacion = 0;
    public TextMeshProUGUI PuntuacionGameplay;

    public float speed = 8.0f;

    public float mouseSensitivity = 2.0f;
    public float limitX = 45.0f;
    public Transform cam;

    private float rotationX;
    private Rigidbody rb;

    public Transform spawnPoint;

    private UIManager uiManager;

    public AudioSource PildoraSound;

    private bool canTakeDamage = true;  
    public float damageCooldown = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.position = spawnPoint.position;

        uiManager = UIManager.inst;
    }

    void Update()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, -limitX, limitX);
            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);

            if (puntuacion >= 10)
            {
                uiManager.ShowWinScreen();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                uiManager.ShowPauseScreen();
            }
        }
    }

    void FixedUpdate()
    {
        if (!uiManager.Win && !uiManager.Pause)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector3 input = new Vector3(x, 0, y).normalized;
            Vector3 movement = input * speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + transform.TransformDirection(movement));
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pildora"))
        {
            PildoraSound.Play();
            Destroy(collision.gameObject);
            puntuacion++;
            PuntuacionGameplay.text = "Puntos: " + puntuacion;
        }

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            
            TakeDamage(1);  
        }
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            Debug.Log("perdiste 1 vida");
            vidas -= damage;
            if (VidasGameplay != null)
                VidasGameplay.text = "Vidas: " + vidas;  

            if (vidas <= 0)
            {
                uiManager.ShowLoseScreen();
            }

            canTakeDamage = false;
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}