using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private Button[] _levelButtons;

    private void OnEnable()
    {
        var furthestLevel = Mathf.Min(GameManager.Instance.FurthestCompletedLevel + 2, _levelButtons.Length);
        for (int i = 0; i < furthestLevel; i++)
            _levelButtons[i].interactable = true;
    }

    public void LoadLevel(int levelIndex) => GameManager.Instance.LoadLevel(levelIndex);
}
