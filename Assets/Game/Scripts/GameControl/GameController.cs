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
        Initialize();
    }

    public Player LoadPlayer(Vector2 spawnPoint)
    {
        return Instantiate(_player, spawnPoint, Quaternion.identity);
    }

    public static void PlayerDied()
    {
        // Загрузка сцены GameOver
        G.SceneLoader.LoadScene("GameOver");
    }
}