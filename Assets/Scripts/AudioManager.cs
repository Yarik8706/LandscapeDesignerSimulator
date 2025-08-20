using DefaultNamespace;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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

    public void PlayPlaceSound()
    {
        var clip = GameDataManager.Instance.gameData.placeSound;
        if (clip != null) _sfxSource.PlayOneShot(clip);
    }

    public void PlayDestroySound()
    {
        var clip = GameDataManager.Instance.gameData.destroySound;
        if (clip != null) _sfxSource.PlayOneShot(clip);
    }
}

