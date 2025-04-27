using System;
using DG.Tweening;
using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private float _speed = 1f;
    private Vector3 _interceptPoint;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(float speed)
    {
        _speed = speed;

        // Get player's current position and velocity
        Vector2 playerPosition = G.Player.transform.position;
        Vector2 playerVelocity = G.ElevatorPlatform.CurrentSpeed * Vector2.up + G.Player.GetComponent<Rigidbody2D>().linearVelocity;

        // Starting projectile position and vector to player
        Vector2 startPosition = transform.position;
        Vector2 directionToPlayer = (playerPosition - startPosition);

        // Проверка на неподвижную цель
        if (playerVelocity.sqrMagnitude < 0.001f)
        {
            // Для неподвижной цели просто направляем пулю прямо в текущее положение игрока
            _interceptPoint = playerPosition;
            GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer.normalized * _speed;
            Destroy(this.gameObject, 10f);
            return;
        }

        // Рассчитаем коэффициенты квадратного уравнения
        float a = Vector2.Dot(playerVelocity, playerVelocity) - _speed * _speed;
        float b = 2 * Vector2.Dot(playerVelocity, directionToPlayer);
        float c = Vector2.Dot(directionToPlayer, directionToPlayer);

        // Объявляем переменную для времени перехвата
        float interceptTime;

        // Проверка ситуации, когда a близко к нулю (линейное уравнение)
        if (Mathf.Abs(a) < 0.0001f)
        {
            // Решаем линейное уравнение b*t + c = 0
            if (Mathf.Abs(b) < 0.0001f)
            {
                // Если и b близко к нулю, то решений нет или бесконечно много
                Destroy(this.gameObject);
                return;
            }
            
            interceptTime = -c / b;
            if (interceptTime <= 0)
            {
                // Если время отрицательное, значит перехват невозможен
                Destroy(this.gameObject);
                return;
            }
        }
        else
        {
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // Нет решений, перехват невозможен
                Destroy(this.gameObject);
                return;
            }

            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            float timeOption1 = (-b - sqrtDiscriminant) / (2 * a);
            float timeOption2 = (-b + sqrtDiscriminant) / (2 * a);

            // Выбираем положительное время, если возможно
            if (timeOption1 > 0 && timeOption2 > 0)
            {
                interceptTime = Mathf.Min(timeOption1, timeOption2); // Выбираем ближайшее время перехвата
            }
            else if (timeOption1 > 0)
            {
                interceptTime = timeOption1;
            }
            else if (timeOption2 > 0)
            {
                interceptTime = timeOption2;
            }
            else
            {
                // Оба времени отрицательные, перехват невозможен
                Destroy(this.gameObject);
                return;
            }
        }

        // Вычисляем точку перехвата и скорость снаряда
        _interceptPoint = playerPosition + playerVelocity * interceptTime;
        Vector2 projectileVelocity = (directionToPlayer + playerVelocity * interceptTime).normalized * _speed;
        GetComponent<Rigidbody2D>().linearVelocity = projectileVelocity;

        Destroy(this.gameObject, 10f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_interceptPoint, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = G.PlayerHealth;
            if (playerHealth != null)
            {
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                G.AudioManager.Play("FireBallHit");
                playerHealth.TakeDamage(damage);
                _animator.SetTrigger("HitPlayer");
            }
        }
        else
        {
            Destroy();
        }
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        RangedEnemy.RemoveProjectileFromList(this);
    }
}