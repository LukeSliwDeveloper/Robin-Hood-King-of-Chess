using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
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
