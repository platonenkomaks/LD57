
using UnityEngine;
public class RangedEnemyAttackState : EnemyState
{
    private RangedEnemy rangedEnemy;
    private float stateTimer;
    
    public RangedEnemyAttackState(RangedEnemy enemy) : base(enemy, EnemyStateID.Attack)
    {
        this.rangedEnemy = enemy;
    }
    
    public override void Enter()
    {
        // Атакуем при входе в состояние
        rangedEnemy.Attack();
        stateTimer = enemy.attackCooldown;
    }
    
    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        
        // По окончании таймера проверяем дистанцию
        if (stateTimer <= 0)
        {
            float distanceToPlayer = enemy.DistanceToPlayer();
            
            // Если вышли из зоны атаки, переходим к преследованию
            if (distanceToPlayer > enemy.attackRange || !enemy.CanSeePlayer())
            {
                enemy.StateMachine.ChangeState(EnemyStateID.Chase);
            }
            else if (distanceToPlayer < enemy.retreatDistance)
            {
                // Если слишком близко, отступаем
                enemy.StateMachine.ChangeState(EnemyStateID.Retreat);
            }
            else
            {
                // Иначе продолжаем атаковать
                rangedEnemy.Attack();
                stateTimer = enemy.attackCooldown;
            }
        }
    }
}