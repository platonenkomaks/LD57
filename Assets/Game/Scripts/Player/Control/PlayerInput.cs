
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    
        private Vector2 movementInput;
        private Vector2 aimDirection;
        private bool fireButtonPressed;
        private bool settingsButtonPressed;
        private bool jumpButtonPressed;
        private bool jumpButtonReleased;
    
        public Camera MainCamera { get; private set; }
    
        private void Start()
        {
            G.PlayerInput = this;
            MainCamera = Camera.main;
        }

        private void Update()
        {
            // Получение ввода для движения
            movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            // Получение направления прицеливания
            var mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y)
                .normalized;

            // Получение ввода для стрельбы и настроек
            fireButtonPressed = Input.GetMouseButton(0);
            settingsButtonPressed = Input.GetKeyDown(KeyCode.P);
        
            // Получение ввода для прыжка
            jumpButtonPressed = Input.GetButtonDown("Jump");
            jumpButtonReleased = Input.GetButtonUp("Jump");
        }

        public Vector2 GetMovementInput()
        {
            return movementInput;
        }

        public Vector2 GetAimDirection()
        {
            return aimDirection;
        }

        public bool IsFireButtonPressed()
        {
            return fireButtonPressed;
        }
    
        public bool IsSettingsButtonPressed()
        {
            return settingsButtonPressed;
        }
    
        public bool IsJumpButtonPressed()
        {
            return jumpButtonPressed;
        }
    
        public bool IsJumpButtonReleased()
        {
            return jumpButtonReleased;
        }
    
        public float GetHorizontalInput()
        {
            return movementInput.x;
        }
    }
