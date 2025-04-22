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

    private int _currentWaveIndex = -1;
    private readonly List<Enemy> _activeEnemies = new();
    private Coroutine _currentSpawnWaveCoroutine;

    #endregion

    #region Unity методы

    private void Start()
    {
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
    }

    #endregion

    #region Приватные методы
    
    private void OnGameStateChange(OnGameStateChangedEvent e)
    {
        if (e.State == GameLoopStateMachine.GameLoopState.Ascend)
        {
            StartCoroutine(StartWaves());
        }
        else if (e.State == GameLoopStateMachine.GameLoopState.Shopping)
        {
            StopAllCoroutines();
            DestroyEnemies();
        }
    }
    
    private IEnumerator StartWaves()
    {
        while (_currentWaveIndex < waves.Count - 1)
        {
            var newWave = StartNewWave(); 
            yield return new WaitForSeconds(newWave.timeBeforeNextWave);
        }
    }

    private Wave StartNewWave()
    {
        _currentWaveIndex++;
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
        _activeEnemies.ForEach(enemy => enemy.Die());
        _activeEnemies.Clear();
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        print("Starting wave spawning: " + wave.enemyCount);
         while (wave.enemyCount > 0)
         {
             print("Checking if can spawn: " + _activeEnemies.Count + " < " + wave.maxEnemiesAtOnce);
             yield return new WaitUntil(() => _activeEnemies.Count < wave.maxEnemiesAtOnce);
             
             SpawnEnemy();
             wave.enemyCount--;
             print("Spawning enemy, remaining: " + wave.enemyCount);
             yield return new WaitForSeconds(wave.timeBetweenSpawns);
         }
    }
    
    private void RemoveEnemy(Enemy enemy)
    {
        print("Enemy died");
        _activeEnemies.Remove(enemy);
    }

    private void SpawnEnemy()
    {
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