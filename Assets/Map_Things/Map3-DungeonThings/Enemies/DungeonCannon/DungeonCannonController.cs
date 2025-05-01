using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonCannonController : MonoBehaviour
{
    [Header("Bomb Variables")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform firePoint; // Cannon ucundaki nokta
    [SerializeField] float bombTravelTime = 2f; // Bombanýn hedefe ulaþma süresi
    [SerializeField] float fireDuration = 3f;
    bool canFire;

    [Header("View&Attack Variables")]
    [SerializeField] float turnSpeed = 150f; // npc nin playe a doðru dönüþ hýzý
    [SerializeField] float viewRangeRadius = 15f; // görüþ alaný yarýçapý
    [SerializeField] float attackRangeRadius = 15f; // atak alaný yarýçapý
    [SerializeField] LayerMask targetMask;

    private void Start()
    {
        canFire = true;
    }

    private void Update()
    {
        DetectTargets(); // Player Aranýyor
    }

    void DetectTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRangeRadius, targetMask);

        foreach (Collider target in targetsInViewRadius) // eðer player bulunduysa bu bloðun içine girer
        {
            Vector3 direction = (target.transform.position - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            float distance = direction.magnitude;

            if (attackRangeRadius>=distance && canFire) // player atak alaný içerisine girerse npc saldýracak
            {
                StartCoroutine(ShootAtTarget(target.transform.position));
            }
            else // eðer player sadece görüþ alaný içindeyse npc player a doðru hareket edecek
            {
                Debug.Log("görüþ alaný alaný içinde");
            }
        }

        if (targetsInViewRadius==null) // eðer görüþ alaný içinde player yoksa npc rasgele dolanacak-patrolling
        {
            // patrolling kodlarý
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

    private Vector3 GetControlPoint(Vector3 start, Vector3 end) // fýrlatýlacak bombanýn bezier eðrisinin orta noktasýný bulur
    {
        // Start ile End'in ortasýný alýp biraz yukarý kaldýrýyoruz
        Vector3 mid = (start + end) * 0.5f;
        mid += Vector3.up * 5f; // Yükseklik deðerini isteðine göre ayarlayabilirsin
        return mid;
    }
}
