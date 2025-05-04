using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlobController : MonoBehaviour,INPCController
{
    Animator npcAnimator;
    NavMeshAgent npcNavAgent;



    [Header("View Range Variables")]
    [SerializeField] float turnSpeed = 125; // npc nin playe a do�ru d�n�� h�z�
    [SerializeField] float viewRangeRadius = 10; // g�r�� alan� yar��ap�
    [SerializeField] LayerMask targetMask;

    [Header("Attack Variables")]
    bool canAttack;
    [SerializeField] float damagePoint; // sald�r� ile verilecek hasr miktar�
    [SerializeField] float attackRangeRadius = 2; // atak alan� yar��ap�
    [SerializeField] float attackDuration;

    [Header("Patroll&Aproach Variables")]
    [SerializeField] bool looksDirectToThePlayer;
    [SerializeField] float randPointRange; //pattrolling s�ras�nda rasgele nokta i�in bir alan yar��ap� 

    //public float waitBetweenPatroll;

    void Start()
    {
        npcAnimator = GetComponent<Animator>();
        npcNavAgent = GetComponent<NavMeshAgent>();
        canAttack = true;

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

            if (!liveManager.isDead)
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

                if (attackRangeRadius >= distance && canAttack) // player atak alan� i�erisine girerse npc sald�racak
                {
                    StartCoroutine(AttackToPlayer(target.gameObject));
                }
                else // e�er player sadece g�r�� alan� i�indeyse npc player a do�ru hareket edecek
                { 
                    if (looksDirectToThePlayer && distance > attackRangeRadius && canAttack)
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

    IEnumerator AttackToPlayer(GameObject player)
    {
        npcNavAgent.SetDestination(transform.position );

        canAttack = false;
        npcAnimator.SetFloat("Speed", 0);
        npcAnimator.SetTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRangeRadius, targetMask);
        foreach (Collider hit in hitColliders)
        {
            // Hasar verece�imiz component'� bulmaya �al���yoruz
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                yield return new WaitForSeconds(0.3f); // sald�r� animasyonunun tam �s�rma an�na denk getirmek i�in k���k bir delay koyuldu
                damageable.TakeDamage(damagePoint);
            }
        }

        yield return new WaitForSeconds(attackDuration);
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
        if (gameObject.name=="BigBlob")
        {
            gameObject.GetComponent<BigBlobAfterDead>().CreateMidBlob();
        }
        GetComponent<BlobController>().enabled = false;

    }
}
