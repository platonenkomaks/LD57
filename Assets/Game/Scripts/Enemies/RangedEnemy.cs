
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Настройки рендж-врага")]
    [SerializeField] private float flyHeight = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float circlingStrafeSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;
    
    private Vector2 _wanderPoint;
    private float _wanderTimer;
    private const float WanderInterval = 2f;

    protected override void InitializeStateMachine()
    {
        StateMachine = new EnemyStateMachine();
        
        // Регистрируем состояния
        StateMachine.RegisterState(new RangedEnemyWanderState(this));
        StateMachine.RegisterState(new RangedEnemyChaseState(this));
        StateMachine.RegisterState(new RangedEnemyAttackState(this));
        StateMachine.RegisterState(new RangedEnemyTakeDamageState(this));
        StateMachine.RegisterState(new RangedEnemyRetreatState(this));
    }

    public override void Start()
    {
        base.Start();
        // Отличия для летающего врага
        rb.gravityScale = 0;
        
        // Начальная точка для блуждания
        SetNewWanderPoint();
    }

    public void Wander()
    {
        // Уменьшаем таймер блуждания
        _wanderTimer -= Time.deltaTime;
        
        if (_wanderTimer <= 0)
        {
            SetNewWanderPoint();
            _wanderTimer = WanderInterval;
        }
        
        // Лениво летим к точке блуждания
        Vector2 direction = (_wanderPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 0.5f;
        
        // Поворачиваем спрайт
        spriteRenderer.flipX = direction.x < 0;
        
        // Анимация полета
        if (animator != null)
        {
            animator.SetBool("IsFlying", true);
        }
    }
    
    private void SetNewWanderPoint()
    {
        // Создаем случайную точку блуждания вокруг начальной позиции
        var randomX = Random.Range(-patrolDistance, patrolDistance);
        var randomY = Random.Range(-patrolDistance * 0.5f, patrolDistance * 0.5f);
        
        _wanderPoint = new Vector2(
            transform.position.x + randomX,
            transform.position.y + randomY
        );
        
        // Убеждаемся, что мы не опустимся слишком низко
        if (_wanderPoint.y < flyHeight)
        {
            _wanderPoint.y = flyHeight;
        }
    }

    public void ChasePlayer()
    {
        if (player == null) return;
        
        // Сохраняем дистанцию для стрельбы
        float distance = DistanceToPlayer();
        Vector2 direction = DirectionToPlayer();
        
        // Если мы слишком близко к игроку, отходим
        if (distance < retreatDistance)
        {
            StateMachine.ChangeState(EnemyStateID.Retreat);
            return;
        }
        
        // Если мы в пределах дистанции атаки, то кружим вокруг игрока
        if (distance <= attackRange * 1.5f)
        {
            CircleAroundPlayer();
        }
        else
        {
            // Иначе приближаемся
            rb.linearVelocity = direction * moveSpeed;
        }
        
        // Поворачиваем спрайт
        spriteRenderer.flipX = direction.x < 0;
        
        // Анимация полета
        if (animator != null)
        {
            animator.SetBool("IsFlying", true);
        }
    }
    
    private void CircleAroundPlayer()
    {
        if (player == null) return;
        
        // Кружимся вокруг игрока
        Vector2 dirToPlayer = DirectionToPlayer();
        Vector2 perpendicular = new Vector2(-dirToPlayer.y, dirToPlayer.x); // Перпендикулярный вектор
        
        rb.linearVelocity = perpendicular * circlingStrafeSpeed;
    }

    public void Retreat()
    {
        if (player == null) return;
        
        // Отходим от игрока
        Vector2 direction = -DirectionToPlayer();
        rb.linearVelocity = direction * moveSpeed * 1.5f;
        
        // Поворачиваем спрайт
        spriteRenderer.flipX = direction.x < 0;
        
        // Если отошли достаточно далеко
        if (DistanceToPlayer() > attackRange * 2)
        {
            StateMachine.ChangeState(EnemyStateID.Chase);
        }
    }

    public void Attack()
    {
        if (!canAttack || player == null) return;
        
        // Останавливаемся
        rb.linearVelocity = Vector2.zero;
        
        // Анимация стрельбы
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Создаем и запускаем снаряд
        if (projectilePrefab != null && firePoint != null)
        {
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
        
        // Запускаем кулдаун
        StartCoroutine(AttackCooldown());
    }

    // Визуализация зон в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}