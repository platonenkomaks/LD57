
using UnityEngine;

public class MeleeEnemyAttackState : EnemyState
{
    private MeleeEnemy meleeEnemy;
    private float stateTimer;
    
    public MeleeEnemyAttackState(MeleeEnemy enemy) : base(enemy, EnemyStateID.Attack)
    {
        this.meleeEnemy = enemy;
    }
    
    public override void Enter()
    {
        meleeEnemy.Attack();
        stateTimer = enemy.attackCooldown;
    }
    
    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        
        // По окончании таймера проверяем дистанцию
        if (!(stateTimer <= 0)) return;
        var distanceToPlayer = enemy.DistanceToPlayer();
            
        // Если вышли из зоны атаки, переходим к преследованию
        if (distanceToPlayer > enemy.attackRange)
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Chase);
        }
        else
        {
            // Иначе продолжаем атаковать
            meleeEnemy.Attack();
            stateTimer = enemy.attackCooldown;
        }
    }
}