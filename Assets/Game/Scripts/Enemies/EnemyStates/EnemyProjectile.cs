using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private float _speed = 1f;

    public void Initialize(float speed)
    {
        _speed = speed;

        // Get player's current position and velocity
        Vector2 playerPos = G.Player.transform.position;
        Vector2 playerVelocity = G.Player.GetComponent<Rigidbody2D>().linearVelocity;

        // Calculate time it would take projectile to reach player's current position
        float distanceToPlayer = Vector2.Distance(transform.position, playerPos);
        float timeToReachPlayer = distanceToPlayer / _speed;

        // Project where player will be after that time
        Vector2 projectedPlayerPos = playerPos + (playerVelocity * timeToReachPlayer);

        // Calculate direction and set velocity
        Vector2 direction = (projectedPlayerPos - (Vector2)transform.position).normalized;
        GetComponent<Rigidbody2D>().linearVelocity = direction * _speed;


        // Уничтожаем снаряд через некоторое время
        Destroy(this.gameObject, 5f);
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