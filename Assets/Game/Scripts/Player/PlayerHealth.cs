
using UnityEngine;
    public class PlayerHealth: MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private GameObject deathEffect;
        
        private void Start()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            // Проигрываем анимацию смерти
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
            
            // Уничтожаем игрока
            Destroy(gameObject);
        }
    }
  
