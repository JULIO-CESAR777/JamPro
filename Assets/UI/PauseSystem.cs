using System;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    private bool isPaused = false;
    private GameManager gm;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settignsPanel;

    private void Start()
    {
        gm = GameManager.instance;
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }
        
        pausePanel.SetActive(false);
        settignsPanel.SetActive(false);
        
    }

    public void OnChangeGameStateCallback(GameState newState)
    {
        isPaused = newState != GameState.Play;
        if (isPaused)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
            settignsPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        settignsPanel.SetActive(!settignsPanel.activeSelf);
    }

    public void ClosePause()
    {
        gm.ChangeGameState(isPaused ? GameState.Play : GameState.Pause);
    }

}
