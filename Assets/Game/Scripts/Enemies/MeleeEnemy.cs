using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy
{
    [Header("Настройки милиш врага")] [SerializeField]
    private float patrolDistance = 3f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.5f;

    
    private Vector2 _initialPosition;
    private Vector2 _patrolDirection = Vector2.right;
    private bool _isGrounded;
    private bool _playingRiseAnimation = false; // Флаг для отслеживания анимации приземления

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

        bool wasGroundedBefore = _isGrounded; // Сохраняем предыдущее состояние
        _isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // Visual representation of the ground check ray
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, _isGrounded ? Color.green : Color.red);

        // Управление анимациями
        HandleAnimations(wasGroundedBefore);
    }

    private void HandleAnimations(bool wasGroundedBefore)
    {
        // Если персонаж только что приземлился
        if (!wasGroundedBefore && _isGrounded)
        {
            StartCoroutine(PlayRiseAnimation());
        }
        // Если персонаж в воздухе, запускаем анимацию падения
        else if (!_isGrounded)
        {
            animator.SetBool("isGrounded", false);
            animator.SetTrigger("fall");
        }
        // Если персонаж на земле и не играет анимацию приземления, то должен быть в Idle
        else if (_isGrounded && !_playingRiseAnimation)
        {
            animator.SetBool("isGrounded", true);
            animator.SetTrigger("idle");
        }
    }

    private IEnumerator PlayRiseAnimation()
    {
        _playingRiseAnimation = true;
        animator.SetBool("isGrounded", true);
        animator.SetTrigger("rise");

        // Получаем длительность анимации rise
        float riseAnimationLength = 0;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains("rise"))
            {
                riseAnimationLength = clip.length;
                break;
            }
        }

        // Ждем окончания анимации
        yield return new WaitForSeconds(riseAnimationLength);

        // По окончании переходим в idle
        _playingRiseAnimation = false;
        animator.SetTrigger("idle");
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
        _rb.linearVelocity = new Vector2(_patrolDirection.x * moveSpeed, _rb.linearVelocity.y);
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
        _rb.linearVelocity = new Vector2(direction.x * moveSpeed * 1.5f, _rb.linearVelocity.y);
    }

    public void Attack()
    {
        if (!canAttack) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            Debug.Log("Attack hit player!");

            PlayerHealth playerHealth = G.PlayerHealth;
            if (playerHealth != null)
            {
                StartCoroutine(AttackAnimation(hitPlayer.transform.position, playerHealth));
            }
        }
        else
        {
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackAnimation(Vector3 playerPosition, PlayerHealth playerHealth)
    {
        float originalPosition = transform.position.x;
        float attackSpeed = 10f;
        float attackPosition = Mathf.Lerp(originalPosition, playerPosition.x, 0.8f);


        float startTime = Time.time;
        float journeyLength = Mathf.Abs(originalPosition - attackPosition);
        float distanceCovered = 0f;

        while (distanceCovered < journeyLength)
        {
            float fractionOfJourney = distanceCovered / journeyLength;
            float x = Mathf.Lerp(originalPosition, attackPosition, fractionOfJourney);
            _rb.MovePosition(new Vector3(x, transform.position.y, transform.position.z));

            distanceCovered = (Time.time - startTime) * attackSpeed;
            yield return null;
        }


        playerHealth.TakeDamage(attackDamage);
        G.AudioManager.Play("SlimeAttack");


        yield return new WaitForSeconds(0.1f);
        startTime = Time.time;
        journeyLength = Mathf.Abs(attackPosition - originalPosition);
        distanceCovered = 0f;

        while (distanceCovered < journeyLength)
        {
            float fractionOfJourney = distanceCovered / journeyLength;
            float x = Mathf.Lerp( attackPosition,originalPosition, fractionOfJourney);
            _rb.MovePosition(new Vector3(x, transform.position.y, transform.position.z));
            distanceCovered = (Time.time - startTime) * attackSpeed;
            yield return null;
        }
        

        // Запускаем кулдаун после завершения анимации
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