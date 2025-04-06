using UnityEngine;
public class MeleeEnemyTakeDamageState : EnemyState
{
    private float stunTime = 0.5f;
    private float timer;
    
    public MeleeEnemyTakeDamageState(Enemy enemy) : base(enemy, EnemyStateID.TakeDamage)
    {
    }
    
    public override void Enter()
    {
        // Анимация получения урона
        var animator = enemy.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        // Останавливаем движение
        var rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Устанавливаем таймер оглушения
        timer = stunTime;
    }
    
    public override void Update()
    {
        timer -= Time.deltaTime;
        
        // По истечении таймера продолжаем преследование
        if (timer <= 0)
        {
            enemy.GetComponent<EnemyStateMachine>().ChangeState(EnemyStateID.Chase);
        }
    }
}