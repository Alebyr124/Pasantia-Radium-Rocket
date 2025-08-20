using UnityEngine;

public class Particles : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float cooldown = 0.5f;
    private float lastTime = 0f;

    // Distancia delante del jugador  
    public float offsetDistance = 1f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && Time.time - lastTime > cooldown)
        {
            lastTime = Time.time;

            // Posición al frente del jugador
            Vector3 spawnPos = transform.position + transform.forward * offsetDistance;

            // Rotar el efecto hacia adelante
            Quaternion spawnRot = Quaternion.LookRotation(transform.forward);

            GameObject dust = Instantiate(dustEffectPrefab, spawnPos, spawnRot);
            Destroy(dust, 2f);
        }
    }
}

