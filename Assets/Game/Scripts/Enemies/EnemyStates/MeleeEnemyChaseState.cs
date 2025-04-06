
using UnityEngine;
public class MeleeEnemyChaseState : EnemyState
{
    private readonly MeleeEnemy _meleeEnemy;
    
    public MeleeEnemyChaseState(MeleeEnemy enemy) : base(enemy, EnemyStateID.Chase)
    {
        this._meleeEnemy = enemy;
    }
    
    public override void Enter()
    {
      
    }
    
    public override void Update()
    {
        var distanceToPlayer = enemy.DistanceToPlayer();
        
        if (distanceToPlayer > enemy.detectionRange * 1.5f || !enemy.CanSeePlayer())
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Patrol);
            return;
        }
        
        // Если игрок в зоне атаки, атакуем
        if (distanceToPlayer <= enemy.attackRange)
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Attack);
        }
    }
    
    public override void FixedUpdate()
    {
        _meleeEnemy.ChasePlayer();
    }
    
    public override void Exit()
    {
        var animator = enemy.GetComponent<Animator>();
            // animator.SetBool("IsRunning", false);
    }
}