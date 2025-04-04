using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector2 playerSpawnPosition = new Vector2(0, 0);

    private CameraFollow _cameraFollow;

    [SerializeField] private Button settingsButton;

    private void Awake()
    {
        if (Camera.main != null) _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        var player = GameController.Instance.LoadPlayer(playerSpawnPosition);
        _cameraFollow.SetTarget(player.transform);
        
        
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