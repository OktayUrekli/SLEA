using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAttackManager : MonoBehaviour
{
    public float damage = 1f;

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
