using UnityEngine;

public class Particles : MonoBehaviour
{
    public GameObject dustEffectPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Entró en el trigger de la pared: " + other.name);
            Instantiate(
                dustEffectPrefab,
                other.ClosestPoint(transform.position),
                Quaternion.identity
            );
        }
    }
}
