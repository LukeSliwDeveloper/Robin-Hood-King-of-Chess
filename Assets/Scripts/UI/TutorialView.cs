using TMPro;
using UnityEngine;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private Sprite[] _tutorialSprites;
    [SerializeField] private string[] _tutorialTexts;
    [SerializeField] private TMP_Text _infoLabel;

    private void Awake()
    {
        if (GameManager.Instance.CurrentLevel <= _tutorialSprites.Length && !PlayerPrefs.HasKey($"Tutorial {GameManager.Instance.CurrentLevel}"))
        {
            PlayerPrefs.SetInt($"Tutorial {GameManager.Instance.CurrentLevel}", 0);
            _infoLabel.text = _tutorialTexts[GameManager.Instance.CurrentLevel - 1];
        }
        else
            gameObject.SetActive(false);
    }
}
