using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string NombreDeEscena;
    
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject settingsUI;

    private void Start()
    {
        mainUI.SetActive(true); 
        settingsUI.SetActive(false);
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene(NombreDeEscena);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
