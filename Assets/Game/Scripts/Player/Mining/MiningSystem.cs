using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
public class MiningSystem : MonoBehaviour
{
  
    public Tilemap removableTilemap; // Разрушаемый слой
    public  Tilemap goldTilemap; // Слой с золотом
    
    [Header("Player")]
    [SerializeField] private float miningRange = 2f; // Радиус копания
    [SerializeField] private SpriteRenderer playerSpriteRenderer; // Компонент спрайта игрока
    [SerializeField] private Sprite normalSprite; // Обычный спрайт гнома
    [SerializeField] private Sprite miningGoldSprite; // Спрайт гнома, когда он копает золото
    
    [Header("Mining Settings")]
    [SerializeField] private float miningCooldown = 0.5f; // Задержка между копаниями
    [SerializeField] private float goldMiningDuration = 1f; // Время копания золота
    [SerializeField] private GameObject miningEffectPrefab; // Эффект копания препятствий
    [SerializeField] private GameObject goldMiningEffectPrefab; // Эффект копания золота
    
    
    private bool _canMine = true; // Может ли игрок копать
    private bool _isMiningGold = false; // Копает ли игрок золото
    private float _lastMiningTime; // Время последнего копания
    private float _goldMiningEndTime; // Время, когда закончится копание золота
    private Camera _mainCamera;

    private void Awake()
    {
        G.MiningSystem = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    
        
        // Проверяем наличие компонентов
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
            Debug.LogWarning("PlayerSpriteRenderer не задан, используется компонент на текущем объекте");
        }
        
        // Устанавливаем начальный спрайт
        if (playerSpriteRenderer != null && normalSprite != null)
        {
            playerSpriteRenderer.sprite = normalSprite;
        }
    }
    
    private void Update()
    {
        // Проверяем, закончилось ли время копания золота
        if (_isMiningGold && Time.time >= _goldMiningEndTime)
        {
            FinishGoldMining();
        }
        
        // Если игрок копает золото, то не может копать препятствия
        if (!_canMine || _isMiningGold)
            return;
        
        // Проверяем, нажата ли левая кнопка мыши
        if (Input.GetMouseButtonDown(0))
        {
            TryMine();
        }
    }
    
    private void TryMine()
    {
        // Проверка кулдауна
        if (Time.time - _lastMiningTime < miningCooldown)
            return;
        
        // Получаем позицию мыши в мировых координатах
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Проверяем, находится ли позиция в пределах радиуса копания
        if (Vector2.Distance(mouseWorldPos, transform.position) > miningRange)
        {
            Debug.Log("Слишком далеко для копания!");
            return;
        }
        
        // Получаем позицию клетки тайлмапа
        Vector3Int obstaclesCellPosition = removableTilemap.WorldToCell(mouseWorldPos);
        Vector3Int goldCellPosition = goldTilemap.WorldToCell(mouseWorldPos);
        
        // Сначала проверяем наличие золота
        TileBase goldTile = goldTilemap.GetTile(goldCellPosition);
        if (goldTile != null)
        {
            MineGold(goldCellPosition);
            return;
        }
        
        // Если золота нет, проверяем наличие препятствия
        TileBase obstacleTile = removableTilemap.GetTile(obstaclesCellPosition);
        if (obstacleTile != null)
        {
            MineObstacle(obstaclesCellPosition);
            return;
        }
    }
    
    private void MineObstacle(Vector3Int cellPosition)
    {
        // Удаляем тайл с карты препятствий
        removableTilemap.SetTile(cellPosition, null);
        
        // Создаем эффект копания
        if (miningEffectPrefab != null)
        {
            Vector3 effectPosition = removableTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(miningEffectPrefab, effectPosition, Quaternion.identity);
        }
         
        // Устанавливаем время последнего копания
        _lastMiningTime = Time.time;
        
        Debug.Log("Препятствие уничтожено!");
    }
    
    private void MineGold(Vector3Int cellPosition)
    {
        // Удаляем тайл с карты золота
        goldTilemap.SetTile(cellPosition, null);
        
        // Создаем эффект копания
        if (goldMiningEffectPrefab != null)
        {
            Vector3 effectPosition = goldTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(goldMiningEffectPrefab, effectPosition, Quaternion.identity);
        }
        
        // Проигрываем звук
       
        
        // Устанавливаем время последнего копания
        _lastMiningTime = Time.time;
        
        // Игрок переходит в состояние копания золота
        StartGoldMining();
        
        
        Debug.Log("Золото добыто!");
    }
    
    private void StartGoldMining()
    {
        _isMiningGold = true;
        _canMine = false;
        
        // Меняем спрайт гнома
        if (playerSpriteRenderer != null && miningGoldSprite != null)
        {
            playerSpriteRenderer.sprite = miningGoldSprite;
        }
        
        // Устанавливаем время окончания копания золота
        _goldMiningEndTime = Time.time + goldMiningDuration;
        
        Debug.Log($"Игрок начал копать золото, будет занят {goldMiningDuration} секунд");
    }
    
    private void FinishGoldMining()
    {
        _isMiningGold = false;
        _canMine = true;
        
        // Меняем спрайт гнома обратно
        if (playerSpriteRenderer != null && normalSprite != null)
        {
            playerSpriteRenderer.sprite = normalSprite;
        }
        
        Debug.Log("Игрок закончил копать золото и может копать снова");
    }
    
   
    
    // Для отображения радиуса копания в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}