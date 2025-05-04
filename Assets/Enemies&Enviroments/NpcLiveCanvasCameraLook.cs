using UnityEngine;

public class NpcLiveCanvasCameraLook : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position+Camera.main.transform.forward);
        }
    }
}
