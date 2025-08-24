using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySoundOnSpawn : MonoBehaviour
{
    public AudioClip spawnSound; 
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (spawnSound != null)
        {
            audioSource.spatialBlend = 0;
            audioSource.PlayOneShot(spawnSound);
        }
        SceneFader fader = FindAnyObjectByType<SceneFader>();
        //fader.StartFadeOut();
        Invoke("LoadScene", 4);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("심상공간");
    }
}