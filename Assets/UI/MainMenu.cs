using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string NombreDeEscena;
    [SerializeField] private string url;
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

    public void OpenLink()
    {
        Application.OpenURL(url);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
