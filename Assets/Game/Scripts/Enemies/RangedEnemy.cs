using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Настройки рендж-врага")]
    [SerializeField] private float flyHeight = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float attackInterval = 1.5f;
    
    private float _attackTimer;
    private bool _isInitialized = false;

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
        Debug.Log($"RangedEnemy {gameObject.name} initialized with player at position: {player.position}");
        
        // Отключаем гравитацию для летающего врага
        _rb.gravityScale = 0;
        
        // Инициализируем аниматор
        if (animator != null)
        {
            animator.Rebind();
            animator.SetBool("IsFlying", true);
        }
        
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
    }

    public void Attack()
    {
        if (player == null) return;
        
        // Останавливаемся
        _rb.linearVelocity = Vector2.zero;
        
        // Анимация стрельбы
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Создаем и запускаем снаряд
        if (projectilePrefab != null && firePoint != null)
        {
            G.AudioManager.Play("FireBall");
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            
            if (projectileRb != null)
            {
                Vector2 direction = DirectionToPlayer();
                projectileRb.linearVelocity = direction * projectileSpeed;
                
                // Уничтожаем снаряд через некоторое время
                Destroy(projectile, 5f);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        
        // Анимация получения урона
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        // Если здоровье кончилось, умираем
        if (health <= 0)
        {
            Die();
        }
    }

    // Визуализация зон в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}