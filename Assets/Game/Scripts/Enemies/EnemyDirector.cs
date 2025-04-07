using System.Collections;
using System.Collections.Generic;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDirector : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
        public float timeBetweenSpawns = 1.5f;
        public float timeBeforeNextWave = 5f;
        public int maxEnemiesAtOnce = 5;
        public bool isBossWave = false;
    }

    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnChance = 1f; // Вероятность спавна от 0 до 1
    }

    [Header("Настройки спавна")] [SerializeField]
    private List<Wave> waves = new List<Wave>();

    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private bool autoStartWaves = true;
    [SerializeField] private bool loopWaves = false;
    [SerializeField] private float initialDelay = 3f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnCheckRadius = 1f;
    

    [Header("События")] public UnityEvent onWaveStart;
    public UnityEvent onWaveComplete;
    public UnityEvent onAllWavesComplete;
    public UnityEvent<int> onWaveNumberChanged;
    public UnityEvent<int, int> onEnemyCountChanged; // текущее количество, максимальное количество

    private int _currentWaveIndex = -1;
    private int _totalEnemiesSpawned = 0;
    private int _totalEnemiesRemaining = 0;
    private readonly List<GameObject> _activeEnemies = new List<GameObject>();
    private bool _isSpawning = false;
    private Coroutine _spawnCoroutine;
    private bool _waveActive = false;
    private readonly Dictionary<int, int> _enemiesRemainingInWave = new();


    private IEnumerator Start()
    {
        yield return null;
        G.EventManager.Register<OnGameStateChangedEvent>(OnGameStateChange);
        // Заполняем словарь для отслеживания оставшихся врагов в каждой волне
        for (int i = 0; i < waves.Count; i++)
        {
            int waveTotal = 0;
            foreach (var enemyInfo in waves[i].enemies)
            {
                waveTotal += enemyInfo.count;
            }

            _enemiesRemainingInWave[i] = waveTotal;
        }

        if (autoStartWaves)
        {
            StartCoroutine(DelayedStart());
        }
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnGameStateChangedEvent>(OnGameStateChange);
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);
        StartNextWave();
    }

    private void Update()
    {
        // Очищаем список от уничтоженных врагов
        _activeEnemies.RemoveAll(enemy => !enemy);

        // Обновляем счетчик
        if (_waveActive)
        {
            onEnemyCountChanged?.Invoke(_activeEnemies.Count, _totalEnemiesRemaining);
        }

        // Проверяем, завершена ли текущая волна
        if (_waveActive && !_isSpawning && _activeEnemies.Count == 0)
        {
            CompleteCurrentWave();
        }
    }

    // Публичный метод для запуска волн
    public void StartWaves()
    {
        if (_currentWaveIndex < 0)
        {
            StartNextWave();
        }
    }

    public void StopAllWaves()
    {
        StopAllCoroutines();
    }

    // Перезапуск волн с самого начала
    public void RestartWaves()
    {
        StopAllCoroutines();
        _isSpawning = false;
        _waveActive = false;
        _currentWaveIndex = -1;

        // Уничтожаем всех активных врагов
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        _activeEnemies.Clear();

        // Сбрасываем счетчики
        _totalEnemiesSpawned = 0;
        _totalEnemiesRemaining = 0;

        // Рестарт словаря оставшихся врагов
        for (int i = 0; i < waves.Count; i++)
        {
            int waveTotal = 0;
            foreach (var enemyInfo in waves[i].enemies)
            {
                waveTotal += enemyInfo.count;
            }

            _enemiesRemainingInWave[i] = waveTotal;
        }

        StartCoroutine(DelayedStart());
    }

    // Запуск следующей волны
    public void StartNextWave()
    {
        _currentWaveIndex++;

        // Проверяем, есть ли еще волны
        if (_currentWaveIndex >= waves.Count)
        {
            if (loopWaves)
            {
                _currentWaveIndex = 0;
            }
            else
            {
                Debug.Log("Все волны завершены!");
                onAllWavesComplete?.Invoke();
                return;
            }
        }

        _waveActive = true;
        Wave currentWave = waves[_currentWaveIndex];
        Debug.Log($"Старт волны: {currentWave.waveName}");

        onWaveNumberChanged?.Invoke(_currentWaveIndex + 1);
        onWaveStart?.Invoke();

        // Запускаем спавн врагов
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
            RestartWaves();
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

    // Корутина для спавна врагов в волне
    private IEnumerator SpawnWave(Wave wave)
    {
        _isSpawning = true;
        _totalEnemiesRemaining = _enemiesRemainingInWave[_currentWaveIndex];
        int enemiesLeftToSpawn = _totalEnemiesRemaining;

        while (enemiesLeftToSpawn > 0)
        {
            // Если достигнут лимит одновременных врагов, ждем
            if (_activeEnemies.Count >= wave.maxEnemiesAtOnce)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Спавним одного врага из доступных типов
            SpawnEnemy(wave);
            enemiesLeftToSpawn--;

            // Ждем перед следующим спавном
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        _isSpawning = false;
    }

    private void SpawnEnemy(Wave wave)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Нет точек спавна!");
            return;
        }

        // Выбираем тип врага на основе вероятностей
        List<EnemySpawnInfo> availableEnemies = new List<EnemySpawnInfo>();
        foreach (var enemyInfo in wave.enemies)
        {
            if (enemyInfo.count > 0)
            {
                availableEnemies.Add(enemyInfo);
            }
        }

        if (availableEnemies.Count == 0)
        {
            return;
        }

        // Выбираем врага с учетом вероятности
        float totalChance = 0;
        foreach (var enemyInfo in availableEnemies)
        {
            totalChance += enemyInfo.spawnChance;
        }

        float randomValue = Random.Range(0, totalChance);
        float currentChance = 0;
        EnemySpawnInfo selectedEnemy = availableEnemies[0];

        for (int i = 0; i < availableEnemies.Count; i++)
        {
            currentChance += availableEnemies[i].spawnChance;
            if (randomValue <= currentChance)
            {
                selectedEnemy = availableEnemies[i];
                break;
            }
        }

        // Уменьшаем счетчик этого типа врагов
        for (int i = 0; i < wave.enemies.Count; i++)
        {
            if (wave.enemies[i].enemyPrefab == selectedEnemy.enemyPrefab)
            {
                wave.enemies[i].count--;
                break;
            }
        }

        // Выбираем точку спавна
        Transform spawnPoint = GetSuitableSpawnPoint();

        // Создаем врага
        GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().Init(enemy.transform); // Инициализация врага (если требуется)
        _activeEnemies.Add(enemy);
        _totalEnemiesSpawned++;

        // Настраиваем компонент врага
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.Init(G.Player.transform);
        }
    }

    // Поиск подходящей точки спавна
    private Transform GetSuitableSpawnPoint()
    {
        List<Transform> suitablePoints = new List<Transform>();

        foreach (var point in spawnPoints)
        {
            // Проверяем, нет ли препятствий
            bool hasObstacle = Physics2D.OverlapCircle(point.position, spawnCheckRadius, obstacleLayer);
            

            if (!hasObstacle)
            {
                suitablePoints.Add(point);
            }
        }

        // Если нет подходящих точек, используем любую
        if (suitablePoints.Count == 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        return suitablePoints[Random.Range(0, suitablePoints.Count)];
    }

    // Завершение текущей волны
    private void CompleteCurrentWave()
    {
        _waveActive = false;
        Debug.Log($"Волна {_currentWaveIndex + 1} завершена!");
        onWaveComplete?.Invoke();

        // Задержка перед следующей волной
        if (_currentWaveIndex < waves.Count - 1 || loopWaves)
        {
            StartCoroutine(WaitForNextWave());
        }
        else
        {
            Debug.Log("Все волны завершены!");
            onAllWavesComplete?.Invoke();
        }
    }

    // Задержка между волнами
    private IEnumerator WaitForNextWave()
    {
        float delay = waves[_currentWaveIndex].timeBeforeNextWave;
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    // Добавление врага в учет (для врагов, созданных другими способами)
    public void RegisterEnemy(GameObject enemy)
    {
        if (!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
            _totalEnemiesRemaining++;
        }
    }

    // Методы для редактора и отладки
    public int GetCurrentWaveIndex()
    {
        return _currentWaveIndex;
    }

    public int GetTotalEnemiesSpawned()
    {
        return _totalEnemiesSpawned;
    }

    public int GetActiveEnemiesCount()
    {
        return _activeEnemies.Count;
    }
    
}