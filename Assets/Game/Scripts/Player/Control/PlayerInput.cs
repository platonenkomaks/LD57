
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _movementInput;
    private Vector2 _aimDirection;
    private bool _fireButtonPressed;
    private bool _settingsButtonPressed;
    
    public Camera MainCamera { get; private set; }


    
    private void Awake()
    {
        G.PlayerInput = this;
        MainCamera = Camera.main;
    }

    private void Update()
    {
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        var mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);

        _aimDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y)
            .normalized;

        // Получение ввода для стрельбы
        _fireButtonPressed = Input.GetMouseButton(0);
        _settingsButtonPressed = Input.GetKeyDown(KeyCode.P);
    }

    public Vector2 GetMovementInput()
    {
        return _movementInput;
    }

    public Vector2 GetAimDirection()
    {
        return _aimDirection;
    }

    public bool IsFireButtonPressed()
    {
        return _fireButtonPressed;
    }
    
    public bool IsSettingsButtonPressed()
    {
        return _settingsButtonPressed;
    }
}