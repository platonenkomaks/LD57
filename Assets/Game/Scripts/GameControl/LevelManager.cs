using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 0,-1);


    [SerializeField] private Button settingsButton;

    public CinemachineTarget cinemachineTarget;
    private void Start()
    {
        var player = GameController.Instance.LoadPlayer(playerSpawnPosition);
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