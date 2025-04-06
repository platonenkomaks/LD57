using UnityEngine;

public class RangedEnemyRetreatState : EnemyState
{
    private readonly RangedEnemy _rangedEnemy;
    private const float RetreatTime = 1.5f;
    private float _timer;

    public RangedEnemyRetreatState(RangedEnemy enemy) : base(enemy, EnemyStateID.Retreat)
    {
        this._rangedEnemy = enemy;
    }

    public override void Enter()
    {
        _timer = RetreatTime;
    }

    public override void Update()
    {
        _timer -= Time.deltaTime;

        // По истечении таймера или при достаточном удалении переходим в погоню
        if (_timer <= 0 || enemy.DistanceToPlayer() > enemy.attackRange * 1.5f)
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Chase);
        }
    }

    public override void FixedUpdate()
    {
        _rangedEnemy.Retreat();
    }
}