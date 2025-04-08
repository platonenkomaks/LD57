using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

// Базовый абстрактный класс врага
public abstract class Enemy : MonoBehaviour
{
    [Header("Базовые настройки")] [SerializeField]
    public float health = 100f;

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
        player = G.Player.transform;
        // Стартовое состояние (например, патруль)
        StateMachine.ChangeState(EnemyStateID.Patrol);
    }

    public virtual void Update()
    {
        if (player == null) return;

        // Обновляем состояние
        StateMachine.Update();
    }

    public virtual void FixedUpdate()
    {
        // Физические обновления (движение и т.д.)
        StateMachine.FixedUpdate();
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
        
        // Убираем коллайдеры
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
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
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
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
        if (StateMachine == null) return;

        // Define colors for each state
        Color stateColor = Color.white;
        switch (StateMachine.CurrentState.ID)
        {
            case EnemyStateID.Patrol:
                stateColor = Color.green;
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
        }

        // Draw a sphere above the enemy with the color of the current state
        Gizmos.color = stateColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.5f);
    }
}