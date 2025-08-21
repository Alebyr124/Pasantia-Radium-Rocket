using UnityEngine;

public class Particles : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float cooldown = 0.5f;
    private float lastTime = 0f;

     
    public float offsetDistance = 1f;

    public AudioSource WallSound;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && Time.time - lastTime > cooldown)
        {
            lastTime = Time.time;

            WallSound.Play();

            Vector3 spawnPos = transform.position + transform.forward * offsetDistance;

            
            Quaternion spawnRot = Quaternion.LookRotation(transform.forward);

            GameObject dust = Instantiate(dustEffectPrefab, spawnPos, spawnRot);
            Destroy(dust, 2f);
        }
    }
}

