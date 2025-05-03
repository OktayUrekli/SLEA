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
    [SerializeField] float turnSpeed; // npc nin playe a do�ru d�n�� h�z�
    [SerializeField] float viewRangeRadius; // g�r�� alan� yar��ap�
    [SerializeField] LayerMask targetMask;

    [Header("GEneral Attack Variables")]
    bool canAttack;
    [SerializeField] float attackDuration;
    public int attackIndex;

    [Header("Close Attack Variables")]
    [SerializeField] float punchDamagePoint; // punch sald�r� ile verilecek hasar miktar�
    [SerializeField] float kickDamagePoint; // kick sald�r� ile verilecek hasar miktar�
    [SerializeField] float closeAttackRangeRadius; // uzak sald�r alan� yar��ap�
    bool closeAttackFinished;

    [Header("Ranged Attack Variables")]
    [SerializeField] float rangeAttackRangeRadius; // yak�n sald�r� alan� yar��ap�
    [SerializeField] ParticleSystem spinAttackVfx;
    [SerializeField] GameObject giantBombPrefab;
    [SerializeField] Transform giantBombThrowPoint;
    [SerializeField] float bombTravelTime;
    bool rangedAttackFinished;

    [Header("Patroll&Aproach Variables")]
    [SerializeField] bool looksDirectToThePlayer;
    [SerializeField] float randPointRange; //pattrolling s�ras�nda rasgele nokta i�in bir alan yar��ap� 
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

        foreach (Collider target in targetsInViewRadius) // e�er player bulunduysa bu blo�un i�ine girer
        {
            Vector3 direction = (target.transform.position - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f) // player a do�ru d�n�� yap�l�yor
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                looksDirectToThePlayer = true;
            }
            else //npc playera direkt olarak bakm�yorsa attack ve approach yap�lamaz
            {
                looksDirectToThePlayer = false;
            }

            float distance = direction.magnitude; // npc ile player aras�ndaki mesafenin boyutu (xz d�zlemindeki mesafe)

            
            if (looksDirectToThePlayer && distance <= closeAttackRangeRadius && canAttack) // e�er yak�n sald�r� alan�na girildiyse
            {
                Debug.Log("yak�n sald�r� alan�na girildi");
                // kick ile tekme mi atacak yoksa punch ile yumruk mu atacak tercih yap�lacak
                attackIndex = Random.Range(2, 3); // 1 dahil 3 de�il yani ya 1 ya da 2 -- 1 punch -- 2 kick
                StartCoroutine(CloseAttackToPlayer(target.gameObject, attackIndex));
            }
            else if (looksDirectToThePlayer && (distance > closeAttackRangeRadius && distance <= rangeAttackRangeRadius) && canAttack) 
            {
                Debug.Log("uzak sald�r� alan�na girildi");

                // e�er uzak sald�r� alan�na girildiyse yak�n sald�r� m� yoksa uzak sald�r m� yap�lacak karakr verecek
                int attackType = Random.Range(1, 2); // 1-yak�n  2- uzak 
                if (attackType==1 && rangedAttackFinished ) // yak�n sald�r�
                {
                    Debug.Log("yak�n sald�r� tercih edildi");
                    closeAttackFinished = false;
                    npcAnimator.SetFloat("Speed", 5); // walk anim �al��mas� i�in gerekli
                    npcNavAgent.SetDestination(target.transform.position); // yak�n sald�r� yapmak i�in uzak oldu�undan dolay� player a yakla��yor
                }
                else if(attackType==2 && closeAttackFinished )//&& transform.position==targetPos) // uzak sald�r�
                {
                    Debug.Log("uzak sald�r� tercih edildi");
                    rangedAttackFinished = false;
                    attackIndex = Random.Range(3, 5); // 3-throw  4-spin
                    StartCoroutine(RangedAttackToPlayer(target.gameObject, attackIndex));
                }
            }
            else // player sadece g�r�� alan� i�indeyse
            {
                Debug.Log("g�r�� alan�na girildi");
                if (closeAttackFinished && rangedAttackFinished)
                {
                    ApproachThePlayer(target.transform);
                }
            }
        }

        if (targetsInViewRadius.Length == 0) // e�er g�r�� alan� i�inde player yoksa npc rasgele dolanacak-patrolling
        {
            looksDirectToThePlayer = false; // g�r�� alan� i�inde player olmay�nca ona do�ru bakamaz
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
             // animasyon ve bomba atma senkronizasyonu i�in delay koyuldu
            Debug.Log("trow sald�r� tercih edildi");
            npcAnimator.SetTrigger("Throw");
            yield return new WaitForSeconds(0.75f);
            ThrowTypeAttack(player);            
        }
        else if (attackType == 4) // Spin
        {
            Debug.Log("spin sald�r� tercih edildi");
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
    private Vector3 GetControlPoint(Vector3 start, Vector3 end) // f�rlat�lacak bomban�n bezier e�risinin orta noktas�n� bulur
    {
        // Start ile End'in ortas�n� al�p biraz yukar� kald�r�yoruz
        Vector3 mid = (start + end) * 0.5f;
        mid += Vector3.up * 5f; // Y�kseklik de�erini iste�ine g�re ayarlayabilirsin
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
            Debug.Log("punch sald�r� tercih edildi");
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    npcAnimator.SetTrigger("Punch");
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi i�in bir delay
                    damageable.TakeDamage(punchDamagePoint);
                }
            }
        }
        else if (attackType == 2) // kick
        {
            Debug.Log("kick sald�r� tercih edildi");
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    npcAnimator.SetTrigger("Kick");
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi i�in bir delay
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
        if (npcNavAgent.remainingDistance <= npcNavAgent.stoppingDistance) //e�er hedef noktaya ula�m�� ise
        {

            Vector3 point;
            if (RandomPoint(transform.position, randPointRange, out point))
            {
                npcAnimator.SetFloat("Speed", 5); // walk/run animasyonu oynat�ls�n diye 0 dan b�y�k bir de�er verilmeli
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //gidilecek noktan�n gizmosu �iziliyor
                npcNavAgent.SetDestination(point);

                /*
                Debug.Log("hedefe ula��ld� beklemeye ge�ildi");
                npcAnimator.SetFloat("Speed", 0);
                waitBetweenPatroll = Random.Range(3, 4.5f); // hedefe ula�t�ktan sonra random bir s�re bekleme ger�ekle�iyor
                yield return new WaitForSeconds(waitBetweenPatroll);
                */
            }

        }
    }

    // rasgele nokta se�en fonksiyon. nokta gidilebilen alanda ise geriye true d�ner
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //merkezden range kadar mesafedeki �ember alan�ndan rasgele nokta se�er
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
        Debug.Log("npc enemy " + gameObject.name + " �ld�");
        GetComponent<GiantSkeletonBossController>().enabled = false;

    }
}
