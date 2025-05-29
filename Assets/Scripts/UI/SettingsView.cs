using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider, _sfxSlider;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private AudioClip _volumeClip;

    private void Awake()
    {
        if (_mixer.GetFloat("MusicVolume", out var volume))
            _musicSlider.value = Mathf.Pow(10f, volume / 20f);
        if (_mixer.GetFloat("SfxVolume", out volume))
            _sfxSlider.value = Mathf.Pow(10f, volume / 20f);
        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
    }

    public void PlayClick() => GameManager.Instance.PlaySfx(_clip);

    public void PlayVolume() => GameManager.Instance.PlaySfx(_volumeClip);

    public void Exit()
    {
        GameManager.Instance.PlaySfx(_clip);
        GameManager.Instance.LoadMenu(false);
    }

    private void ChangeMusicVolume(float amount)
    {
        GameManager.Instance.Save("MusicVolume", Mathf.Log10(amount) * 20f);
        _mixer.SetFloat("MusicVolume", Mathf.Log10(amount) * 20f);
    }

    private void ChangeSfxVolume(float amount)
    {
        GameManager.Instance.Save("SfxVolume", Mathf.Log10(amount) * 20f);
        _mixer.SetFloat("SfxVolume", Mathf.Log10(amount) * 20f);
    }
}
