using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MiningSystem : MonoBehaviour
{
    [Header("Mining Settings")]
    [SerializeField] private float miningRange = 2f; // Радиус копания
    [SerializeField] private float detectionRange = 50f; // Увеличенный радиус детекции мышью
    [SerializeField] private float miningCooldown = 0.5f; // Задержка между копаниями
    [SerializeField] private float goldMiningDuration = 1f; // Время копания золота
    [SerializeField] private GameObject miningEffectPrefab; // Эффект копания препятствий
    [SerializeField] private GameObject goldMiningEffectPrefab; // Эффект копания золота

    public Tilemap removableTilemap; // Разрушаемый слой
    public Tilemap goldTilemap; // Слой с золотом


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
        int goldCount = CountGold();
        G.GoldManager.SetGoldRemaining(goldCount);
    }

    private void Update()
    {
        // Проверяем, закончилось ли время копания золота
        if (_isMiningGold && Time.time >= _goldMiningEndTime)
        {
            FinishGoldMining();
        }

        if (G.PlayerStateMachine == null)
            return;
        if (G.PlayerStateMachine.CurrentState != PlayerStateMachine.PlayerState.Mining)
            return;

        // Если игрок копает золото, то не может копать препятствия
        if (!_canMine || _isMiningGold)
            return;

        // Проверяем, нажата ли левая кнопка мыши
        if (Input.GetMouseButtonDown(0))
        {
            TryMine();
        }
    }

    public void EnableMining()
    {
        _canMine = true;
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
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
    
    private void TryMine()
    {
        // Проверка кулдауна
        if (Time.time - _lastMiningTime < miningCooldown) return;
    
        // Получаем позицию мыши в мировых координатах
        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        // Проверяем, находится ли позиция в пределах радиуса ДЕТЕКЦИИ (увеличенный радиус)
        if (Vector2.Distance(mouseWorldPos, transform.position) > detectionRange) return;
    
        // Определяем направление атаки и воспроизводим соответствующую анимацию
        PlayMiningAnimation(mouseWorldPos);
    
        G.AudioManager.Play("Axe");
    
        // Обновляем время последнего копания
        _lastMiningTime = Time.time;
    
        // Получаем позицию клетки тайлмапа
        var obstaclesCellPosition = removableTilemap.WorldToCell(mouseWorldPos);
        var goldCellPosition = goldTilemap.WorldToCell(mouseWorldPos);
    
        // Сначала проверяем наличие золота
        var goldTile = goldTilemap.GetTile(goldCellPosition);
        if (goldTile)
        {
            MineGold(goldCellPosition);
            return;
        }
    
        // Если золота нет, проверяем наличие препятствия
        var obstacleTile = removableTilemap.GetTile(obstaclesCellPosition);
        if (!obstacleTile) return;
        MineObstacle(obstaclesCellPosition);
    }

private void PlayMiningAnimation(Vector2 targetPosition)
{
    // Вычисляем направление от игрока к позиции клика мыши
    Vector2 direction = targetPosition - (Vector2)transform.position;
    
    // Предполагаем, что у вас есть аниматор
    Animator animator = GetComponent<Animator>();
    
    // Определяем, в каком направлении нужно копать
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
    // Нормализуем угол в диапазоне от -180 до 180 градусов
    if (angle < 0) angle += 360;
    
    // Воспроизводим соответствующую анимацию на основе угла
    if (angle >= 45 && angle < 135)
    {
        // Верхнее направление (вверх)
        animator.SetTrigger("StrikeUp");
    }
    else if (angle >= 225 && angle < 315)
    {
        // Нижнее направление (вниз)
        animator.SetTrigger("StrikeDown");
    }
    else
    {
        // Боковые направления (влево или вправо)
        animator.SetTrigger("StrikeSide");
        
        // Если нужно различать правое и левое направления для отражения спрайта
        bool isFacingRight = direction.x > 0;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }
}
    
    private void MineObstacle(Vector3Int cellPosition)
    {
        
        // Удаляем тайл с карты препятствий
        removableTilemap.SetTile(cellPosition, null);
        
        // Создаем эффект копания
        if (miningEffectPrefab)
        {
            var effectPosition = removableTilemap.GetCellCenterWorld(cellPosition);
            Instantiate(miningEffectPrefab, effectPosition, Quaternion.identity);
        }
        // Проигрываем звук копания препятствия
        G.AudioManager.Play("Impact");
         
        // Устанавливаем время последнего копания
        _lastMiningTime = Time.time;
        
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
        
        // Проигрываем звук копания золота
        G.AudioManager.Play("Impact");
        
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
        // Устанавливаем время окончания копания золота
        _goldMiningEndTime = Time.time + goldMiningDuration;
       
    }
    
    private void FinishGoldMining()
    {
        _isMiningGold = false;
        _canMine = false;
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Carrying);
    }
    
    
    // Для отображения радиуса копания в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}