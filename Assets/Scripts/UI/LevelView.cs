using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private Button[] _levelButtons;
    [SerializeField] private AudioClip _clip;

    private void OnEnable()
    {
        var furthestLevel = Mathf.Min(GameManager.Instance.FurthestCompletedLevel, _levelButtons.Length);
        for (int i = 0; i <= furthestLevel; i++)
            _levelButtons[i].interactable = true;
    }

    public void LoadLevel(int levelIndex)
    {
        GameManager.Instance.PlaySfx(_clip);
        GameManager.Instance.LoadLevel(levelIndex);
    }
}
