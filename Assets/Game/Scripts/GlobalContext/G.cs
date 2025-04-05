using Game.Scripts.Events;

public static class G
{
    public static UIManager UIManager;

    // Managers
    public static GameController GameController;
    public static PlayerInput PlayerInput;
    public static PlayerController PlayerController;
    public static SceneLoader SceneLoader;
    public static readonly EventManager EventManager = new();

    // Current level
    public static LevelManager LevelManager;

    
    // Mining
    public static MiningSystem MiningSystem;

    
    // Effects
    public static VisualEffectsManager VisualEffectsManager;
    
    // Sound
    public static AudioManager AudioManager;
}