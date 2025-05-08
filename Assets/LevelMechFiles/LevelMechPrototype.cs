using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMechPrototype : MonoBehaviour
{
    public GameObject[] stages;
    public GameObject[] bridges;
    [SerializeField] GameObject[][] enemies;
    public int currentStage;

    public int[] enemyCountsAtStages;

    // Start is called before the first frame update
    void Start()
    {
        enemyCountsAtStages = new int[stages.Length];
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].SetActive(false);
        }
        for (int i = 0; i < bridges.Length; i++)
        {
            bridges[i].SetActive(false);
        }
        currentStage = 0;
        stages[currentStage].SetActive(true);
        GetEnemiesAtTheStages();
    }

    // Update is called once per frame
    void Update()
    {
        ControlEnemyStateAtStages();
    }

    void GetEnemiesAtTheStages()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            enemyCountsAtStages[i]=stages[i].GetComponentsInChildren<IDamageable>().Length;
        }
    }

    void ControlEnemyStateAtStages()
    {
        enemyCountsAtStages[currentStage] = stages[currentStage].GetComponentsInChildren<IDamageable>().Length;
        // e�er stage �zerinde enemy npc kalmad�ysa sonraki stage e ge�i� k�pr�s� aktif edilir
        if (enemyCountsAtStages[currentStage]==0 && (currentStage+1<stages.Length))
        {
            bridges[currentStage].SetActive(true); // ara k�pr� aktif edildi
            currentStage++;
        }
        else
        {
            Debug.Log("B�l�m bitti");
        }
    }
}
