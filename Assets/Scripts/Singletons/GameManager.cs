using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public int FurthestCompletedLevel { get; private set; } = -1;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            DontDestroyOnLoad(gameObject);
        }
        return true;
    }

    public void LoadLevel(int levelIndex) => SceneManager.LoadScene(levelIndex);
}
