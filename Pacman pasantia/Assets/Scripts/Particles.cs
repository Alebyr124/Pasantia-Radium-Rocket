using UnityEngine;

public class Particles : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float cooldown = 0.5f;
    private float lastTime = 0f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && Time.time - lastTime > cooldown)
        {
            lastTime = Time.time;

            GameObject dust = Instantiate(
                dustEffectPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );
            Destroy(dust, 2f);
        }
    }
}
