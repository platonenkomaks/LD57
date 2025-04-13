using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event Action OnHealthChanged;

    [SerializeField] public int maxHealth = 5;
    private int _currentHealth;
    private bool _isInvincible = false;

    public int currentHealth
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = value;
            OnHealthChanged?.Invoke();
        }
    }

   
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        G.PlayerHealth = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = G.Player.GetComponent<SpriteRenderer>();
        }

        _isInvincible = true;

        float timer = 0f;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        spriteRenderer.enabled = true;
        _isInvincible = false;
    }

    private void Die()
    {
        G.PlayerController.Die();
    }
}