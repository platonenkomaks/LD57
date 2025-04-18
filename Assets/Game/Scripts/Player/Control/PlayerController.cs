using System;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")] [SerializeField]
    private float moveSpeed = 8f;

    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControlFactor = 0.5f;

    [Header("Jump Parameters")] [SerializeField]
    private float jumpForce = 16f;

    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Combat Parameters")] [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField] private Transform firePoint;
    [SerializeField] private Sprite combatSprite;

    private Animator _playerAnimator;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;


    // Private variables
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter; 
    public bool isGrounded;
    private bool _isJumping;
    private bool _canShoot = false;
    private Vector2 _velocity;

    private float _lastShootTime;
    private RandomSoundPlayer _randomSoundPlayer;

    // Input reference
    private PlayerInput _playerInput;

    private void Awake()
    {
        G.PlayerController = this;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _randomSoundPlayer = GetComponent<RandomSoundPlayer>();
    }

    private void Start()
    {
        _playerAnimator = G.Player.GetComponent<Animator>();
    }

    private void Update()
    {
        // Поворот спрайта в зависимости от направления
        var horizontalInput = _playerInput.GetHorizontalInput();

        _spriteRenderer.flipX = horizontalInput switch
        {
            > 0 => false,
            < 0 => true,
            _ => _spriteRenderer.flipX
        };
        // Проверка соприкосновения с землей
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Обработка времени "койота"
        if (isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Обработка буфера прыжка
        if (_playerInput.IsJumpButtonPressed())
        {
            this.transform.SetParent(null);
            _jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        // Логика прыжка
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f && !_isJumping && this.transform.parent == null)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _isJumping = true;
            _jumpBufferCounter = 0f;
            G.AudioManager.Stop("PlayerJump");
            G.AudioManager.Play("PlayerJump");
        }

        // Контроль высоты прыжка в зависимости от удержания кнопки
        if (_playerInput.IsJumpButtonReleased() && _rb.linearVelocity.y > 0f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
        }

        if (_canShoot && _playerInput.IsFireButtonPressed() && isGrounded)
        {
            {
                
                G.AudioManager.Play("GunCLick");
                var cooldown = G.StatSystem.ShootgunCooldown;
                if (Time.time - _lastShootTime < cooldown) return;

                Shoot();
            }
        }

        // Обновление анимаций, если аниматор существует
        if (!_animator) return;
        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_rb.linearVelocity.x));
        _animator.SetBool("IsGrounded", isGrounded);
        _animator.SetFloat("VerticalVelocity", _rb.linearVelocity.y);
    }

    private void FixedUpdate()
        {
            // Расчет целевой скорости
            var horizontalInput = _playerInput.GetHorizontalInput();
            var targetSpeed = horizontalInput * moveSpeed;

            // Расчет скорости ускорения в зависимости от нахождения на земле или в воздухе
            var accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            if (!isGrounded)
            {
                accelRate *= airControlFactor;
            }

            // Расчет движения
            var speedDiff = targetSpeed - _rb.linearVelocity.x;
            var movement = speedDiff * accelRate * Time.fixedDeltaTime;

            // Применение движения
            _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);


            _rb.gravityScale = _rb.linearVelocity.y switch
            {
                // Улучшенная физика прыжка (более быстрое падение)
                < 0 => fallMultiplier,
                > 0 when !Input.GetButton("Jump") => lowJumpMultiplier,
                _ => 1f
            };
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Platform"))
            {
                transform.SetParent(collision.transform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Platform"))
            {
                transform.SetParent(null);
            }
        }

        public void EnableCombatMode()
        {
            _canShoot = true;
        }
        
        public void DisableCombatMode()
        {
            _canShoot = false;
        }

        private void Shoot()
        {
            G.AudioManager.Play("Shoot");
    
          

            Vector2 direction = Vector2.right * (_spriteRenderer.flipX ? -1 : 1);

            if (_playerInput.GetVerticalInput() > 0.5f)
            {
                _playerAnimator.SetTrigger("ShootUP");
                direction = Vector2.up;
                Debug.Log("Shooting UP");
            }
            else
            {
                _playerAnimator.SetTrigger("ShootSide");
                Debug.Log("Shooting SIDE");
            }

            var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    
            if (projectile != null)
            {
                
                if (projectile.GetComponent<Projectile>() != null)
                {
                    projectile.GetComponent<Projectile>().Initialize(direction);
                    
                }
            }
            

            _lastShootTime = Time.time;
            
            // Вызываем событие выстрела
            G.EventManager.Trigger(new OnPlayerShoot());
        }
        private void OnDrawGizmosSelected()
        {
            // Отрисовка радиуса проверки земли для отладки
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        public void Die()
        {
            _playerAnimator.SetTrigger("Dead");
        }
        
        public void onDeadAnimationEnd()
        {
            Debug.Log("Death animation ended");
           // Здесь можно добавить логику, которая будет выполняться после окончания анимации смерти
            // Например, перезапуск уровня или переход на экран Game Over
           // G.GameManager.RestartLevel();
           G.EventManager.Trigger(new OnPlayerDeath());
        }
    }