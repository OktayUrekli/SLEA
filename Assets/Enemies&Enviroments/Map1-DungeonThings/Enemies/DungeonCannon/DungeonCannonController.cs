using System.Collections;
using UnityEngine;

public class DungeonCannonController : MonoBehaviour,INPCController
{
    [Header("Bomb Variables")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform firePoint; // Cannon ucundaki nokta
    [SerializeField] float bombTravelTime; // Bomban�n hedefe ula�ma s�resi
    [SerializeField] float fireDuration;
    bool canFire;

    [Header("View&Attack Variables")]
    [SerializeField] float turnSpeed; // npc nin playe a do�ru d�n�� h�z�
    [SerializeField] float viewRangeRadius; // g�r�� alan� yar��ap�
    [SerializeField] float attackRangeRadius; // atak alan� yar��ap�
    [SerializeField] LayerMask targetMask;

    private void Start()
    {
        canFire = true;
    }

    private void Update()
    {
        DetectTargets(); // Player Aran�yor
    }

    void DetectTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRangeRadius, targetMask);

        foreach (Collider target in targetsInViewRadius) // e�er player bulunduysa bu blo�un i�ine girer
        {
            PlayerLiveManager liveManager = target.GetComponent<PlayerLiveManager>();

            if (!liveManager.isDead)
            {
                Vector3 direction = (target.transform.position - transform.position);
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                }

                float distance = direction.magnitude;

                if (attackRangeRadius>=distance && canFire) // player atak alan� i�erisine girerse npc sald�racak
                {
                    StartCoroutine(ShootAtTarget(target.transform.position));
                }
                else // e�er player sadece g�r�� alan� i�indeyse npc player a do�ru hareket edecek
                {
                   // Debug.Log("g�r�� alan� alan� i�inde");
                   // cannon hareket etmeyecek
                }
            }

        }

        if (targetsInViewRadius==null) // e�er g�r�� alan� i�inde player yoksa npc rasgele dolanacak-patrolling
        {
            // patrolling kodlar�
            // cannon hareket etmeyecek
        }
    }


    IEnumerator ShootAtTarget(Vector3 targetPosition)
    {
        canFire = false;
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
        CannonBombMovement movement = bomb.GetComponent<CannonBombMovement>();
        movement.Initialize(firePoint.position, GetControlPoint(firePoint.position, targetPosition), targetPosition, bombTravelTime);
        yield return new WaitForSeconds(fireDuration);
        canFire = true;
    }

    private Vector3 GetControlPoint(Vector3 start, Vector3 end) // f�rlat�lacak bomban�n bezier e�risinin orta noktas�n� bulur
    {
        // Start ile End'in ortas�n� al�p biraz yukar� kald�r�yoruz
        Vector3 mid = (start + end) * 0.5f;
        mid += Vector3.up * 5f; // Y�kseklik de�erini iste�ine g�re ayarlayabilirsin
        return mid;
    }

    public void OnDeath()
    {
        Debug.Log("npc enemy " + gameObject.name + " �ld�");
        GetComponent<DungeonCannonController>().enabled = false;
    }
}
