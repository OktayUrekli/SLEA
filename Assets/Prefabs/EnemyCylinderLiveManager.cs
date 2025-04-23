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
        currentLiveImg.fillAmount = currentLive / maxCylinderLive;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Sword"))
        {
            EnemyTakeDamage(50);

        }
        if (collision.gameObject.CompareTag("Arrow"))
        {
            EnemyTakeDamage(30);
            Destroy(collision.gameObject);
        }
    }
    */
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
            currentLiveImg.fillAmount = currentLive / maxCylinderLive;
            gameObject.SetActive(false);
        }
        else
        {
            currentLiveImg.fillAmount = currentLive / maxCylinderLive;
        }
    }
}
