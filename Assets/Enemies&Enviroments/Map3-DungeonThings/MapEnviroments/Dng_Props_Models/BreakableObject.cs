using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] GameObject insObjectPrefab;// heal ya da SE oalcak
    [SerializeField] int insAmount; // preffab objeden ne kadar oluþturulacak onun sayýsý
    [SerializeField] GameObject objArmature;
    [SerializeField] ParticleSystem breakVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword") || other.gameObject.CompareTag("Arrow"))
        {
            MainDutyOnCollision();
            InstantiateObjcets();
        }
    }

    void MainDutyOnCollision()
    {
        objArmature.SetActive(false);
        breakVFX.Play();
        Destroy(gameObject, 0.3f);
    }

    void InstantiateObjcets()
    {
        for (int i = 0; i < insAmount; i++)
        {
            float x = Random.Range(-1,2);
            float z = Random.Range(-1,2);
            Vector3 insPos= transform.position+ new Vector3(x,0,z);
            Instantiate(insObjectPrefab,insPos,Quaternion.identity);
        }
    }
}
