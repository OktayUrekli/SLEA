using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InLevelStageStatusMech : MonoBehaviour
{
    public GameObject[] stages; // içindeki npc sayýlarýna ulaþmak için ihtiyaç var
    public GameObject[] bridges; 
    [SerializeField] GameObject[][] enemies;
    public int currentStage;

    public int[] enemyCountsAtStages;

    void Start()
    {       
        enemyCountsAtStages = new int[stages.Length];
        NpcCountAtStages();
        StartCoroutine(ControleNpcCountAtLastStage());
        Debug.Log(PlayerPrefs.GetInt("lastOpenLevelIndex") + "en son açýk olan level");
    }

    void Update()
    {      
        ControlEnemyStateAtStages();
    }

    void NpcCountAtStages() // stage lerdeki npc sayýlarý tespit ediliyor
    {
        for (int i = 0; i < stages.Length; i++)
        {
            enemyCountsAtStages[i] = stages[i].GetComponentsInChildren<INPCController>().Length;
        }
    }

    IEnumerator ControleNpcCountAtLastStage() // her stage de ki enemy npc sayýsýný alýr
    {
        while (true)
        {
            if (enemyCountsAtStages[stages.Length - 1] == 0) // eðer son stage de npc kalmadýysa 
            {
                yield return new WaitForSeconds(0.3f);
                NpcCountAtStages(); // tekrar stage lerdeki npc sayýsý kontrol ediliyor.
                if (enemyCountsAtStages[stages.Length - 1] == 0) // 0.3 sn sonra hala son stage de npc yoksa bölümü bitir
                {
                    Debug.Log("Bölüm bitti");
                    Invoke("LevelSuccessful", 1f);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    void ControlEnemyStateAtStages() // stage de npc kalmayýnca gerçekleþecek olaylarý kontrol eder 
    {
        enemyCountsAtStages[currentStage] = stages[currentStage].GetComponentsInChildren<INPCController>().Length; // stage de ki npc sayýsý alýnýyor
        
        // eðer stage üzerinde enemy npc kalmadýysa sonraki stage e geçiþ köprüsü aktif edilir
        if (enemyCountsAtStages[currentStage]==0 && (currentStage+1<stages.Length))
        {
            bridges[currentStage].GetComponent<BridgeGateBehaviour>().OpenGates();
            currentStage++;
        }

    }

    void LevelSuccessful()
    {
        FindAnyObjectByType<InGameCanvasController>().LevelSuccessful();

        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int lastOpenLevelIndex = PlayerPrefs.GetInt("lastOpenLevelIndex"); // sonraki lecelin kilidi açýlýyor
        if (currentLevelIndex==lastOpenLevelIndex && currentLevelIndex<10) // þimdilik 10 level var dahasý açýlamaz 
        {
            lastOpenLevelIndex+=1;
            PlayerPrefs.SetInt("lastOpenLevelIndex",lastOpenLevelIndex);
        }

        FindAnyObjectByType<InGameCanvasController>().SaveCollectedSECount(); // bölüm baþarýlý bitince toplanan se ler kaydediliyor
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform bornPoint = stages[currentStage].transform.GetChild(0).transform; // reborn point 0. indexte bulunmalýdýr
            collision.gameObject.transform.position = new Vector3(bornPoint.position.x, bornPoint.position.y+2, bornPoint.position.z);
        }
    }

}
