using UnityEngine;

public class PlayerCollisionDetectManager : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            gameObject.GetComponent<PlayerLiveManager>().TakeDamage(50);
            other.gameObject.SetActive(false);
        }
        
        if (other.CompareTag("Heal"))
        {
            gameObject.GetComponent<PlayerLiveManager>().TakeHeal(30);
            other.gameObject.SetActive(false);
        }
    }
}
