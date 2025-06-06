using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] levelPages; // i�erisinde 5 b�l�m butonu i�eren paneller bulunur
    [SerializeField] Button[] levelButtons; // t�klan�ld���nda ilgili b�l�m�n sahnesini a�an butonlar bulunur

    int currentPageIndex;
    int lastOpenLevelIndex;

    private void Start()
    {
        ActivenessOfPages();

        ActivenessOfLevelButtons();

    }

    // b�t�n sayfalar� g�r�nmez yapt�ktan sonra sadece ilk sayfay� g�r�n�r yapar
    void ActivenessOfPages()
    {
        for (int i = 0; i < levelPages.Length; i++)
        {
            levelPages[i].SetActive(false);
        }
        currentPageIndex = 0;
        levelPages[currentPageIndex].SetActive(true);
    }


    // ge�ilen ve oynanabilir b�l�mlerin butonlar�n� g�r�n�r di�erlerini g�r�nmez k�lan fonksiyon
    void ActivenessOfLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }

        if (PlayerPrefs.HasKey("lastOpenLevelIndex"))
        {
            lastOpenLevelIndex = PlayerPrefs.GetInt("lastOpenLevelIndex");
            for (int i = 0; i < lastOpenLevelIndex; i++)
            {
                levelButtons[i].interactable = true;
            }
            
        }
        else
        {
            PlayerPrefs.SetInt("lastOpenLevelIndex", 1); // sadece ilk level açık oluyor başlangıçta
            lastOpenLevelIndex = PlayerPrefs.GetInt("lastOpenLevelIndex");
            levelButtons[lastOpenLevelIndex-1].interactable = true; // ilk level butonu açılıyor
        }
    }

    public void PreviusPageButton()
    {
        if (currentPageIndex > 0)
        {
            levelPages[currentPageIndex].SetActive(false);
            currentPageIndex--;
            levelPages[currentPageIndex].SetActive(true);
        }

    }

    public void NextPageButton()
    {
        if (currentPageIndex < levelPages.Length - 1)
        {
            levelPages[currentPageIndex].SetActive(false);
            currentPageIndex++;
            levelPages[currentPageIndex].SetActive(true);
        }
    }


    // b�l�m butonlar�na atanan fonksiyon
    // t�klan�ld���nda ilgili b�l�me g�t�r�r
    public void GoToLevelButton(string levelName)
    {
        //mobile s�r�m i�in aktif edilecek
        SceneManager.LoadScene(levelName);

    }
}