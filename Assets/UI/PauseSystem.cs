using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
    private bool isPaused = false;
    private GameManager gm;
    private PlayerHealth PlayerHealth;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settignsPanel;
    
    [SerializeField] private GameObject DeathPanel;
    [SerializeField] private GameObject WinPanel;
    
    private void Start()
    {
        gm = GameManager.instance;
        if (gm != null)
        {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
        }
        
        PlayerHealth = PlayerHealth.GetInstance();
        
        pausePanel.SetActive(false);
        settignsPanel.SetActive(false);
        DeathPanel.SetActive(false);
        WinPanel.SetActive(false);
        
    }

    public void OnChangeGameStateCallback(GameState newState)
    {
        isPaused = newState != GameState.Play;

        if (PlayerHealth.win && !PlayerHealth.isDead)
        {
            WinPanel.SetActive(true);
            return;
        }
        
        if (PlayerHealth.isDead)
        {
            DeathPanel.SetActive(true);   
            return;
        }

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
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
