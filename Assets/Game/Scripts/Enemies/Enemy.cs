using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

// Базовый абстрактный класс врага
public abstract class Enemy : MonoBehaviour
{
    [Header("Базовые настройки")] [SerializeField]
    public float maxHealth = 100f;
    [SerializeField] public float health = 100f;

    [SerializeField] public float moveSpeed = 2f;
    [SerializeField] public float detectionRange = 5f;
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] public float attackCooldown = 1.5f;
    [SerializeField] public float retreatDistance = 3f;
    [SerializeField] public Transform player;

    public EnemyStateMachine StateMachine;
    protected  Rigidbody2D _rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public bool canAttack = true;

    public void Init(Transform targetPlayer)
    {
        player = targetPlayer.transform;
    }


    public virtual void Awake()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Инициализация машины состояний
        InitializeStateMachine();
    }


    public virtual void Start()
    {
        if (G.Player == null)
        {
            Debug.LogError("G.Player is null in Enemy.Start()");
            return;
        }
        
        player = G.Player.transform;
        Debug.Log($"Enemy {gameObject.name} initialized with player at position: {player.position}");
        
        // Стартовое состояние (например, патруль)
        if (StateMachine == null)
        {
            Debug.LogError($"StateMachine is null in Enemy {gameObject.name}");
            return;
        }
        
        StateMachine.ChangeState(EnemyStateID.Patrol);
    }

    public virtual void Update()
    {
        // Обновляем состояние
        if (StateMachine != null)
        {
            StateMachine.Update();
        }
    }

    public virtual void FixedUpdate()
    {
        // Физические обновления (движение и т.д.)
        if (StateMachine != null)
        {
            StateMachine.FixedUpdate();
        }
    }

    protected abstract void InitializeStateMachine();

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        // Если здоровье кончилось, умираем
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Иначе переходим в состояние получения урона
            StateMachine.ChangeState(EnemyStateID.TakeDamage);
        }
    }

    public virtual void Die()
    {
        // Проигрываем анимацию смерти
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }
    
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    

    public virtual IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public float DistanceToPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning($"DistanceToPlayer: player is null in {gameObject.name}");
            return Mathf.Infinity;
        }
        
        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log($"DistanceToPlayer: distance={distance} in {gameObject.name}");
        return distance;
    }

    public Vector2 DirectionToPlayer()
    {
        if (player == null) return Vector2.zero;
        return (player.position - transform.position).normalized;
    }

    // Функция для проверки зрения (line of sight)
    public bool CanSeePlayer()
    {
        if (player == null) return false;
        
        // Проверяем, находится ли игрок в зоне обнаружения
        return DistanceToPlayer() <= detectionRange;
    }
    
    private void OnDrawGizmos()
    {
        if (StateMachine == null || StateMachine.CurrentState == null) return;

        // Define colors for each state
        Color stateColor = Color.white;
        switch (StateMachine.CurrentState.ID)
        {
            case EnemyStateID.Patrol:
                stateColor = Color.green;
                break;
            case EnemyStateID.Wander:
                stateColor = Color.cyan;
                break;
            case EnemyStateID.Chase:
                stateColor = Color.yellow;
                break;
            case EnemyStateID.Attack:
                stateColor = Color.red;
                break;
            case EnemyStateID.TakeDamage:
                stateColor = Color.blue;
                break;
            case EnemyStateID.Retreat:
                stateColor = Color.magenta;
                break;
        }

        // Draw a sphere above the enemy with the color of the current state
        Gizmos.color = stateColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.5f);
    }
}