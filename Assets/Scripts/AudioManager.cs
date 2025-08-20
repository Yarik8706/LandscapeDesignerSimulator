using UnityEngine;
using DefaultNamespace;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private void Awake()
    {
        Instance = this;
        _musicSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        var data = GameDataManager.Instance.gameData;
        if (data.backgroundMusic != null)
        {
            _musicSource.clip = data.backgroundMusic;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    public void PlayBuildSound()
    {
        var clip = GameDataManager.Instance.gameData.buildSound;
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBreakSound()
    {
        var clip = GameDataManager.Instance.gameData.breakSound;
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
}

