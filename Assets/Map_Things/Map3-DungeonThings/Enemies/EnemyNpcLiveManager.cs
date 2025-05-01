using UnityEngine;
using UnityEngine.UI;

public class EnemyNpcLiveManager : MonoBehaviour,IDamageable
{
    [Header("VFXs")]
    [SerializeField] ParticleSystem npcDeadVFX;
    [SerializeField] ParticleSystem npcTakeHitVFX;

    [Header("Live Variables")]
    [SerializeField] float maxLive;
    [SerializeField] Image currentLiveImg;
    [SerializeField] GameObject floatingText;
    float currentLive;

    void Start()
    {
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
            PlayVFX(npcDeadVFX);

            EnemyLiveStatus();
            Destroy(gameObject,0.75f);
        }
        else
        {
            PlayVFX(npcTakeHitVFX);
            EnemyLiveStatus();
        }
    }

    void EnemyLiveStatus() // npc nin kalan canýný ui da günceller
    {
        currentLiveImg.fillAmount = currentLive / maxLive;
    }

    void PlayVFX(ParticleSystem vfx) // istenilen vfx i oynatýr
    {
        vfx.Play();
    }


}
