using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerControls _playerControls;
    public Rigidbody _rb;

    public float _maxSpeed, _acceleration, _deacceleration;
    private float _currentSpeed;

    public WeaponController _weaponController;

    private InputAction _move;
    private InputAction _look;
    private InputAction _attack;

    private Vector3 _movementInput;
    private Vector3 _pointerPosition;
    

    //float rotationFactorPerFrame = 15.0f;

    [SerializeField] private LayerMask _groundMask;

    Camera _cam;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }
    // Start is called before the first frame update
    private void Start()
    {
        _cam = Camera.main;
        if(!_rb)
            TryGetComponent(out _rb);
        TryGetComponent(out _weaponController);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        Aim();
        Attack();
    }

    // Conseguimos la posicion del raton en el plano
    private (bool success, Vector3 position) GetMousePosition()
    {
        _pointerPosition = _look.ReadValue<Vector2>();

        var ray = _cam.ScreenPointToRay(_pointerPosition);
        
        if(Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    // Funcion para que el jugador apunte al raton
    private void Aim()
    {
        if (!MouseOnScreen())
            return;

        var (success, position) = GetMousePosition();

        if(success)
        {
            var direction = (position - transform.position).normalized;
            direction.y = 0.0f;
            transform.forward = direction;
        }
    }

    private bool MouseOnScreen()
    {
        var view = _look.ReadValue<Vector2>();

        return !(view.x > Screen.width || view.x < 0 || view.y < 0 || view.y > Screen.height);

        //var view = _cam.ScreenToViewportPoint(_look.ReadValue<Vector2>());

        //return view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
    }
    
    private void Attack()
    {
        if (_attack.IsPressed())
        {
            _weaponController.Attack();
        }
    }
    private void HandleMovement()
    {
        _movementInput = _move.ReadValue<Vector2>();
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y); // Transformar el 2D a 3D

        if(_currentSpeed >= 0)
        {
            _currentSpeed += _acceleration *_maxSpeed * Time.deltaTime;
        }
        else
        {
            _currentSpeed -= _deacceleration * _maxSpeed * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);
        _rb.velocity = movement * _currentSpeed;
    }

    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _look = _playerControls.Player.Look;
        _attack = _playerControls.Player.Fire;

        _move.Enable();
        _look.Enable();
        _attack.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _attack.Disable();
    }

}
