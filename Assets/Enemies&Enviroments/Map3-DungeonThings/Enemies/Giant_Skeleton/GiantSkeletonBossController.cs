using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GiantSkeletonBossController : MonoBehaviour,INPCController
{
    Animator npcAnimator;
    NavMeshAgent npcNavAgent;

    [Header("View Range Variables")]
    [SerializeField] float turnSpeed; // npc nin playe a doðru dönüþ hýzý
    [SerializeField] float viewRangeRadius; // görüþ alaný yarýçapý
    [SerializeField] LayerMask targetMask;

    [Header("GEneral Attack Variables")]
    bool canAttack;
    [SerializeField] float attackDuration;
    public int attackIndex;

    [Header("Close Attack Variables")]
    [SerializeField] float punchDamagePoint; // punch saldýrý ile verilecek hasar miktarý
    [SerializeField] float kickDamagePoint; // kick saldýrý ile verilecek hasar miktarý
    [SerializeField] float closeAttackRangeRadius; // uzak saldýr alaný yarýçapý
    bool closeAttackFinished;

    [Header("Ranged Attack Variables")]
    [SerializeField] float rangeAttackRangeRadius; // yakýn saldýrý alaný yarýçapý
    [SerializeField] ParticleSystem spinAttackVfx;
    [SerializeField] GameObject giantBombPrefab;
    [SerializeField] Transform giantBombThrowPoint;
    [SerializeField] float bombTravelTime;
    bool rangedAttackFinished;

    [Header("Patroll&Aproach Variables")]
    [SerializeField] bool looksDirectToThePlayer;
    [SerializeField] float randPointRange; //pattrolling sýrasýnda rasgele nokta için bir alan yarýçapý 
    //public float waitBetweenPatroll;

    void Start()
    {
        npcAnimator = GetComponent<Animator>();
        npcNavAgent = GetComponent<NavMeshAgent>();
        canAttack = true;
        closeAttackFinished = true;
        rangedAttackFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        DetectTargets();
    }

    void DetectTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRangeRadius, targetMask);

        foreach (Collider target in targetsInViewRadius) // eðer player bulunduysa bu bloðun içine girer
        {
            Vector3 direction = (target.transform.position - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f) // player a doðru dönüþ yapýlýyor
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                looksDirectToThePlayer = true;
            }
            else //npc playera direkt olarak bakmýyorsa attack ve approach yapýlamaz
            {
                looksDirectToThePlayer = false;
            }

            float distance = direction.magnitude; // npc ile player arasýndaki mesafenin boyutu (xz düzlemindeki mesafe)

            
            if (looksDirectToThePlayer && distance <= closeAttackRangeRadius && canAttack) // eðer yakýn saldýrý alanýna girildiyse
            {
                Debug.Log("yakýn saldýrý alanýna girildi");
                // kick ile tekme mi atacak yoksa punch ile yumruk mu atacak tercih yapýlacak
                attackIndex = Random.Range(2, 3); // 1 dahil 3 deðil yani ya 1 ya da 2 -- 1 punch -- 2 kick
                StartCoroutine(CloseAttackToPlayer(target.gameObject, attackIndex));
            }
            else if (looksDirectToThePlayer && (distance > closeAttackRangeRadius && distance <= rangeAttackRangeRadius) && canAttack) 
            {
                Debug.Log("uzak saldýrý alanýna girildi");

                // eðer uzak saldýrý alanýna girildiyse yakýn saldýrý mý yoksa uzak saldýr mý yapýlacak karakr verecek
                int attackType = Random.Range(1, 2); // 1-yakýn  2- uzak 
                if (attackType==1 && rangedAttackFinished ) // yakýn saldýrý
                {
                    Debug.Log("yakýn saldýrý tercih edildi");
                    closeAttackFinished = false;
                    npcAnimator.SetFloat("Speed", 5); // walk anim çalýþmasý için gerekli
                    npcNavAgent.SetDestination(target.transform.position); // yakýn saldýrý yapmak için uzak olduðundan dolayý player a yaklaþýyor
                }
                else if(attackType==2 && closeAttackFinished )//&& transform.position==targetPos) // uzak saldýrý
                {
                    Debug.Log("uzak saldýrý tercih edildi");
                    rangedAttackFinished = false;
                    attackIndex = Random.Range(3, 5); // 3-throw  4-spin
                    StartCoroutine(RangedAttackToPlayer(target.gameObject, attackIndex));
                }
            }
            else // player sadece görüþ alaný içindeyse
            {
                Debug.Log("görüþ alanýna girildi");
                if (closeAttackFinished && rangedAttackFinished)
                {
                    ApproachThePlayer(target.transform);
                }
            }
        }

        if (targetsInViewRadius.Length == 0) // eðer görüþ alaný içinde player yoksa npc rasgele dolanacak-patrolling
        {
            looksDirectToThePlayer = false; // görüþ alaný içinde player olmayýnca ona doðru bakamaz
            Patroll();
        }
    }

    IEnumerator RangedAttackToPlayer(GameObject player,int attackType)
    {
        npcNavAgent.SetDestination(transform.position);

        canAttack = false;
        npcAnimator.SetFloat("Speed", 0);

        if (attackType == 3) // Throw
        {
             // animasyon ve bomba atma senkronizasyonu için delay koyuldu
            Debug.Log("trow saldýrý tercih edildi");
            npcAnimator.SetTrigger("Throw");
            yield return new WaitForSeconds(0.75f);
            ThrowTypeAttack(player);            
        }
        else if (attackType == 4) // Spin
        {
            Debug.Log("spin saldýrý tercih edildi");
            npcAnimator.SetTrigger("Spin");
            SpinTypeAttack();
        }

        yield return new WaitForSeconds(attackDuration);
        canAttack = true;
        rangedAttackFinished = true;
    }


    void ThrowTypeAttack(GameObject targetPlayer)
    {
        GameObject bomb = Instantiate(giantBombPrefab, giantBombThrowPoint.position, Quaternion.identity);
        CannonBombMovement movement = bomb.GetComponent<CannonBombMovement>();
        movement.Initialize(giantBombThrowPoint.position, GetControlPoint(giantBombThrowPoint.position, targetPlayer.transform.position), targetPlayer.transform.position, bombTravelTime);
    }
    private Vector3 GetControlPoint(Vector3 start, Vector3 end) // fýrlatýlacak bombanýn bezier eðrisinin orta noktasýný bulur
    {
        // Start ile End'in ortasýný alýp biraz yukarý kaldýrýyoruz
        Vector3 mid = (start + end) * 0.5f;
        mid += Vector3.up * 5f; // Yükseklik deðerini isteðine göre ayarlayabilirsin
        return mid;
    }

    void SpinTypeAttack()
    {
        spinAttackVfx.Play();
    }

    IEnumerator CloseAttackToPlayer(GameObject player, int attackType)
    {
        npcNavAgent.SetDestination(transform.position);
        npcAnimator.SetFloat("Speed", 0);

        canAttack = false;

        if (attackType == 1) // punch
        {
            Debug.Log("punch saldýrý tercih edildi");
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    npcAnimator.SetTrigger("Punch");
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi için bir delay
                    damageable.TakeDamage(punchDamagePoint);
                }
            }
        }
        else if (attackType == 2) // kick
        {
            Debug.Log("kick saldýrý tercih edildi");
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    npcAnimator.SetTrigger("Kick");
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi için bir delay
                    damageable.TakeDamage(kickDamagePoint);
                }
            }
            
        }
        yield return new WaitForSeconds(attackDuration);
        closeAttackFinished = true;
        canAttack = true;
    }

    void ApproachThePlayer(Transform playerPos)
    {
        npcAnimator.SetFloat("Speed", 5);
        npcNavAgent.SetDestination(playerPos.position);
    }

    #region Parolling 
    void Patroll()
    {
        if (npcNavAgent.remainingDistance <= npcNavAgent.stoppingDistance) //eðer hedef noktaya ulaþmýþ ise
        {

            Vector3 point;
            if (RandomPoint(transform.position, randPointRange, out point))
            {
                npcAnimator.SetFloat("Speed", 5); // walk/run animasyonu oynatýlsýn diye 0 dan büyük bir deðer verilmeli
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //gidilecek noktanýn gizmosu çiziliyor
                npcNavAgent.SetDestination(point);

                /*
                Debug.Log("hedefe ulaþýldý beklemeye geçildi");
                npcAnimator.SetFloat("Speed", 0);
                waitBetweenPatroll = Random.Range(3, 4.5f); // hedefe ulaþtýktan sonra random bir süre bekleme gerçekleþiyor
                yield return new WaitForSeconds(waitBetweenPatroll);
                */
            }

        }
    }

    // rasgele nokta seçen fonksiyon. nokta gidilebilen alanda ise geriye true döner
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //merkezden range kadar mesafedeki çember alanýndan rasgele nokta seçer
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


    #endregion

    public void OnDeath()
    {
        Debug.Log("npc enemy " + gameObject.name + " öldü");
        GetComponent<GiantSkeletonBossController>().enabled = false;

    }
}
