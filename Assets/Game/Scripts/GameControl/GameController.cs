using System;
using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.Serialization;


public class GameController : Singleton<GameController>
{
    [SerializeField] private Player player;
    
    private readonly GameLoopStateMachine _gameLoopStateMachine = new();

    private void Initialize()
    {
       
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
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        G.EventManager.Unregister<SetGameStateEvent>(ChangeGameState);
    }

    public Player LoadPlayer(Vector3 spawnPoint)
    {
        return Instantiate(player, spawnPoint, Quaternion.identity);
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