using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeGateBehaviour : MonoBehaviour
{
    LevelMechPrototype levelStatus;

    // Start is called before the first frame update
    void Start()
    {
        levelStatus=FindAnyObjectByType<LevelMechPrototype>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            levelStatus.stages[levelStatus.currentStage-1].SetActive(false);
            levelStatus.stages[levelStatus.currentStage].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (levelStatus.currentStage - 1 >= 0)
            {
                levelStatus.bridges[levelStatus.currentStage - 1].SetActive(false);
            }
        }
    }
}
