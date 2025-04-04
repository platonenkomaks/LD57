using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour

{
    [SerializeField] private string newGameSceneName = "Game";
    
    [Header("UI Elements")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    
    [SerializeField] private GameObject optionsScreen;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
  
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OnOptionsClicked);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OnCreditsClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }

    private void OnNewGameClicked()
    {
        G.SceneLoader.LoadScene(newGameSceneName);
    }

    private void OnContinueClicked()
    {
        // Продолжить игру с последнего сохранения
    }

    private void OnOptionsClicked()
    {
        optionsScreen.SetActive(true);
    }

    private void OnCreditsClicked()
    {
        G.SceneLoader.LoadScene("Credits");
    }

    private void OnExitClicked()
    {
        // Выход из игры
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}