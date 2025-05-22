using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        NpcCountAtStages();
        StartCoroutine(ControleNpcCountAtLastStage());
    }

    // Update is called once per frame
    void Update()
    {      
        ControlEnemyStateAtStages();
    }

    void NpcCountAtStages() // stage lerdeki npc say�lar� tespit ediliyor
    {
        for (int i = 0; i < stages.Length; i++)
        {
            enemyCountsAtStages[i] = stages[i].GetComponentsInChildren<INPCController>().Length;
        }
    }

    IEnumerator ControleNpcCountAtLastStage() // her stage de ki enemy npc say�s�n� al�r
    {
        while (true)
        {
            if (enemyCountsAtStages[stages.Length - 1] == 0) // e�er son stage de npc kalmad�ysa 
            {
                yield return new WaitForSeconds(0.3f);
                NpcCountAtStages(); // tekrar stage lerdeki npc say�s� kontrol ediliyor.
                if (enemyCountsAtStages[stages.Length - 1] == 0) // 0.3 sn sonra hala son stage de npc yoksa b�l�m� bitir
                {
                    Debug.Log("B�l�m bitti");
                    Invoke("LevelSuccessful", 1f);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    void ControlEnemyStateAtStages() // stage de npc kalmay�nca ger�ekle�ecek olaylar� kontrol eder 
    {
        enemyCountsAtStages[currentStage] = stages[currentStage].GetComponentsInChildren<INPCController>().Length; // stage de ki npc say�s� al�n�yor
        
        // e�er stage �zerinde enemy npc kalmad�ysa sonraki stage e ge�i� k�pr�s� aktif edilir
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
        int lastOpenLevelIndex = PlayerPrefs.GetInt("lastOpenLevelIndex"); // sonraki lecelin kilidi a��l�yor
        if (currentLevelIndex==lastOpenLevelIndex && currentLevelIndex<10) // �imdilik 10 level var dahas� a��lamaz 
        {
            lastOpenLevelIndex++;
            PlayerPrefs.SetInt("lastOpenLevelIndex",lastOpenLevelIndex);
        }

        FindAnyObjectByType<InGameCanvasController>().SaveCollectedSECount(); // b�l�m ba�ar�l� bitince toplanan se ler kaydediliyor
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform bornPoint = stages[currentStage].transform.GetChild(0).transform;
            collision.gameObject.transform.position = new Vector3(bornPoint.position.x, bornPoint.position.y+2, bornPoint.position.z);
        }
    }

}
