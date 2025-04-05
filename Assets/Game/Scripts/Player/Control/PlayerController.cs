using UnityEngine;
[DefaultExecutionOrder(500)]
public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform weaponHolder;
    

    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 5f;
    
    
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;

    private Vector2 _movementInput;

    
 
    private void Awake()
    {
        G.PlayerController = this;
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        _movementInput = _playerInput.GetMovementInput();
        
        if (_playerInput.IsFireButtonPressed())
        {
            // Fire weapon
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rb.linearVelocity = _movementInput * moveSpeed;
    }

 
    
}