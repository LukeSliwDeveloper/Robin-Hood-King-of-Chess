using TMPro;
using UnityEngine;

public class EndView : MonoBehaviour
{
    [SerializeField] private TMP_Text _infoLabel;
    [SerializeField] private TMP_Text _nextButtonLabel;
    [SerializeField] private AudioClip _clip;

    private bool _won;

    private void Awake()
    {
        BoardManager.Instance.OnGameOver += Activate;
        gameObject.SetActive(false);
    }

    public void Return()
    {
        GameManager.Instance.PlaySfx(_clip);
        GameManager.Instance.LoadMenu(_won);
    }

    public void LoadLevel()
    {
        GameManager.Instance.PlaySfx(_clip);
        if (_won && (GameManager.Instance.CurrentLevel != GameManager.Instance.LevelsAmount))
            GameManager.Instance.LoadLevel(GameManager.Instance.CurrentLevel + 1);
        else
            GameManager.Instance.LoadLevel(GameManager.Instance.CurrentLevel);
    }

    private void Activate(bool won)
    {
        _won = won;
        _infoLabel.text = won ? $"Level {GameManager.Instance.CurrentLevel} Complete" : "Level Failed";
        _nextButtonLabel.text = (won && (GameManager.Instance.CurrentLevel != GameManager.Instance.LevelsAmount)) ? $"Level {GameManager.Instance.CurrentLevel + 1}" : "Retry";
        gameObject.SetActive(true);
    }
}
