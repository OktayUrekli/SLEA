using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FrogController : MonoBehaviour, INPCController
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
    [SerializeField] int attackIndex;

    [Header("Close Attack Variables")]
    [SerializeField] float punchDamagePoint; // punch sald�r� ile verilecek hasar miktar�
    [SerializeField] float weaponDamagePoint; // kick sald�r� ile verilecek hasar miktar�
    [SerializeField] float closeAttackRangeRadius; // yak�n sald�r alan� yar��ap�
    bool closeAttackFinished;

    [Header("Ranged Attack Variables")]
    [SerializeField] float rangeAttackRangeRadius; // uzak sald�r� alan� yar��ap�
    [SerializeField] ParticleSystem meteorAttackVfx;
    [SerializeField] ParticleSystem healUpVfx;
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
            PlayerLiveManager liveManager = target.GetComponent<PlayerLiveManager>();

            if (!liveManager.isDead) // e�er player �l� de�ilse bu i�lemler ger�ekle�tirilmeli
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
                    attackIndex = Random.Range(1, 3); // 1 dahil 3 de�il yani ya 1 ya da 2 -- 1 punch -- 2 kick
                    StartCoroutine(CloseAttackToPlayer(target.gameObject, attackIndex));
                }
                else if (looksDirectToThePlayer && (distance > closeAttackRangeRadius && distance <= rangeAttackRangeRadius) && canAttack)
                {
                    Debug.Log("uzak sald�r� alan�na girildi");

                    // e�er uzak sald�r� alan�na girildiyse yak�n sald�r� m� yoksa uzak sald�r m� yap�lacak karakr verecek
                    int attackType = Random.Range(1, 3); // 1-yak�n  2- uzak 
                    if (attackType == 1 && rangedAttackFinished) // yak�n sald�r�
                    {
                        Debug.Log("yak�n sald�r� tercih edildi");
                        closeAttackFinished = false;
                        npcAnimator.SetFloat("Speed", 5); // walk anim �al��mas� i�in gerekli
                        npcNavAgent.SetDestination(target.transform.position); // yak�n sald�r� yapmak i�in uzak oldu�undan dolay� player a yakla��yor
                    }
                    else if (attackType == 2 && closeAttackFinished)//&& transform.position==targetPos) // uzak sald�r�
                    {
                        Debug.Log("uzak sald�r� tercih edildi");
                        rangedAttackFinished = false;
                        attackIndex = Random.Range(3, 5); // 3-heal  4-meteor
                        StartCoroutine(RangedAttackToPlayer(target.gameObject, attackIndex));
                    }
                }
                else // player sadece g�r�� alan� i�indeyse
                {
                    Debug.Log("g�r�� alan�na girildi");
                    closeAttackFinished= true;
                    rangedAttackFinished= true;
                    if (closeAttackFinished && rangedAttackFinished && canAttack)
                    {
                        ApproachThePlayer(target.transform);
                    }
                }
            }

        }

        if (targetsInViewRadius.Length == 0) // e�er g�r�� alan� i�inde player yoksa npc rasgele dolanacak-patrolling
        {
            looksDirectToThePlayer = false; // g�r�� alan� i�inde player olmay�nca ona do�ru bakamaz
            Patroll();
        }
    }

    IEnumerator RangedAttackToPlayer(GameObject player, int attackType)
    {
        npcNavAgent.SetDestination(transform.position);

        canAttack = false;
        npcAnimator.SetFloat("Speed", 0);

        if (attackType == 3) // Heal 
        {
            // animasyon ve bomba atma senkronizasyonu i�in delay koyuldu
            Debug.Log("heal tercih edildi");
            
            yield return new WaitForSeconds(0.75f);
            HealUp();
        }
        else if (attackType == 4) // meteor
        {
            Debug.Log("meteor sald�r� tercih edildi");
            npcAnimator.SetTrigger("MeteorRain");
            MeteorTypeAttack();
        }

        yield return new WaitForSeconds(attackDuration);
        canAttack = true;
        rangedAttackFinished = true;
    }

    void HealUp()
    {
        float currentHP = GetComponent<EnemyNpcLiveManager>().currentLive;
        if (currentHP<400)
        {
            npcAnimator.SetTrigger("HealUp");
            healUpVfx.Play();
            GetComponent<EnemyNpcLiveManager>().currentLive += 50; // can artt�r�l�yor
            GetComponent<EnemyNpcLiveManager>().EnemyLiveStatus(); // can ui g�ncelleniyor
        }
    }

    void MeteorTypeAttack()
    {
        meteorAttackVfx.Play();
    }

    IEnumerator CloseAttackToPlayer(GameObject player, int attackType)
    {
        npcNavAgent.SetDestination(transform.position);
        npcAnimator.SetFloat("Speed", 0);

        canAttack = false;

        if (attackType == 1) // punch
        {
            Debug.Log("punch sald�r� tercih edildi");
            npcAnimator.SetTrigger("Punch");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi i�in bir delay
                    damageable.TakeDamage(punchDamagePoint);
                }
            }
        }
        else if (attackType == 2) // weapon
        {
            Debug.Log("weapon sald�r� tercih edildi");
            npcAnimator.SetTrigger("UseWeapon");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, closeAttackRangeRadius, targetMask);
            foreach (Collider hit in hitColliders)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    yield return new WaitForSeconds(0.4f); // ilk hasar ile animasyon denk gelmesi i�in bir delay
                    damageable.TakeDamage(weaponDamagePoint);
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
        GetComponent<FrogController>().enabled = false;

    }
}
