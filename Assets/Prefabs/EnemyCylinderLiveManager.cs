using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class EnemyCylinderLiveManager : MonoBehaviour
{
    [SerializeField] float maxCylinderLive = 150;
    [SerializeField] Image currentLiveImg;
    [SerializeField] float currentLive;

    [SerializeField] GameObject floatingText;

    void Start()
    {
        currentLive = maxCylinderLive;
        EnemyLiveStatus();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Sword"))
        {
            EnemyTakeDamage(50);

        }
        if (other.CompareTag("Arrow"))
        {
            EnemyTakeDamage(30);
            Destroy(other.gameObject);
        }
    }

    void EnemyTakeDamage(int damage)
    {
        currentLive -= damage;

        GameObject floatingTextObjReferance = Instantiate(floatingText, transform.position, Quaternion.identity);
        floatingTextObjReferance.GetComponentInChildren<TextMesh>().text = damage.ToString();
        Destroy(floatingTextObjReferance, 1f);

        if (currentLive <= 0)
        {
            currentLive = 0;
            EnemyLiveStatus();
            gameObject.SetActive(false);
        }
        else
        {
            EnemyLiveStatus();
        }
    }

    void EnemyLiveStatus()
    {
        currentLiveImg.fillAmount = currentLive / maxCylinderLive;
    }
}
