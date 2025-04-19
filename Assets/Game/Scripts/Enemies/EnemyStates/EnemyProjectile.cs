using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private float _speed = 1f;
    private Vector3 _interceptPoint;

    public void Initialize(float speed)
    {
        _speed = speed;

        // Get player's current position and velocity
        Vector2 P = G.Player.transform.position;
        Vector2 V = G.ElevatorPlatform.CurrentSpeed * Vector2.up + G.Player.GetComponent<Rigidbody2D>().linearVelocity;

        // Starting projectile position and vector to player
        Vector2 S = transform.position;
        Vector2 D = (P - S);

        // Equation: || P + V * t - S || = _speed * t
        //   The left part is the distance the projectile will travel based on vectors
        //   The right part is the distance the projectile will travel based on time

        // Equation is equivalent to: || D + V * t || = _speed * t
        // Expanding to: || D ||^2 + 2 * D * V * t + || V ||^2 * t^2 = _speed^2 * t^2\
        // Or: (Vector2.Dot(V, V) - _speed^2)* t^2 + 2 * Vector2.Dot(V, D) * t + Vector2.Dot(D, D) = 0
        // Solve quadratic equation for t
        float a = Vector2.Dot(V, V) - _speed * _speed;
        float b = 2 * Vector2.Dot(V, D);
        float c = Vector2.Dot(D, D);

        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            Destroy(this.gameObject);
            return;
        }

        float t1 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);

        float t = Mathf.Max(t1, t2);
        _interceptPoint = P + V * t;
        Vector2 velocity = (D + V * t).normalized * _speed;
        GetComponent<Rigidbody2D>().linearVelocity = velocity;

        // Уничтожаем снаряд через некоторое время
        Destroy(this.gameObject, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_interceptPoint, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если попали в игрока, наносим урон
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = G.PlayerHealth;
            if (playerHealth != null)
            {
                G.AudioManager.Play("FireBallHit");
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
            }

        }
        
        Destroy(gameObject, 2f);
    }
}