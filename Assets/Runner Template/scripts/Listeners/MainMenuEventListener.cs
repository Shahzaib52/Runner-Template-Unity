using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEventListener : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnGameStarted += OnGameStarted;
    }
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.instance.OnGameOver -= OnGameStarted;
    }

    void OnGameStarted()
    {
        panel.SetActive(false);
    }

    public void OnPlayButtonPressed()
    {
        GameManager.instance.OnPlayButtonPressed();
    }
}
