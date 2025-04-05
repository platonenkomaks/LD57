using Game.Scripts.Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private Player _player;
    
    private readonly GameLoopStateMachine _gameLoopStateMachine = new();

    private void Initialize()
    {
        _player = Resources.Load<Player>("Prefabs/Player");

        if (_player == null)
        {
            Debug.LogError("Player prefab not found");
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        
        Initialize();
    }

    // Sample use of the EventManager
    private void Start()
    {
        _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Tutorial);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Tutorial);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Descend);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Mining);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Ascend);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _gameLoopStateMachine.SetState(GameLoopStateMachine.GameLoopState.Shopping);
        }
    }

    public Player LoadPlayer(Vector3 spawnPoint)
    {
        return Instantiate(_player, spawnPoint, Quaternion.identity);
    }

    public static void PlayerDied()
    {
        // Загрузка сцены GameOver
        G.SceneLoader.LoadScene("GameOver");
    }
}