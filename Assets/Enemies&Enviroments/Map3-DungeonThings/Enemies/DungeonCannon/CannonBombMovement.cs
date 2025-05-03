using UnityEngine;

public class CannonBombMovement : MonoBehaviour
{
    private Vector3 startPoint, controlPoint, endPoint;
    private float duration;
    private float elapsedTime = 0f;

    [SerializeField] ParticleSystem explodeVFX;

    [SerializeField] float explosionRadius ;
    [SerializeField] int explosionDamage ;
    [SerializeField] LayerMask damageableLayers; // Hangi layer'dakilere hasar verecek

    private bool exploded = false;

    public void Initialize(Vector3 start, Vector3 control, Vector3 end, float time)
    {
        startPoint = start;
        controlPoint = control;
        endPoint = end;
        duration = time;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        Vector3 position = Mathf.Pow(1 - t, 2) * startPoint +
                           2 * (1 - t) * t * controlPoint +
                           Mathf.Pow(t, 2) * endPoint;

        transform.position = position;

        if (t >= 1f && !exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        PlayVFX(explodeVFX);

        // Patlama alanýnda kimler var bulalým
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);

        foreach (Collider hit in hitColliders)
        {
            // Hasar vereceðimiz component'ý bulmaya çalýþýyoruz
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }
        }
        
        Destroy(gameObject,0.2f); // Bombayý yok ediyoruz
    }

    void PlayVFX(ParticleSystem vfx) // istenilen vfx i oynatýr
    {
        vfx.Play();
        
    }

    void OnDrawGizmosSelected()
    {
        // Editörde patlama alanýný görmek için
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

