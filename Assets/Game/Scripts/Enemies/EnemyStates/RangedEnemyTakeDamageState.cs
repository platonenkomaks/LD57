using UnityEngine;
public class RangedEnemyTakeDamageState : EnemyState
{
    private const float StunTime = 0.3f; // Меньше, так как летающий враг более мобильный
    private float _timer;
    
    public RangedEnemyTakeDamageState(Enemy enemy) : base(enemy, EnemyStateID.TakeDamage)
    {
    }
    
    public override void Enter()
    {
        // Анимация получения урона
        var animator = enemy.GetComponent<Animator>();
        if (animator)
        {
           // animator.SetTrigger("Hurt");
        }
        
        // Кратковременное оглушение
        _timer = StunTime;
    }
    
    public override void Update()
    {
        _timer -= Time.deltaTime;
        
        // По истечении таймера переходим к отступлению
        if (_timer <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyStateID.Retreat);
        }
    }
}