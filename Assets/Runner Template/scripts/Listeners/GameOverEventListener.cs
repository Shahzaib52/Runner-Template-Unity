using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEventListener : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnGameOver += OnGameOver;
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.instance.OnGameOver -= OnGameOver;
    }

    void OnGameOver()
    {
        panel.SetActive(true);
    }

    public void OnRestartButtonPressed()
    {
        GameManager.instance.OnRestartButtonPressed();
    }
}
