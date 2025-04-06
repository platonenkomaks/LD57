using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject playerSpawnPosition;
    [SerializeField] private Button settingsButton;
    
    public CinemachineTarget cinemachineTarget;
    public TileGrid tileGrid;
    
    private Player _player;
    

    private void Start()
    {
        _player = GameController.Instance.LoadPlayer(playerSpawnPosition.transform.position);
        G.Player = _player;
        cinemachineTarget.SetTargetForCinemachineCamera(_player.transform);
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }
        
        G.MiningSystem.removableTilemap = tileGrid.removableTilemap;
        G.MiningSystem.goldTilemap = tileGrid.goldTilemap;
    }
    
    private void OnSettingsButtonPressed()
    {
        G.UIManager.ShowScreen("SettingsMenu");
    }
}