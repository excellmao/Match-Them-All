using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header("Panels")] 
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameoverPanel;


    public void GameStateChangedCallback(EGameState gameState)
    {
        menuPanel.SetActive(gameState == EGameState.MENU);
        gamePanel.SetActive(gameState == EGameState.GAME);
        levelCompletePanel.SetActive(gameState == EGameState.LEVELCOMPLETE);
        gameoverPanel.SetActive(gameState == EGameState.GAMEOVER);
    }
}
