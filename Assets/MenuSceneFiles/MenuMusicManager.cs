using System.Collections;
using UnityEngine;

public class MenuMusicsManager : MonoBehaviour
{
    [SerializeField] AudioClip[] menuMusics;
    [SerializeField] AudioSource myAudioSource;

    private void Awake()
    {
        if (FindAnyObjectByType<InGameMusicsManager>() != null) // levelden menuye dönünce game music manager yok edilmeli
        {
            Destroy(FindAnyObjectByType<InGameMusicsManager>().gameObject);
        }
        
    }

    private void Start()
    {
        Debug.Log("start çalýþtý menu m m");
        ShuffleArray(menuMusics);
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
            foreach (AudioClip clip in menuMusics)
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