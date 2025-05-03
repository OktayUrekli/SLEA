using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlobAfterDead : MonoBehaviour
{
    [Header("Other Blobs Creating")]
    [SerializeField] GameObject midBlob;
    [SerializeField] Transform createBlobPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMidBlob()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(midBlob, createBlobPos.position, Quaternion.identity);
        }
    } 
}
