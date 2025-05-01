using UnityEngine;

public class NPCVision : MonoBehaviour
{
    [SerializeField] float viewRangeRadius = 15f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, viewRangeRadius);
    }
}
