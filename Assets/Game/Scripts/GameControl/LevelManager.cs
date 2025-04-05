using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject playerSpawnPosition;
    [SerializeField] private Button settingsButton;

    public CinemachineTarget cinemachineTarget;
    private void Start()
    {
        var player = GameController.Instance.LoadPlayer(playerSpawnPosition.transform.position);
        cinemachineTarget.SetTargetForCinemachineCamera(player.transform);
        
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