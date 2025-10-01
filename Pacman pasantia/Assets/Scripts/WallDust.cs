using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDust : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float cooldown = 0.5f;
    private float lastTime = -1f;

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

            // Destruye el objeto instanciado después de 0.5s
            Destroy(dust, 0.5f);
        }
    }
}

