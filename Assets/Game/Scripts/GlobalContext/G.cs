using Events;
using GameControl;

public static class G
{
    public static Player Player;
    
    public static UIManager UIManager;
    
    // Managers
    public static GameController GameController;
    public static PlayerInput PlayerInput;
    public static PlayerController PlayerController;
    public static SceneLoader SceneLoader;
    public static readonly EventManager EventManager = new();
    public static StatSystem StatSystem;
    
    public static PlayerStateMachine PlayerStateMachine;
    
    // Current level
    public static LevelManager LevelManager;
    
    // Mining
    public static GoldManager GoldManager;
    public static MiningSystem MiningSystem;
    
    // Effects
    public static VisualEffectsManager VisualEffectsManager;
    
    // Sound
    public static AudioManager AudioManager;
    
    public static PlayerHealth PlayerHealth;
}