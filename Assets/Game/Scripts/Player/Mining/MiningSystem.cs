using UnityEngine;
using UnityEngine.Tilemaps;

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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        G.MiningSystem = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
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

        if (Input.GetMouseButtonDown(0))
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
    
        PlayMiningAnimation(mouseWorldPos);
        G.AudioManager.Play("Axe");
        _lastMiningTime = Time.time;
    
        var obstaclesCellPosition = removableTilemap.WorldToCell(mouseWorldPos);
        var goldCellPosition = goldTilemap.WorldToCell(mouseWorldPos);
    
        var goldTile = goldTilemap.GetTile(goldCellPosition);
        if (goldTile)
        {
            MineGold(goldCellPosition);
            return;
        }
    
        var obstacleTile = removableTilemap.GetTile(obstaclesCellPosition);
        if (!obstacleTile) return;
        
        MineObstacle(obstaclesCellPosition);
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
        
        G.AudioManager.Play("StoneCrack");
        _lastMiningTime = Time.time;
        StartGoldMining();
        
        Debug.Log("Золото добыто!");
    }
    
    private void StartGoldMining()
    {
        _isMiningGold = true;
        _canMine = false;
        _goldMiningEndTime = Time.time + goldMiningDuration;
    }
    
    private void FinishGoldMining()
    {
        _isMiningGold = false;
        _canMine = false;
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Carrying);
    }
    #endregion
}