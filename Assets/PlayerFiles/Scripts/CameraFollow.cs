using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; // Takip edilecek karakter
    public Vector3 offset = new Vector3(0, 10, -10); // Kamera açýsý ve mesafesi
  

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.transform.position + offset;
        transform.position = desiredPosition;

        transform.LookAt(player.transform.position); // Karaktere doðru bakmasý için
    }
}

