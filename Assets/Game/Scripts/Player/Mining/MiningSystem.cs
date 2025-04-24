using Events;
using GameControl;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MiningSystem : MonoBehaviour
{
    #region Serialized Fields
    [Header("Mining Settings")]
    [SerializeField] private float miningRange = 2f;
    [SerializeField] private float detectionRange = 50f;
    [SerializeField] private float miningCooldown = 0.5f;
    [SerializeField] private float goldMiningDuration = 1f;
    [SerializeField] private GameObject miningEffectPrefab;
    [SerializeField] private GameObject goldMiningEffectPrefab;

    [Header("Tilemaps")]
    public Tilemap removableTilemap;
    public Tilemap goldTilemap;

    [Header("Highlight Settings")] 
    public Tilemap highlightTilemap;
    public TileBase highlightTile;
    public Color highlightNormalColor = new Color(1f, 1f, 1f, 0.5f);
    public Color highlightGoldColor = new Color(1f, 0.84f, 0f, 0.5f);
    #endregion

    #region Private Fields
    private bool _canMine = true;
    private bool _isMiningGold = false;
    private float _lastMiningTime;
    private float _goldMiningEndTime;
    private Camera _mainCamera;
    private Vector3Int _currentHighlightPosition = Vector3Int.one * int.MinValue;
    private Vector3Int _lastMinedPosition = Vector3Int.one * int.MinValue;
    private bool _isMouseHeld = false;
    
    // Checkpoint system
    private TilemapSnapshot _goldTilemapSnapshot;
    private TilemapSnapshot _removableTilemapSnapshot;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        G.MiningSystem = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _goldTilemapSnapshot = new TilemapSnapshot(goldTilemap);
        _removableTilemapSnapshot = new TilemapSnapshot(removableTilemap);
        G.EventManager.Register<OnCheckpoint>(OnCheckpoint);
        G.EventManager.Register<OnPlayerRespawn>(OnPlayerRespawn);
    }
    
    private void OnDestroy()
    {
        G.EventManager.Unregister<OnCheckpoint>(OnCheckpoint);
        G.EventManager.Unregister<OnPlayerRespawn>(OnPlayerRespawn);
        G.MiningSystem = null;
    }

    private void Update()
    {
        HandleGoldMiningTimer();
        if (G.PlayerStateMachine == null) return;
        
        HandleTileHighlight();
        HandleMiningInput();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion

    #region Public Methods
    public void EnableMining()
    {
        _canMine = true;
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
    }
    #endregion

    #region Private Methods
    private void HandleGoldMiningTimer()
    {
        if (_isMiningGold && Time.time >= _goldMiningEndTime)
        {
            FinishGoldMining();
        }
    }

    private void HandleTileHighlight()
    {
        if (G.PlayerStateMachine.CurrentState == PlayerStateMachine.PlayerState.Mining)
        {
            UpdateTileHighlight();
        }
        else
        {
            ClearHighlight();
        }
    }

    private void HandleMiningInput()
    {
        if (!_canMine || _isMiningGold) return;

        // Отслеживаем нажатие и отпускание кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseHeld = true;
            TryMine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isMouseHeld = false;
            _lastMinedPosition = Vector3Int.one * int.MinValue; // Сбрасываем последнюю позицию при отпускании кнопки
        }
        // Если кнопка мыши удерживается и прошло время перезарядки, пробуем добывать
        else if (_isMouseHeld && Time.time - _lastMiningTime >= miningCooldown)
        {
            TryMine();
        }
    }

    private int CountGold()
    {
        int count = 0;
        foreach (var pos in goldTilemap.cellBounds.allPositionsWithin)
        {
            if (goldTilemap.HasTile(pos))
            {
                count++;
            }
        }
        return count;
    }
    
    private void UpdateTileHighlight()
    {
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        if (Vector2.Distance(mouseWorldPos, transform.position) > detectionRange)
        {
            ClearHighlight();
            return;
        }
        
        Vector3Int cellPosition = removableTilemap.WorldToCell(mouseWorldPos);
        
        if (cellPosition == _currentHighlightPosition) return;
            
        ClearHighlight();
        
        bool hasGold = goldTilemap.HasTile(cellPosition);
        bool hasObstacle = removableTilemap.HasTile(cellPosition);
        
        if (hasGold || hasObstacle)
        {
            G.AudioManager.Play("Interact");
            highlightTilemap.SetTile(cellPosition, highlightTile);
            highlightTilemap.SetColor(cellPosition, hasGold ? highlightGoldColor : highlightNormalColor);
            _currentHighlightPosition = cellPosition;
        }
    }
    
    private void ClearHighlight()
    {
        if (_currentHighlightPosition != Vector3Int.one * int.MinValue)
        {
            highlightTilemap.SetTile(_currentHighlightPosition, null);
            _currentHighlightPosition = Vector3Int.one * int.MinValue;
        }
    }
    
    private void TryMine()
    {
        if (Time.time - _lastMiningTime < miningCooldown) return;
    
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        if (Vector2.Distance(mouseWorldPos, transform.position) > detectionRange) return;
    
        var obstaclesCellPosition = removableTilemap.WorldToCell(mouseWorldPos);
        var goldCellPosition = goldTilemap.WorldToCell(mouseWorldPos);
        
        // Проверяем, не добываем ли мы снова тот же блок (актуально для удержания кнопки)
        if (obstaclesCellPosition == _lastMinedPosition || goldCellPosition == _lastMinedPosition)
        {
            return;
        }
    
        var goldTile = goldTilemap.GetTile(goldCellPosition);
        var obstacleTile = removableTilemap.GetTile(obstaclesCellPosition);
        
        // Если нет ни золота, ни препятствия - не проигрываем анимацию
        if (!goldTile && !obstacleTile) return;
        
        // Проигрываем анимацию добычи
        PlayMiningAnimation(mouseWorldPos);
        G.AudioManager.Play("Axe");
        _lastMiningTime = Time.time;
    
        if (goldTile)
        {
            if (!G.BackPack.IsFull())
            {
                MineGold(goldCellPosition);
                _lastMinedPosition = goldCellPosition;
                return;
            }
            else
            {
                // Если рюкзак полон, то трясем рюкзак
                StartCoroutine(ShakeBackpack());
            }
        }
    
        if (obstacleTile)
        {
            MineObstacle(obstaclesCellPosition);
            _lastMinedPosition = obstaclesCellPosition;
        }
    }

    private void OnCheckpoint(OnCheckpoint e)
    {
        _goldTilemapSnapshot = new TilemapSnapshot(goldTilemap);
        _removableTilemapSnapshot = new TilemapSnapshot(removableTilemap);
    }
    
    private void OnPlayerRespawn(OnPlayerRespawn e)
    {
        _goldTilemapSnapshot.ApplyTo(goldTilemap);
        _removableTilemapSnapshot.ApplyTo(removableTilemap);
    }

    private void PlayMiningAnimation(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        Animator animator = GetComponent<Animator>();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        if (angle < 0) angle += 360;
        
        if (angle >= 45 && angle < 135)
        {
            animator.SetTrigger("StrikeUp");
        }
        else if (angle >= 225 && angle < 315)
        {
            animator.SetTrigger("StrikeDown");
        }
        else
        {
            animator.SetTrigger("StrikeSide");
        }
    }
    
    private void MineObstacle(Vector3Int cellPosition)
    {
        removableTilemap.SetTile(cellPosition, null);
        highlightTilemap.SetTile(cellPosition, null);
    
        if (_currentHighlightPosition == cellPosition)
        {
            _currentHighlightPosition = Vector3Int.one * int.MinValue;
        }
    
        if (miningEffectPrefab)
        {
            var effectPosition = removableTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(miningEffectPrefab, effectPosition, Quaternion.identity);
        }
    
        // Обновляем туман войны после удаления блока
        if (G.FogOfWarSystem != null)
        {
            G.FogOfWarSystem.OnBlockMined(cellPosition);
        }
    
        G.AudioManager.Play("StoneCrack");
        _lastMiningTime = Time.time;
    }


    private void MineGold(Vector3Int cellPosition)
    {
        goldTilemap.SetTile(cellPosition, null);
        highlightTilemap.SetTile(cellPosition, null);
    
        if (_currentHighlightPosition == cellPosition)
        {
            _currentHighlightPosition = Vector3Int.one * int.MinValue;
        }
    
        if (goldMiningEffectPrefab != null)
        {
            Vector3 effectPosition = goldTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(goldMiningEffectPrefab, effectPosition, Quaternion.identity);
        }
    
        // Обновляем туман войны после добычи золота
        if (G.FogOfWarSystem != null)
        {
            G.FogOfWarSystem.OnBlockMined(cellPosition);
        }
    
        G.AudioManager.Play("StoneCrack");
        _lastMiningTime = Time.time;
        StartGoldMining();
    }
    private void StartGoldMining()
    {
        _isMiningGold = true;
        _goldMiningEndTime = Time.time + goldMiningDuration;
    }
    
    private void FinishGoldMining()
    {
        _isMiningGold = false;
        G.BackPack.AddGold(1);
    }
    
    
    private static IEnumerator ShakeBackpack()
    {
        if (G.BackPack == null || G.BackPack.gameObject == null) yield break;
    
        var backpackObject = G.BackPack.gameObject;
        var originalPosition = backpackObject.transform.localPosition;
    
        const float duration = 1f;
        const float magnitude = 0.15f;
        float elapsed = 0;
    
        while (elapsed < duration)
        {
            var x = originalPosition.x + Random.Range(-10f, 10f) * magnitude;
            var y = originalPosition.y + Random.Range(-10f, 10f) * magnitude;
        
            backpackObject.transform.localPosition = new Vector3(x, y, originalPosition.z);
        
            elapsed += Time.deltaTime;
        
            yield return null;
        }
        
        backpackObject.transform.localPosition = originalPosition;
    }
    #endregion
}