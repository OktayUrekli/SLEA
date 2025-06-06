using UnityEngine;

public class EnemyNpcProjectile : MonoBehaviour
{
    [SerializeField] float damagePoint; // saldýrý ile verilecek hasr miktarý
    [SerializeField] float firePower;

    float destroyItselfTime=1f;
    float elapsedTime=0;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * firePower * Time.deltaTime, ForceMode.Impulse);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime>=destroyItselfTime)
        {
            Destroy(gameObject); // zamaný dolarsa kendini yok eder
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damagePoint);
                Destroy(gameObject);
            }
        }
        else // herhangi bir yere çarparsa yok olur
        {
            Destroy(gameObject);
        }
    }
}
