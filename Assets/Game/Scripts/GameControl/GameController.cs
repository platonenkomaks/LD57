using Game.Scripts.Events;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private Player _player;

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
        G.EventManager.Register<TestEvent>(testEvent => Debug.Log("TestEvent triggered!"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            G.EventManager.Trigger(new TestEvent());
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