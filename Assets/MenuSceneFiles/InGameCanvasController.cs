using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCanvasController : MonoBehaviour
{
    [SerializeField] GameObject levelSuccessPanel;
    [SerializeField] GameObject levelDefeatPanel;
    [SerializeField] GameObject[] panels;

    [SerializeField] TextMeshProUGUI collectedSECountText;
    int collectedSECount;

    void Start()
    {
        Time.timeScale = 1;
        collectedSECount = 0;
        UpdateSECount(collectedSECount);
        CloseAllPanels();
    }


    public void CloseAllPanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }

   
    public void UpdateSECount(int seAmount)
    {
        collectedSECount += seAmount;
        collectedSECountText.text= collectedSECount.ToString();
    }

    public void LevelSuccessful()
    {
        levelSuccessPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void LevelDefeat()
    {
        levelDefeatPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // ayný sahne en baþtan tekrar yükleniyor
    }

    public void NextLevelButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1); // sonraki level yükleniyor
    }

    public void ReturnMenuButton()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
    }
}
