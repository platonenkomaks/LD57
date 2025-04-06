
using UnityEngine;
public class RangedEnemyChaseState : EnemyState
{
    private readonly RangedEnemy _rangedEnemy;
    
    public RangedEnemyChaseState(RangedEnemy enemy) : base(enemy, EnemyStateID.Chase)
    {
        this._rangedEnemy = enemy;
    }
    
    public override void Update()
    {
        var distanceToPlayer = enemy.DistanceToPlayer();
        
        // Если игрок вышел из зоны обнаружения, возвращаемся к блужданию
        if (distanceToPlayer > enemy.detectionRange * 1.5f || !enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Wander);
            return;
        }
        
        // Если игрок в зоне атаки, атакуем
        if (distanceToPlayer <= enemy.attackRange && enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Attack);
        }
    }
    
    public override void FixedUpdate()
    {
        _rangedEnemy.ChasePlayer();
    }
}