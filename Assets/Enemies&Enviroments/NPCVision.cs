using UnityEngine;

public class NPCVision : MonoBehaviour
{
    [SerializeField] float viewRangeRadius = 15f;
    [SerializeField] float attackRangeRadius = 2; // atak alaný yarýçapý


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, viewRangeRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, attackRangeRadius);

    }
}
