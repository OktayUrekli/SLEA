using System.Collections;
using UnityEngine;


public class InGameMusicsManager : MonoBehaviour
{

    [SerializeField] AudioClip[] gameMusics;
    [SerializeField] AudioSource myAudioSource;

    public static InGameMusicsManager instance;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        ShuffleArray(gameMusics);
        StartCoroutine(PlayMusic());
    }

    private void ShuffleArray(AudioClip[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            AudioClip temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    IEnumerator PlayMusic()
    {
        while (true)
        {
            foreach (AudioClip clip in gameMusics)
            {
                myAudioSource.clip = clip;
                yield return new WaitForSeconds(2f);
                myAudioSource.Play();

                // Sesin bitmesini bekle
                yield return new WaitForSeconds(clip.length);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}