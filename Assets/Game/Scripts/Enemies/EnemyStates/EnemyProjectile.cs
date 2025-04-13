using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    
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