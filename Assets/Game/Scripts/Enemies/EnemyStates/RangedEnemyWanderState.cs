
using UnityEngine;
using System.Collections.Generic;
public class RangedEnemyWanderState : EnemyState
{
    private readonly RangedEnemy _rangedEnemy;
    
    public RangedEnemyWanderState(RangedEnemy enemy) : base(enemy, EnemyStateID.Wander)
    {
        this._rangedEnemy = enemy;
    }
    
    public override void Update()
    {
        // Проверяем дистанцию до игрока
        float distanceToPlayer = enemy.DistanceToPlayer();
        
        // Если игрок в зоне обнаружения, переходим к преследованию
        if (distanceToPlayer <= enemy.detectionRange && enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Chase);
        }
    }
    
    public override void FixedUpdate()
    {
        // Блуждаем
        _rangedEnemy.Wander();
    }
}