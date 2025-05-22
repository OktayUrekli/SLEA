using TMPro;
using UnityEngine;

public class MenuCanvasManager : MonoBehaviour
{

    [SerializeField] GameObject[] panels;
    [SerializeField] TextMeshProUGUI seCount;


    void Start()
    {
        CloseAllPanels();
        ShowCollectedSECount();
    }

    void ShowCollectedSECount()
    {
        if (PlayerPrefs.HasKey("SECount"))
        {
            int se_count = PlayerPrefs.GetInt("SECount");
            seCount.text = se_count.ToString();
        }
        else
        {
            PlayerPrefs.SetInt("SECount", 0);
            int se_count = PlayerPrefs.GetInt("SECount");
            seCount.text = se_count.ToString();
        }
    }


    public void CloseAllPanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }



}