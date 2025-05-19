using UnityEngine;

public class InLevelStageStatusMech : MonoBehaviour
{
    public GameObject[] stages; // i�indeki npc say�lar�na ula�mak i�in ihtiya� var
    public GameObject[] bridges; 
    [SerializeField] GameObject[][] enemies;
    public int currentStage;

    public int[] enemyCountsAtStages;

    // Start is called before the first frame update
    void Start()
    {       
        enemyCountsAtStages = new int[stages.Length];
        GetEnemiesAtTheStages();
    }

    // Update is called once per frame
    void Update()
    {
        GetEnemiesAtTheStages();
        ControlEnemyStateAtStages();
    }

    void GetEnemiesAtTheStages() // her stage de ki enemy npc say�s�n� al�r
    { 
        for (int i = 0; i < stages.Length; i++)
        {
            enemyCountsAtStages[i]=stages[i].GetComponentsInChildren<IDamageable>().Length;
        }
    }

    void ControlEnemyStateAtStages() // stage de npc kalmay�nca ger�ekle�ecek olaylar� kontrol eder 
    {
        enemyCountsAtStages[currentStage] = stages[currentStage].GetComponentsInChildren<IDamageable>().Length; // stage de ki npc say�s� al�n�yor
        
        // e�er stage �zerinde enemy npc kalmad�ysa sonraki stage e ge�i� k�pr�s� aktif edilir
        if (enemyCountsAtStages[currentStage]==0 && (currentStage+1<stages.Length))
        {
            bridges[currentStage].GetComponent<BridgeGateBehaviour>().OpenGates();
            currentStage++;
        }
        if (enemyCountsAtStages[currentStage] == 0 && (currentStage +1==stages.Length))
        {
            Debug.Log("B�l�m bitti");
            Invoke("LevelSuccessful", 1f);
        }

    }

    void LevelSuccessful()
    {
        FindAnyObjectByType<InGameCanvasController>().LevelSuccessful();
    }
}
