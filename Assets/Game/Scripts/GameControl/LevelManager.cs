using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject playerSpawnPosition;
    [SerializeField] private Button settingsButton;
    
    public PlayerCinemachineTarget playerCinemachineTarget;
    public TileGrid tileGrid;
    
    public BatteryLightUI batteryLightUI;
    public Tilemap highlightTilemap;
    private Player _player;
    

    private void Start()
    {
        _player = GameController.Instance.LoadPlayer(playerSpawnPosition.transform.position);
        G.Player = _player;
        playerCinemachineTarget.SetTargetForCinemachineCamera(_player.transform);

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }
        
        G.MiningSystem.removableTilemap = tileGrid.removableTilemap;
        G.MiningSystem.goldTilemap = tileGrid.goldTilemap;
        G.MiningSystem.highlightTilemap = highlightTilemap;

       batteryLightUI.Init();

    }
    
    private void OnSettingsButtonPressed()
    {
        G.UIManager.ShowScreen("SettingsMenu");
    }
}