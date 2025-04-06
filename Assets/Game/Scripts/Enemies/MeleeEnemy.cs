using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Настройки милиш врага")]
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    
    
    
    private Vector2 _initialPosition;
    private Vector2 _patrolDirection = Vector2.right;
    private bool _isGrounded;

    public override void Awake()
    {
        base.Awake();
        _initialPosition = transform.position;
    }

    protected override void InitializeStateMachine()
    {
        StateMachine = new EnemyStateMachine();
        
        // Регистрируем состояния
        StateMachine.RegisterState(new MeleeEnemyPatrolState(this));
        StateMachine.RegisterState(new MeleeEnemyChaseState(this));
        StateMachine.RegisterState(new MeleeEnemyAttackState(this));
        StateMachine.RegisterState(new MeleeEnemyTakeDamageState(this));
    }

    public override void Update()
    {
        base.Update();
        
        // Проверяем есть ли земля под врагом
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        
        _isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        
        // Визуальное отображение луча проверки земли
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);
    }

    public bool IsGrounded() => _isGrounded;

    public void Patrol()
    {
        // Проверка на край платформы или препятствие
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, _patrolDirection, 0.5f, groundLayer);
        RaycastHit2D edgeCheck = Physics2D.Raycast(
            new Vector2(transform.position.x + _patrolDirection.x * 0.5f, transform.position.y - 0.5f),
            Vector2.down, 1f, groundLayer);
        
        // Если впереди стена или край платформы, то меняем направление
        if (wallCheck.collider != null || edgeCheck.collider == null || 
            Vector2.Distance(_initialPosition, transform.position) >= patrolDistance)
        {
            _patrolDirection *= -1;
            
            // Поворачиваем спрайт
            spriteRenderer.flipX = _patrolDirection.x < 0;
        }
        
        // Двигаемся в текущем направлении
        rb.linearVelocity = new Vector2(_patrolDirection.x * moveSpeed, rb.linearVelocity.y);
        
        // Анимация ходьбы
        if (animator != null)
        {
            //animator.SetBool("IsWalking", true);
        }
    }

    public void ChasePlayer()
    {
        if (player == null) return;
        
        // Определяем направление к игроку
        Vector2 direction = DirectionToPlayer();
        direction.y = 0; // Ограничиваем движение по Y (для хождения по платформам)
        
        // Поворачиваем спрайт
        spriteRenderer.flipX = direction.x < 0;
        
        // Двигаемся к игроку
        rb.linearVelocity = new Vector2(direction.x * moveSpeed * 1.5f, rb.linearVelocity.y);
        
        // Анимация бега
        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }

    public void Attack()
    {
        if (!canAttack) return;
        
        // Останавливаемся
        rb.linearVelocity = Vector2.zero;
        
        // Анимация атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Наносим урон игроку
        // Используем OverlapCircle для определения попадания
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            // Получаем компонент здоровья игрока и наносим урон
           // PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
           // if (playerHealth != null)
          //  {
           //    playerHealth.TakeDamage(attackDamage);
          //  }
        }
        
        // Запускаем кулдаун
        StartCoroutine(AttackCooldown());
    }

    // Визуализация зоны атаки в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}