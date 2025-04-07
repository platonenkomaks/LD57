using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private LayerMask collisionLayers; // Добавьте слои, с которыми пуля должна взаимодействовать

    private Vector2 _direction;
    private Vector2 _lastPosition;

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
        _lastPosition = transform.position;
        Destroy(gameObject, lifetime);
    }

    private void Start()
    {
        Debug.Log("Пуля создана в позиции " + transform.position);
    }

    private void Update()
    {
        // Сохраняем текущую позицию перед перемещением
        _lastPosition = transform.position;
        
        // Перемещаем пулю
        transform.Translate(_direction * speed * Time.deltaTime);
        
        // Выполняем Raycast между предыдущей и текущей позицией
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(_lastPosition, currentPosition);
        
        RaycastHit2D hit = Physics2D.Raycast(_lastPosition, _direction, distance, collisionLayers);
        Debug.DrawLine(_lastPosition, currentPosition, Color.red, 0.5f);
        
        if (hit.collider != null)
        {
            Debug.Log("Raycast попал в: " + hit.collider.gameObject.name + " с тегом: " + hit.collider.tag);
            HandleCollision(hit.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D вызван с: " + other.gameObject.name);
        HandleCollision(other);
    }
    
    private void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Обнаружен тег Enemy!");
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Найден компонент Enemy, применяем урон");
                enemy.TakeDamage(1);
                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.LogWarning("Объект с тегом Enemy не имеет компонента Enemy");
            }
        }
        
        if (!other.CompareTag("Player")) // Игнорируем столкновения с игроком
        {
            Destroy(gameObject);
        }
    }
}