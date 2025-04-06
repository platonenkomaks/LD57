using UnityEngine;


public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 15;
    [SerializeField] private GameObject impactEffect;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если попали в игрока, наносим урон
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        
        // Создаем эффект попадания
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        // Уничтожаем снаряд при столкновении
        Destroy(gameObject);
    }
}