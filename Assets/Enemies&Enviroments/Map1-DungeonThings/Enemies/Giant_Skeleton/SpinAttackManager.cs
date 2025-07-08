using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackManager : MonoBehaviour
{
    public float damage =5f;

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
