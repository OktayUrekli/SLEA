using UnityEngine;

public class MenuCanvasManager : MonoBehaviour
{

    [SerializeField] GameObject[] panels;

    void Start()
    {
        CloseAllPanels();
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