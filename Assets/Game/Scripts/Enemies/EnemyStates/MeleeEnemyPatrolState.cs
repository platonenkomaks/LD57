using UnityEngine;
public class MeleeEnemyPatrolState : EnemyState
{
    private readonly MeleeEnemy _meleeEnemy;
    
    public MeleeEnemyPatrolState(MeleeEnemy enemy) : base(enemy, EnemyStateID.Patrol)
    {
        this._meleeEnemy = enemy;
    }
    
    public override void Enter()
    {
        
    }
    
    public override void Update()
    {
        var distanceToPlayer = enemy.DistanceToPlayer();
        
        // Если игрок в зоне обнаружения, переходим к преследованию
        if (distanceToPlayer <= enemy.detectionRange && enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Chase);
        }
    }
    
    public override void FixedUpdate()
    {
        _meleeEnemy.Patrol();
    }
    
    public override void Exit()
    {
       
    }
}