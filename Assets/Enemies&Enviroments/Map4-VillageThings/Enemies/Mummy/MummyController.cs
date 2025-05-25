using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MummyController : MonoBehaviour,INPCController
{
    NavMeshAgent npcNavAgent;


    [Header("View Range Variables")]
    [SerializeField] float turnSpeed; // npc nin playe a doðru dönüþ hýzý
    [SerializeField] float viewRangeRadius; // görüþ alaný yarýçapý
    [SerializeField] LayerMask targetMask;

    [Header("Attack Variables")]
    bool canAttack;
    [SerializeField] float damagePoint; // saldýrý ile verilecek hasr miktarý
    [SerializeField] float attackRangeRadius; // atak alaný yarýçapý
    [SerializeField] GameObject mummyArmature;
    [SerializeField] ParticleSystem suicadeAttackVFX;

    [Header("Patroll&Aproach Variables")]
    [SerializeField] bool looksDirectToThePlayer;
    [SerializeField] float randPointRange; //pattrolling sýrasýnda rasgele nokta için bir alan yarýçapý 

    //public float waitBetweenPatroll;

    void Start()
    {
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

        foreach (Collider target in targetsInViewRadius) // eðer player bulunduysa bu bloðun içine girer
        {
            PlayerLiveManager liveManager = target.GetComponent<PlayerLiveManager>();

            if (!liveManager.isDead)
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

                float distance = direction.magnitude; // npc ile player arasýndaki mesafenin skaler büyüklüðü (xz düzlemindeki mesafe)

                if (attackRangeRadius >= distance && canAttack) // player atak alaný içerisine girerse npc saldýracak
                {
                    AttackToPlayer(target.gameObject);
                }
                else // eðer player sadece görüþ alaný içindeyse npc player a doðru hareket edecek
                {
                    if (looksDirectToThePlayer && distance > attackRangeRadius && canAttack)
                    {
                        ApproachThePlayer(target.transform);
                    }
                }
            }

        }

        if (targetsInViewRadius.Length == 0) // eðer görüþ alaný içinde player yoksa npc rasgele dolanacak-patrolling
        {
            looksDirectToThePlayer = false; // görüþ alaný içinde player olmayýnca ona doðru bakamaz
            Patroll();
        }
    }

    void AttackToPlayer(GameObject player)
    {
        // player atak alanýna girince npc nin durmasý için pleyerýn atak alanýna ilk girdiði andaki konumu hedef konumu olacak
        npcNavAgent.SetDestination(transform.position);

        canAttack = false;
       // npcAnimator.SetFloat("Speed", 0);
        GetComponent<Animator>().enabled = false; // intihar saldýrýsý sonrasý baþka nimasyon yapamaz
        suicadeAttackVFX.Play();
        mummyArmature.SetActive(false);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRangeRadius, targetMask);
        foreach (Collider hit in hitColliders)
        {
            // Hasar vereceðimiz component'ý bulmaya çalýþýyoruz
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                //yield return new WaitForSeconds(0.5f); // saldýrý animasyonunun tam ýsýrma anýna denk getirmek için küçük bir delay koyuldu
                damageable.TakeDamage(damagePoint);
            }
        }
        
        Destroy(gameObject, 1f);
        //yield return new WaitForSeconds(attackDuration);
        //canAttack = true;

    }

    void ApproachThePlayer(Transform playerPos)
    {
        //npcAnimator.SetFloat("Speed", 5);
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
                //npcAnimator.SetFloat("Speed", 5); // walk/run animasyonu oynatýlsýn diye 0 dan büyük bir deðer verilmeli
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //gidilecek noktanýn gizmosu çiziliyor
                npcNavAgent.SetDestination(point);

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
        GetComponent<MummyController>().enabled = false;
    }
}
