using System.Collections;
using System.Collections.Generic;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class EnemyDirector : MonoBehaviour
{
    #region Структуры данных

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
        public float timeBetweenSpawns = 1.5f;
        public float timeBeforeNextWave = 5f;
        public int maxEnemiesAtOnce = 5;
        public bool isBossWave = false;

        public Wave DeepCopy()
        {
            return new Wave()
            {
                waveName = waveName,
                enemies = new List<EnemySpawnInfo>(enemies),
                timeBetweenSpawns = timeBetweenSpawns,
                timeBeforeNextWave = timeBeforeNextWave,
                maxEnemiesAtOnce = maxEnemiesAtOnce,
                isBossWave = isBossWave
            };
        }
    }

    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnChance; // Вероятность спавна от 0 до 1
    }

    #endregion

    #region Настройки в инспекторе

    [Header("Настройки спавна")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private bool autoStartWaves = true;
    [SerializeField] private bool loopWaves = false;
    [SerializeField] private float initialDelay = 3f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnCheckRadius = 1f;

    [Header("События")] 
    public UnityEvent onWaveStart;
    public UnityEvent onWaveComplete;
    public UnityEvent onAllWavesComplete;
    public UnityEvent<int> onWaveNumberChanged;
    public UnityEvent<int, int> onEnemyCountChanged; // текущее количество, максимальное количество

    #endregion

    #region Приватные поля

    private int _currentWaveIndex = -1;
    private int _totalEnemiesSpawned = 0;
    private int _totalEnemiesRemaining = 0;
    private readonly List<GameObject> _activeEnemies = new List<GameObject>();
    private bool _isSpawning = false;
    private Coroutine _spawnCoroutine;
    private bool _waveActive = false;
    private readonly Dictionary<int, int> _enemiesRemainingInWave = new();

    #endregion

    #region Unity методы

    private IEnumerator Start()
    {
        yield return null;
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
        InitializeWaveEnemyCounts();

        if (autoStartWaves)
        {
            StartCoroutine(DelayedStart());
        }
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
    }

    private void Update()
    {
        CleanupDestroyedEnemies();
        UpdateEnemyCounter();
        CheckWaveCompletion();
    }

    #endregion

    #region Публичные методы
    

    public void StopAllWaves()
    {
        StopAllCoroutines();
    }

    public void RestartWaves()
    {
        ResetWaveState();
        ResetEnemies();
        ResetEnemyCounts();
        StartCoroutine(DelayedStart());
    }

    public IEnumerator StartWavesWithIncreasingDelays(float initialDelay, float delayIncrement)
    {
        float currentDelay = initialDelay;

        while (_currentWaveIndex < waves.Count || loopWaves)
        {
            StartNextWave(); 

        
            yield return new WaitForSeconds(currentDelay);
            currentDelay += delayIncrement; // Увеличиваем задержку
        }

        onAllWavesComplete?.Invoke(); 
    }

    public void StartNextWave()
    {
        _currentWaveIndex++;

        if (!CheckWaveAvailability())
        {
            return;
        }

        InitializeNewWave();
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
            _totalEnemiesRemaining++;
        }
    }

    #endregion

    #region Приватные методы

    private void InitializeWaveEnemyCounts()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            int waveTotal = waves[i].enemies.Sum(enemyInfo => enemyInfo.count);
            _enemiesRemainingInWave[i] = waveTotal;
        }
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);
        StartNextWave();
    }

    private void CleanupDestroyedEnemies()
    {
        _activeEnemies.RemoveAll(enemy => !enemy);
    }

    private void UpdateEnemyCounter()
    {
        if (_waveActive)
        {
            onEnemyCountChanged?.Invoke(_activeEnemies.Count, _totalEnemiesRemaining);
        }
    }

    private void CheckWaveCompletion()
    {
        if (_currentWaveIndex >= waves.Count && !loopWaves)
        {
            CompleteCurrentWave();
        }
    }
    
    /* private void CheckWaveCompletion()
    {
        if (_waveActive && !_isSpawning/* && _activeEnemies.Count == 0) 
            // убрать комментарий, если нужно завершать волну только при уничтожении всех врагов
        {
            if (_totalEnemiesRemaining <= 0)
            {
                CompleteCurrentWave();
            }
        }
        else if (_currentWaveIndex >= waves.Count)
        {
            CompleteCurrentWave();
        }
    } */
    

    private void ResetWaveState()
    {
        print("Restarting");
        StopAllCoroutines();
        _isSpawning = false;
        _waveActive = false;
        _currentWaveIndex = -1;
    }

    private void ResetEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _activeEnemies.Clear();
        _totalEnemiesSpawned = 0;
        _totalEnemiesRemaining = 0;
    }

    private void ResetEnemyCounts()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            int waveTotal = waves[i].enemies.Sum(enemyInfo => enemyInfo.count);
            _enemiesRemainingInWave[i] = waveTotal;
        }
    }

    private bool CheckWaveAvailability()
    {
        if (_currentWaveIndex >= waves.Count)
        {
            if (loopWaves)
            {
                _currentWaveIndex = 0;
            }
            else
            {
                onAllWavesComplete?.Invoke();
                return false;
            }
        }
        return true;
    }

    private void InitializeNewWave()
    {
        _waveActive = true;
        Wave currentWave = waves[_currentWaveIndex].DeepCopy();

        onWaveNumberChanged?.Invoke(_currentWaveIndex + 1);
        onWaveStart?.Invoke();

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }

        _spawnCoroutine = StartCoroutine(SpawnWave(currentWave));
    }

    private void OnGameStateChange(OnGameStateChangedEvent e)
    {
        if (e.State == GameLoopStateMachine.GameLoopState.Ascend)
        {
            StartCoroutine(StartWavesWithIncreasingDelays(5f, 2f)); // Первая задержка 5 секунд, каждая следующая увеличивается на 2 секунды
        }
        else if (e.State == GameLoopStateMachine.GameLoopState.Shopping)
        {
            StopAllWaves();
            DestroyEnemies();
        }
    }

    private void DestroyEnemies()
    {
        _activeEnemies.ForEach(Destroy);
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        _isSpawning = true;
        _totalEnemiesRemaining = _enemiesRemainingInWave[_currentWaveIndex];
        int enemiesLeftToSpawn = _totalEnemiesRemaining;

        while (enemiesLeftToSpawn > 0)
        {
            if (_activeEnemies.Count >= wave.maxEnemiesAtOnce)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            SpawnEnemy(wave);
            enemiesLeftToSpawn--;
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }
        
        _isSpawning = false;
    }

    private void SpawnEnemy(Wave wave)
    {
        if (spawnPoints.Count == 0) return;

        EnemySpawnInfo selectedEnemy = SelectEnemyToSpawn(wave);
        if (selectedEnemy.enemyPrefab == null) return;

        Transform spawnPoint = GetSuitableSpawnPoint();
        CreateEnemy(selectedEnemy, spawnPoint);
    }

    private EnemySpawnInfo SelectEnemyToSpawn(Wave wave)
    {
        var availableEnemies = wave.enemies.Where(e => e.count > 0).ToList();
        if (!availableEnemies.Any()) return new EnemySpawnInfo();

        float totalChance = availableEnemies.Sum(e => e.spawnChance);
        float randomValue = Random.Range(0, totalChance);
        float currentChance = 0;

        foreach (var enemy in availableEnemies)
        {
            currentChance += enemy.spawnChance;
            if (randomValue <= currentChance)
            {
                return enemy;
            }
        }

        return availableEnemies[0];
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

    private void CreateEnemy(EnemySpawnInfo enemyInfo, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        if (enemy.TryGetComponent<Enemy>(out var enemyComponent))
        {
            enemyComponent.Init(G.Player.transform);
        }

        _activeEnemies.Add(enemy);
        _totalEnemiesSpawned++;
    }

    private void CompleteCurrentWave()
    {
        _waveActive = false;
        onWaveComplete?.Invoke();

        if (_currentWaveIndex < waves.Count - 1 || loopWaves)
        {
            StartCoroutine(WaitForNextWave());
        }
        else
        {
            onAllWavesComplete?.Invoke();
        }
    }

    private IEnumerator WaitForNextWave()
    {
        float delay = waves[_currentWaveIndex].timeBeforeNextWave;
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    #endregion

    #region Методы для отладки

    public int GetCurrentWaveIndex() => _currentWaveIndex;
    public int GetTotalEnemiesSpawned() => _totalEnemiesSpawned;
    public int GetActiveEnemiesCount() => _activeEnemies.Count;

    #endregion
}