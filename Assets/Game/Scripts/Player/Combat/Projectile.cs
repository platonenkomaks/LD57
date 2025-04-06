using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;

    private Vector2 _direction;

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Тут можно обработать попадание
        Destroy(gameObject);
    }
}