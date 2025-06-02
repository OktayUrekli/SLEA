using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuCanvasManager : MonoBehaviour
{

    [SerializeField] GameObject[] panels;
    [SerializeField] TextMeshProUGUI seCount;

    [Header("Music Settings Variables")]
    [SerializeField] AudioMixer myAudioMixer;
    [SerializeField] Image musicOn;
    [SerializeField] Image musicOff;

    void Start()
    {
        CloseAllPanels();
        MusicSoundState();
        ShowCollectedSECount();
    }

    void MusicSoundState()
    {
        if (PlayerPrefs.HasKey("MusicState"))
        {
            int musicState=PlayerPrefs.GetInt("MusicState");
            if (musicState==1) // ses açýk
            {
                musicOn.gameObject.SetActive(true);
                musicOff.gameObject.SetActive(false);
                myAudioMixer.SetFloat("Volume", 0); // ses açýldý
            }
            else if (musicState==0)
            {
                musicOn.gameObject.SetActive(false);
                musicOff.gameObject.SetActive(true);
                myAudioMixer.SetFloat("Volume", -80); // ses kapatýldý
            }
        }
        else
        {
            PlayerPrefs.SetInt("MusicState", 1); // 1 ses açýk
            musicOn.gameObject.SetActive(true);
            musicOff.gameObject.SetActive(false);
        }
    }

    public void MusicStateButton()
    {
        int musicState = PlayerPrefs.GetInt("MusicState");
        if (musicState == 1) // ses açýksa
        {
            PlayerPrefs.SetInt("MusicState", 0); // 0 yap ve sesi kapat
            musicOn.gameObject.SetActive(false);
            musicOff.gameObject.SetActive(true);
            myAudioMixer.SetFloat("Volume", -80); // ses kapatýldý
        }
        else if (musicState == 0) // ses kapalýysa
        {
            PlayerPrefs.SetInt("MusicState", 1); // 1 yap ve sesi aç
            musicOn.gameObject.SetActive(true);
            musicOff.gameObject.SetActive(false);
            myAudioMixer.SetFloat("Volume", 0); // ses açýldý

        }
        PlayerPrefs.Save();
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