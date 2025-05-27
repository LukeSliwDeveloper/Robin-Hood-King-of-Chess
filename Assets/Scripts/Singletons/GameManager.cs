using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private AudioSource[] _sfxSources;
    [SerializeField] private AudioMixer _mixer;

    public int FurthestCompletedLevel { get; private set; } = -1;

    private int _currentLevel;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            DontDestroyOnLoad(gameObject);
        }
        return true;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            _mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        if (PlayerPrefs.HasKey("SfxVolume"))
            _mixer.SetFloat("SfxVolume", PlayerPrefs.GetFloat("SfxVolume"));
    }

    public void PlaySfx(AudioClip clip)
    {
        foreach (AudioSource source in _sfxSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
            }
        }
    }

    public void Save(string keyName, float value)
    {
        PlayerPrefs.SetFloat(keyName, value);
        PlayerPrefs.Save();
    }

    public void LoadLevel(int levelIndex)
    {
        _currentLevel = levelIndex - 1;
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadMenu(bool won)
    {
        if (won)
            FurthestCompletedLevel = _currentLevel;
        SceneManager.LoadScene(0);
    }
}
