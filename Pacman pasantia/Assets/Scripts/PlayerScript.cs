using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using TMPro;

public class playerScript : MonoBehaviour
{
    public int puntuacion = 0;
    public TextMeshProUGUI PuntuacionGameplay;

    public float speed = 8.0f;

    public float mouseSensitivity = 2.0f;
    public float limitX = 45.0f;
    public Transform cam;

    private float rotationX;
    private float rotationY;

    private Rigidbody rb;

    public Transform spawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.position = spawnPoint.position;
    }

    void Update()
    {
        if (!UIManager.inst.Win)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");


            transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * speed);

            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, -limitX, limitX);
            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
            rb = GetComponent<Rigidbody>();

            if (puntuacion >= 10)
            {
                UIManager.inst.ShowWinScreen();
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pildora")
        {
            puntuacion++;
            Destroy(collision.gameObject);
            PuntuacionGameplay.text = "Puntos: " + puntuacion;
        }
    }
}



