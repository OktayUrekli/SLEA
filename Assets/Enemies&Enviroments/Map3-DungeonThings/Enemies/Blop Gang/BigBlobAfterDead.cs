using UnityEngine;

public class BigBlobAfterDead : MonoBehaviour
{
    [Header("Other Blobs Creating")]
    [SerializeField] GameObject midBlob;
    [SerializeField] Transform[] createBlobPos;

    public void CreateMidBlob()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(midBlob, createBlobPos[i].position, Quaternion.identity);
        }
    } 
}
