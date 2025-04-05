using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector2 playerSpawnPosition = new Vector2(0, 0);


    [SerializeField] private Button settingsButton;

    private void Start()
    {
        var player = GameController.Instance.LoadPlayer(playerSpawnPosition);
        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }
    }
    
    
    private void OnSettingsButtonPressed()
    {
        G.UIManager.ShowScreen("SettingsMenu");
    }
    
}