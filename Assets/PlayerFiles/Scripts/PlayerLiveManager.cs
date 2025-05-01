using TMPro;
using UnityEngine;
using UnityEngine.UI;


public interface IDamageable
{
    void TakeDamage(float damage);
}
public class PlayerLiveManager : MonoBehaviour, IDamageable
{
    Animator playerAnimator;

    [SerializeField] float maxLive = 200;
    [SerializeField] float currentLive;

    [SerializeField] TextMeshProUGUI remaingLiveText;
    [SerializeField] Image remaingLiveImg;

    [SerializeField] ParticleSystem takeDamageBloodVfx;
    [SerializeField] ParticleSystem deathVfx;

    [SerializeField] GameObject floatingText;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        remaingLiveText.text = maxLive.ToString();
        currentLive = maxLive;
        remaingLiveImg.fillAmount = currentLive/maxLive;
    }

 
    public void TakeDamage(float damage)
    {
        currentLive -= damage;

        GameObject floatingTextObjReferance=Instantiate(floatingText, transform.position, Quaternion.identity);
        floatingTextObjReferance.GetComponentInChildren<TextMesh>().text = damage.ToString();
        Destroy(floatingTextObjReferance, 1f);

        if (currentLive <=0)
        {
            currentLive = 0;
            LiveStatus();
            playerAnimator.SetTrigger("Died");
            deathVfx.Play();
            gameObject.GetComponent<PlayerMovementManager>().enabled = false;
            gameObject.GetComponent<PlayerCombatManager>().enabled = false;
        }
        else
        {
            LiveStatus();
            takeDamageBloodVfx.Play();
        }

    }

    public void TakeHeal(int heal)
    {
        currentLive +=heal;
        if (currentLive > maxLive)
        {
            currentLive = maxLive;
            LiveStatus();
        }
        else
        {
            LiveStatus();
        }

    }

    void LiveStatus()
    {
        remaingLiveText.text = currentLive.ToString();
        remaingLiveImg.fillAmount = currentLive / maxLive;
    }

}
