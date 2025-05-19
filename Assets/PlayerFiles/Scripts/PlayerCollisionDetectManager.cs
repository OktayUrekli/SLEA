using UnityEngine;

public class PlayerCollisionDetectManager : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Spike"))
        {
            gameObject.GetComponent<PlayerLiveManager>().TakeDamage(15);

        }
        
        if (other.CompareTag("Heal"))
        {
            gameObject.GetComponent<PlayerLiveManager>().TakeHeal(30);
            Destroy(other.gameObject);
        }
    }
}
