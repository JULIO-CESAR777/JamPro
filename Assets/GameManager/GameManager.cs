using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ----- SingleTon ---------
        #region Singleton
        public static GameManager instance { get; private set; }
        public static GameManager GetInstance() => instance;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
    
            instance = this;
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        #endregion
        // ------ Fin del singleton  ---------
    
        public GameState gameState;
        public Action<GameState> onChangeGameState;
        
        public bool canPause;
        
        private void Start()
        {
            gameState = GameState.Play;
            canPause = true;
            
        }
    
        public void PauseGame()
        {
            if (canPause)
            {
                if (gameState == GameState.Pause)
                {
                    ChangeGameState(GameState.Play);
                }
                else if (gameState == GameState.Play)
                {
                    ChangeGameState(GameState.Pause);
                }
            }
        }
    
        public void ChangeGameState(GameState newGameState)
        {
            if (!canPause) return;
            
            gameState = newGameState;
            onChangeGameState?.Invoke(gameState);
        }
        
        /*
         How to call the pause system...
         Create a variable: private bool isPaused = false; MainManager gm;
         
         Then set it in the start method:
         gm = MainManager.GetInstance();
         if (gm != null)
         {
            gm.onChangeGameState += OnChangeGameStateCallback;

            if (gm.gameState == GameState.Pause)
                isPaused = true;
         }
         
         And just create the method that is gonna be called:
          public void OnChangeGameStateCallback(GameState newState){
            isPaused = newState != GameState.Play;
            if(isPaused){}else{}
          }
          
         
        */ 
}

public enum GameState
{
    Play,
    Pause,
    GameOver
}