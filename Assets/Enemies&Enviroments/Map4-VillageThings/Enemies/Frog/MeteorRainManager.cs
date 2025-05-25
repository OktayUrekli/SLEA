using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorRainManager : MonoBehaviour
{
    public float damage = 7f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable health = other.GetComponent<IDamageable>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
