using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Director;

    public delegate void _delegate();
    public _delegate OnGameStarted, OnGameOver, OnGamePaused;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    public void OnPlayerDied()
    {
        if(OnGameOver != null)
        {
            OnGameOver();
        }
    }

    internal void OnPlayButtonPressed()
    {
        if(OnGameStarted != null)
        {
            OnGameStarted();
        }

        Director.SetActive(true);
    }

    internal void OnRestartButtonPressed()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
