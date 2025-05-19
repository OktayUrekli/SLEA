using UnityEngine;

public class InLevelStageStatusMech : MonoBehaviour
{
    public GameObject[] stages; // içindeki npc sayýlarýna ulaþmak için ihtiyaç var
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

    void GetEnemiesAtTheStages() // her stage de ki enemy npc sayýsýný alýr
    { 
        for (int i = 0; i < stages.Length; i++)
        {
            enemyCountsAtStages[i]=stages[i].GetComponentsInChildren<IDamageable>().Length;
        }
    }

    void ControlEnemyStateAtStages() // stage de npc kalmayýnca gerçekleþecek olaylarý kontrol eder 
    {
        enemyCountsAtStages[currentStage] = stages[currentStage].GetComponentsInChildren<IDamageable>().Length; // stage de ki npc sayýsý alýnýyor
        
        // eðer stage üzerinde enemy npc kalmadýysa sonraki stage e geçiþ köprüsü aktif edilir
        if (enemyCountsAtStages[currentStage]==0 && (currentStage+1<stages.Length))
        {
            bridges[currentStage].GetComponent<BridgeGateBehaviour>().OpenGates();
            currentStage++;
        }
        if (enemyCountsAtStages[currentStage] == 0 && (currentStage +1==stages.Length))
        {
            Debug.Log("Bölüm bitti");
            Invoke("LevelSuccessful", 1f);
        }

    }

    void LevelSuccessful()
    {
        FindAnyObjectByType<InGameCanvasController>().LevelSuccessful();
    }
}
