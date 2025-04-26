using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using System.Linq;
using Game.Scripts.StateMachine.GameLoop;

public class EnemySpawner : MonoBehaviour
{
    #region Настройки в инспекторе

    [Header("Настройки спавна")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnCheckRadius = 1f;
    
    #endregion

    #region Приватные поля

    private bool _isSpawning = false;
    private int _currentWaveIndex = -1;
    private readonly List<Enemy> _activeEnemies = new();
    private Coroutine _currentSpawnWaveCoroutine;

    #endregion

    #region Unity методы

    private void OnEnable()
    {
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
        G.EventManager.Register<OnPlayerDeath>(OnPlayerDeath);
    }
    
    private void OnDisable()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
        G.EventManager.Unregister<OnPlayerDeath>(OnPlayerDeath);
    }

    #endregion

    #region Приватные методы

    private void OnPlayerDeath(OnPlayerDeath e)
    {
        DestroyEnemies();
        StopSpawn();
    }
    
    private void OnGameStateChange(OnGameStateChangedEvent e)
    {
        if (e.State == GameLoopStateMachine.GameLoopState.Ascend)
        {
            StartCoroutine(StartWaves());
        }
        else if (e.State == GameLoopStateMachine.GameLoopState.Shopping)
        {
            StopSpawn();
        }
    }
    
    private IEnumerator StartWaves()
    {
        _isSpawning = true;
        while (_isSpawning)
        {
            var newWave = StartNewWave();
            yield return new WaitForSeconds(newWave.timeBeforeNextWave);
        }
    }

    
    
    private void StopSpawn()
    {
        G.PlayerHealth.MakeInvincible();
        
        _isSpawning = false;
        
        StopAllCoroutines();
        
        
        
        //Запускаем анимацию уничтожения
        G.DeathStar.StartDestroy();
        
        //Красим всех живых врагов в черный цвет и останавливаем их
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = _activeEnemies[i];
            enemy.GetComponent<SpriteRenderer>().color = Color.black;
            enemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            
            if (enemy.GetComponent<RangedEnemy>() != null)
            {
                enemy.GetComponent<RangedEnemy>().DestroyAllProjectiles();
            }
        }
        
        //Уничтожаем врагов с медленной анимацией
        SlowKillEnemies();
        
        _currentWaveIndex = -1;
        
    } 
    private void SlowKillEnemies()
    {
        int enemiesToDestroy = _activeEnemies.Count;
        for (var i = enemiesToDestroy - 1; i >= 0; i--)
        {
            var enemy = _activeEnemies[i];
            enemy.SlowDie();
        }
    }
    
    
    
    
    
    private Wave StartNewWave()
    {
        _currentWaveIndex++;
        _currentWaveIndex = Mathf.Clamp(_currentWaveIndex, 0, waves.Count - 1);
        Wave currentWave = Wave.DeepCopy(waves[_currentWaveIndex]);
        
        if (_currentSpawnWaveCoroutine != null)
        {
            StopCoroutine(_currentSpawnWaveCoroutine);
        }
        
        _currentSpawnWaveCoroutine = StartCoroutine(SpawnWave(currentWave));
        return currentWave;
    }

    

    private void DestroyEnemies()
    {
        int enemiesToDestroy = _activeEnemies.Count;
        for (var i = enemiesToDestroy - 1; i >= 0; i--)
        {
            var enemy = _activeEnemies[i];
            enemy.Die();
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
         while (wave.enemyCount > 0)
         {
             yield return new WaitUntil(() => _activeEnemies.Count < wave.maxEnemiesAtOnce);
             
             SpawnEnemy();
             wave.enemyCount--;
             yield return new WaitForSeconds(wave.timeBetweenSpawns);
         }
    }
    
    private void RemoveEnemy(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    private void SpawnEnemy()
    {
        if (!_isSpawning) return;
        
        Transform spawnPoint = GetSuitableSpawnPoint();
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.Init(G.Player.transform);
        enemy.OnDie += RemoveEnemy;
        _activeEnemies.Add(enemy);
    }

    private Transform GetSuitableSpawnPoint()
    {
        var suitablePoints = spawnPoints
            .Where(point => !Physics2D.OverlapCircle(point.position, spawnCheckRadius, obstacleLayer))
            .ToList();

        return suitablePoints.Any() 
            ? suitablePoints[Random.Range(0, suitablePoints.Count)]
            : spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
    #endregion
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public int enemyCount;
    public float timeBetweenSpawns = 1.5f;
    public float timeBeforeNextWave = 5f;
    public int maxEnemiesAtOnce = 5;
    public bool isBossWave = false;

    public static Wave DeepCopy(Wave source)
    {
        return new Wave
        {
            waveName = source.waveName,
            enemyCount = source.enemyCount,
            timeBetweenSpawns = source.timeBetweenSpawns,
            timeBeforeNextWave = source.timeBeforeNextWave,
            maxEnemiesAtOnce = source.maxEnemiesAtOnce,
            isBossWave = source.isBossWave
        };
    }
}