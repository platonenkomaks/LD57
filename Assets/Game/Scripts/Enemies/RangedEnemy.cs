using UnityEngine;
using System.Collections.Generic;

public class RangedEnemy : Enemy
{
    [Header("Настройки рендж-врага")]
    [SerializeField] private float flyHeight = 3f;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float attackInterval = 1.5f;
    
    private float _attackTimer;
    private bool _isInitialized = false;
    
    // Статический список для хранения всех созданных пуль
    private static List<EnemyProjectile> _activeProjectiles = new List<EnemyProjectile>();

    protected override void InitializeStateMachine()
    {
        // Пустая реализация, так как мы не используем машину состояний
    }

    public override void Start()
    {
        if (G.Player == null)
        {
            Debug.LogError("G.Player is null in RangedEnemy.Start()");
            return;
        }
        
        player = G.Player.transform;
        
        // Отключаем гравитацию для летающего врага
        _rb.gravityScale = 0;
        
        // Инициализируем аниматор
        animator.Rebind();
        // animator.SetBool("IsFlying", true);
        
        _isInitialized = true;
    }

    public override void Update()
    {
        if (!_isInitialized || player == null) return;
        
        // Проверка на получение урона
        if (health < maxHealth)
        {
            // Можно добавить анимацию получения урона
            return;
        }
        
        // Движемся к игроку, сохраняя фиксированную высоту
        Vector2 direction = DirectionToPlayer();
        float targetHeight = player.position.y + flyHeight;
        float currentHeight = transform.position.y;
        
        // Корректируем высоту полета
        if (Mathf.Abs(currentHeight - targetHeight) > 0.1f)
        {
            Vector2 verticalCorrection = new Vector2(0, (targetHeight - currentHeight) * 2f);
            _rb.linearVelocity = verticalCorrection;
        }
        else
        {
            // Движемся горизонтально к игроку
            direction.y = 0; // Игнорируем вертикальное движение к игроку
            _rb.linearVelocity = direction * moveSpeed;
        }
        
        // Атакуем игрока
        
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            Attack();
            _attackTimer = attackInterval;
        }
        
        // Поворачиваемся к игроку
        spriteRenderer.flipX = direction.x < 0;
        
        // Очистка списка от уничтоженных пуль
        CleanupDestroyedProjectiles();
    }

    public void Attack()
    {
        if (player == null) return;
        _rb.linearVelocity = Vector2.zero;
        
        // Анимация стрельбы
        
        G.AudioManager.Play("FireBall");
        EnemyProjectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Initialize(projectileSpeed);
        
        // Добавляем пулю в список активных
        _activeProjectiles.Add(projectile);
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        
        animator.SetTrigger("Hurt");
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    // Удаляет уничтоженные пули из списка
    private void CleanupDestroyedProjectiles()
    {
        _activeProjectiles.RemoveAll(p => p == null);
    }
    
    // Метод для получения всех активных пуль
    public static List<EnemyProjectile> GetAllProjectiles()
    {
        return new List<EnemyProjectile>(_activeProjectiles);
    }
    
    // Метод для удаления всех активных пуль
    public void DestroyAllProjectiles()
    {
        foreach (var projectile in _activeProjectiles)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }
        
        _activeProjectiles.Clear();
    }
    
    // Метод для удаления конкретной пули
    public static void DestroyProjectile(EnemyProjectile projectile)
    {
        if (_activeProjectiles.Contains(projectile))
        {
            _activeProjectiles.Remove(projectile);
            Destroy(projectile.gameObject);
        }
    }
    
    // Необходимо вызвать этот метод, когда пуля самостоятельно уничтожается
    public static void RemoveProjectileFromList(EnemyProjectile projectile)
    {
        if (_activeProjectiles.Contains(projectile))
        {
            _activeProjectiles.Remove(projectile);
        }
    }
    
    private void OnDestroy()
    {
         DestroyAllProjectiles();
    }
}