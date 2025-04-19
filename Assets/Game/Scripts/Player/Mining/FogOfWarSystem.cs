using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarSystem : MonoBehaviour
{
    #region Serialized Fields
    [Header("Fog Settings")]
    [SerializeField] private Tilemap fogTilemap;
    [SerializeField] private TileBase fogTile;
    [SerializeField] private float visibilityRange = 2f;
    [SerializeField] private int penetrationDepth = 1; // Глубина видимости сквозь блоки
    [SerializeField] private float tileFadeDuration = 1f;
    
    [Header("References")]
    [SerializeField] private SpriteRenderer alphaBlendTilePrefab;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private Tilemap goldTilemap;
    #endregion

    #region Private Fields
    private Transform _playerTransform;
    #endregion

    #region Unity Methods
    
    private void Awake()
    {
        G.FogOfWarSystem = this;
    }
    
    public void Init(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }
    
    private void LateUpdate()
    {
        if (_playerTransform == null) return;
        
        // Обновляем область видимости вокруг игрока каждый кадр
        Vector3Int playerCell = mainTilemap.WorldToCell(_playerTransform.position);
        RevealAreaAroundPosition(playerCell);
    }

    private void OnValidate()
    {
        // Убедимся, что глубина видимости не меньше 0
        penetrationDepth = Mathf.Max(0, penetrationDepth);
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
    
    private void RevealAreaAroundPosition(Vector3Int centerPosition)
    {
        // Открываем клетку, где находится игрок
        fogTilemap.SetTile(centerPosition, null);
        
        // Запускаем лучи во всех направлениях от игрока
        for (int angle = 0; angle < 360; angle += 5) // Шаг в 5 градусов для хорошего покрытия
        {
            CastVisibilityRay(centerPosition, angle);
        }
    }

    private void CastVisibilityRay(Vector3Int startPosition, float angleDegrees)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        
        Vector3 worldStart = mainTilemap.GetCellCenterWorld(startPosition);
        int blocksEncountered = 0;
        float currentDistance = 0f;
        float stepSize = 0.5f; // Размер шага для проверки
        
        while (currentDistance <= visibilityRange)
        {
            currentDistance += stepSize;
            
            // Вычисляем текущую позицию на луче
            Vector2 currentPos = (Vector2)worldStart + direction * currentDistance;
            Vector3Int cellPos = mainTilemap.WorldToCell(currentPos);
            
            // Tween the alpha of the tile before removing it
            if (fogTilemap.HasTile(cellPos))
            {
                FadeOutTile(cellPos);
                fogTilemap.SetTile(cellPos, null);
            }
            
            // Проверяем, есть ли здесь твердый блок
            bool isBlockingTile = (mainTilemap.HasTile(cellPos) || goldTilemap.HasTile(cellPos));
            
            if (isBlockingTile)
            {
                blocksEncountered++;
                
                // Если превысили глубину проникновения, прекращаем луч
                if (blocksEncountered > penetrationDepth)
                {
                    break;
                }
            }
        }
    }

    private void FadeOutTile(Vector3Int cellPos)
    {
        Vector3 worldPos = fogTilemap.GetCellCenterWorld(cellPos);
        SpriteRenderer spriteRenderer = Instantiate(alphaBlendTilePrefab, worldPos, Quaternion.identity, transform);
        spriteRenderer.sprite = fogTilemap.GetSprite(cellPos);
        spriteRenderer.sortingOrder = 10; // Ensure it renders above the tilemap
        
        // Tween the alpha to 0
        spriteRenderer.DOFade(0, tileFadeDuration).OnComplete(() =>
        {
            Destroy(spriteRenderer.gameObject);
        });
    }

    private void RevealAdjacentTiles(Vector3Int position)
    {
        // Обновляем видимость для всей области вокруг разрушенного блока
        for (int angle = 0; angle < 360; angle += 5)
        {
            CastVisibilityRay(position, angle);
        }
    }
    #endregion
}