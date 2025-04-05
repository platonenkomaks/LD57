using System.Collections;
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
    private IEnumerator Start()
    {
        G.EventManager.Register<SetGameStateEvent>(ChangeGameState);
        
        // Skip a frame to let everything finish initializing
        yield return null;
        
        ChangeGameState(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Tutorial });
        
        // TEST
        yield return null;
        ChangeGameState(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Shopping });
    }

    private void OnDestroy()
    {
        G.EventManager.Unregister<SetGameStateEvent>(ChangeGameState);
    }

    private void Update()
    {}

    public Player LoadPlayer(Vector3 spawnPoint)
    {
        return Instantiate(_player, spawnPoint, Quaternion.identity);
    }

    public static void PlayerDied()
    {
        // Загрузка сцены GameOver
        G.SceneLoader.LoadScene("GameOver");
    }

    private void ChangeGameState(SetGameStateEvent e)
    {
        _gameLoopStateMachine.SetState(e.State);
    }
}