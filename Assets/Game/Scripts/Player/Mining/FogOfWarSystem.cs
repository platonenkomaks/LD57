using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarSystem : MonoBehaviour
{
    #region Serialized Fields
    [Header("Fog Settings")]
    [SerializeField] private Tilemap fogTilemap;
    [SerializeField] private TileBase fogTile;
    [SerializeField] private float visibilityRange = 2f;
    [SerializeField] private Color fogColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    
    [Header("References")]
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private Tilemap goldTilemap;
    #endregion

    #region Private Fields
    private Transform _playerTransform;
    private bool _initialized = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        G.FogOfWarSystem = this;
    }

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeFogOfWar();
    }

    private void LateUpdate()
    {
        if (_playerTransform == null) return;
        
        // Обновляем область видимости вокруг игрока каждый кадр
        Vector3Int playerCell = mainTilemap.WorldToCell(_playerTransform.position);
        RevealAreaAroundPosition(playerCell);
    }
    #endregion

    #region Public Methods
    // Публичный метод, который будет вызываться из MiningSystem
    public void OnBlockMined(Vector3Int position)
    {
        RevealAdjacentTiles(position);
    }
    #endregion

    #region Private Methods
    private void InitializeFogOfWar()
    {
        if (_initialized) return;
        
        // Покрываем туманом всю область, основываясь на размерах основного тайлмапа
        BoundsInt bounds = mainTilemap.cellBounds;
        
        for (int x = bounds.min.x - 10; x < bounds.max.x + 10; x++)
        {
            for (int y = bounds.min.y - 10; y < bounds.max.y + 10; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                fogTilemap.SetTile(cellPosition, fogTile);
                fogTilemap.SetColor(cellPosition, fogColor);
            }
        }

        // Открываем начальную область вокруг игрока
        if (_playerTransform != null)
        {
            Vector3Int playerCell = mainTilemap.WorldToCell(_playerTransform.position);
            RevealAreaAroundPosition(playerCell);
        }
        
        _initialized = true;
    }

    private void RevealAreaAroundPosition(Vector3Int centerPosition)
    {
        // Открываем клетку, где находится игрок
        fogTilemap.SetTile(centerPosition, null);
        
        // Открываем клетки в радиусе видимости
        for (int x = -Mathf.RoundToInt(visibilityRange); x <= Mathf.RoundToInt(visibilityRange); x++)
        {
            for (int y = -Mathf.RoundToInt(visibilityRange); y <= Mathf.RoundToInt(visibilityRange); y++)
            {
                if (x * x + y * y <= visibilityRange * visibilityRange)
                {
                    Vector3Int offsetPosition = new Vector3Int(x, y, 0);
                    Vector3Int tilePosition = centerPosition + offsetPosition;
                    
                    // Проверяем, видим ли этот тайл или он скрыт другими
                    if (IsTileVisible(centerPosition, tilePosition))
                    {
                        fogTilemap.SetTile(tilePosition, null);
                    }
                }
            }
        }
    }

    private void RevealAdjacentTiles(Vector3Int position)
    {
        // Открываем соседние клетки
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),   // верх
            new Vector3Int(0, -1, 0),  // низ
            new Vector3Int(-1, 0, 0),  // лево
            new Vector3Int(1, 0, 0),   // право
            new Vector3Int(-1, 1, 0),  // верхний левый угол
            new Vector3Int(1, 1, 0),   // верхний правый угол
            new Vector3Int(-1, -1, 0), // нижний левый угол
            new Vector3Int(1, -1, 0)   // нижний правый угол
        };

        foreach (Vector3Int offset in neighbors)
        {
            Vector3Int neighborPosition = position + offset;
            fogTilemap.SetTile(neighborPosition, null);
        }
    }

    // Проверяет, видна ли клетка из указанной позиции
    private bool IsTileVisible(Vector3Int fromPosition, Vector3Int toPosition)
    {
        // Простая реализация проверки видимости по линии
        // Можно улучшить использованием алгоритма Брезенхема для линии
        
        Vector3 from = mainTilemap.GetCellCenterWorld(fromPosition);
        Vector3 to = mainTilemap.GetCellCenterWorld(toPosition);
        Vector3 direction = to - from;
        float distance = Vector3.Distance(from, to);
        
        // Проверяем, есть ли на пути твердые блоки
        RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance);
        if (hit.collider != null)
        {
            // Если что-то блокирует путь
            return false;
        }
        
        return true;
    }
    #endregion
}