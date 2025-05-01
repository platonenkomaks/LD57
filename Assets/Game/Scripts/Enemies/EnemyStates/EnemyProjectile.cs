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

        // If calculation fails, default to simple aim at player's current position
        bool useSimpleAiming = false;

        // Проверка на неподвижную цель
        if (playerVelocity.sqrMagnitude < 0.001f)
        {
            useSimpleAiming = true;
        }
        else
        {
            // Рассчитаем коэффициенты квадратного уравнения
            float a = Vector2.Dot(playerVelocity, playerVelocity) - (_speed * _speed);
            float b = 2f * Vector2.Dot(playerVelocity, directionToPlayer);
            float c = Vector2.Dot(directionToPlayer, directionToPlayer);

            // Объявляем переменную для времени перехвата
            float interceptTime;

            // Проверка ситуации, когда a близко к нулю (линейное уравнение)
            if (Mathf.Abs(a) < 0.0001f)
            {
                // Решаем линейное уравнение b*t + c = 0
                if (Mathf.Abs(b) < 0.0001f)
                {
                    // Если и b близко к нулю, то используем простое наведение
                    useSimpleAiming = true;
                }
                else
                {
                    interceptTime = -c / b;
                    if (interceptTime <= 0)
                    {
                        // Если время отрицательное, используем простое наведение
                        useSimpleAiming = true;
                    }
                    else
                    {
                        // Вычисляем точку перехвата
                        _interceptPoint = playerPosition + playerVelocity * interceptTime;
                        Vector2 projectileVelocity = ((Vector2)_interceptPoint - startPosition).normalized * _speed;
                        GetComponent<Rigidbody2D>().linearVelocity = projectileVelocity;
                    }
                }
            }
            else
            {
                float discriminant = b * b - 4f * a * c;
                if (discriminant < 0)
                {
                    // Нет решений, используем простое наведение
                    useSimpleAiming = true;
                }
                else
                {
                    float sqrtDiscriminant = Mathf.Sqrt(discriminant);
                    float timeOption1 = (-b - sqrtDiscriminant) / (2f * a);
                    float timeOption2 = (-b + sqrtDiscriminant) / (2f * a);

                    // Выбираем положительное время, если возможно
                    if (timeOption1 > 0 && timeOption2 > 0)
                    {
                        interceptTime = Mathf.Min(timeOption1, timeOption2); // Выбираем ближайшее время перехвата
                        
                        // Вычисляем точку перехвата и скорость снаряда
                        _interceptPoint = playerPosition + playerVelocity * interceptTime;
                        Vector2 projectileVelocity = ((Vector2)_interceptPoint - startPosition).normalized * _speed;
                        GetComponent<Rigidbody2D>().linearVelocity = projectileVelocity;
                    }
                    else if (timeOption1 > 0)
                    {
                        interceptTime = timeOption1;
                        _interceptPoint = playerPosition + playerVelocity * interceptTime;
                        Vector2 projectileVelocity = ((Vector2)_interceptPoint - startPosition).normalized * _speed;
                        GetComponent<Rigidbody2D>().linearVelocity = projectileVelocity;
                    }
                    else if (timeOption2 > 0)
                    {
                        interceptTime = timeOption2;
                        _interceptPoint = playerPosition + playerVelocity * interceptTime;
                        Vector2 projectileVelocity = ((Vector2)_interceptPoint - startPosition).normalized * _speed;
                        GetComponent<Rigidbody2D>().linearVelocity = projectileVelocity;
                    }
                    else
                    {
                        // Оба времени отрицательные, используем простое наведение
                        useSimpleAiming = true;
                    }
                }
            }
        }

        // Default to simple aiming if intercept calculation failed
        if (useSimpleAiming)
        {
            _interceptPoint = playerPosition;
            GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer.normalized * _speed;
        }

        Destroy(this.gameObject, 10f);
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
        else if (collision.CompareTag("Platform"))
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            G.AudioManager.Play("FireBallHit");
            _animator.SetTrigger("HitPlayer");
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