using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource[] _sfxSources;
    [SerializeField] private AudioMixer _mixer;

    public int FurthestCompletedLevel { get; private set; }
    public int LevelsAmount { get; private set; } = 10;
    public int CurrentLevel { get; private set; }

    protected override bool Awake()
    {
        if (base.Awake())
        {
            if (PlayerPrefs.HasKey("FurthestCompletedLevel"))
                FurthestCompletedLevel = PlayerPrefs.GetInt("FurthestCompletedLevel");
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
                return;
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
        FadeMusic(false);
        CurrentLevel = levelIndex;
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadNextLevel()
    {
        FadeMusic(false);
        if (FurthestCompletedLevel < CurrentLevel)
            FurthestCompletedLevel = CurrentLevel;
        PlayerPrefs.SetInt("FurthestCompletedLevel", FurthestCompletedLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(++CurrentLevel);
    }

    public void LoadMenu(bool won)
    {
        FadeMusic(false);
        if (won)
        {
            if (FurthestCompletedLevel < CurrentLevel)
                FurthestCompletedLevel = CurrentLevel;
            PlayerPrefs.SetInt("FurthestCompletedLevel", FurthestCompletedLevel);
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene(0);
    }

    public void FadeMusic(bool toZero)
    {
        _musicSource.DOFade(toZero ? 0f : 1f, .1f);
    }
}
