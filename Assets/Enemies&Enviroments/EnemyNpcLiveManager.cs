using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public interface INPCController
{
    void OnDeath();
}

public class EnemyNpcLiveManager : MonoBehaviour,IDamageable
{
    Animator npcAnimator;

    [Header("VFXs")]
    [SerializeField] ParticleSystem npcDeadVFX;
    [SerializeField] ParticleSystem npcTakeHitVFX;

    [Header("Live Variables")]
    [SerializeField] float maxLive;
    [SerializeField] Image currentLiveImg;
    [SerializeField] GameObject floatingText;
    public float currentLive;



    void Start()
    {       
        npcAnimator = GetComponent<Animator>();
        currentLive=maxLive;
        EnemyLiveStatus();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Sword"))
        {
            TakeDamage(50);

        }
        if (other.CompareTag("Arrow"))
        {
            TakeDamage(30);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentLive -= damage;

        GameObject floatingTextObjReferance = Instantiate(floatingText, transform.position, Quaternion.identity);
        floatingTextObjReferance.GetComponentInChildren<TextMesh>().text = damage.ToString();
        Destroy(floatingTextObjReferance, 1f);

        if (currentLive<=0)
        {
            EnemyNPCDead();
        }
        else
        {
            PlayVFX(npcTakeHitVFX);
            EnemyLiveStatus();
        }
    }

    void EnemyNPCDead()
    {
        GetComponent<Collider>().enabled = true;
        if (npcAnimator!=null)
        {
            npcAnimator.SetTrigger("Dead");
        }
        GetComponent<Collider>().enabled = false;
        PlayVFX(npcDeadVFX);
        EnemyLiveStatus();
        gameObject.GetComponent<INPCController>().OnDeath(); // npc ölünce mekaniðini konrol eden script devredýþý býrakýlýyor
        Destroy(gameObject, 1f);
    }

    public void EnemyLiveStatus() // npc nin kalan canýný ui da günceller
    {
        currentLiveImg.fillAmount = currentLive / maxLive;
    }

    void PlayVFX(ParticleSystem vfx) // istenilen vfx i oynatýr
    {
        vfx.Play();
    }


}
