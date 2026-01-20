using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float _speed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _rotateRate = 0.5f;
    [SerializeField] private GameObject _interactText;
    [SerializeField] private Transform _model;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 5;
    [SerializeField] private PlayerPunch _playerPunch;

    public IInteractable InteractingObject { get; set; }
    
    private CharacterController _controller;
    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _dashAction;
    private InputAction _punchAction;
    
    private bool _isDead = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Interact");
        _dashAction = InputSystem.actions.FindAction("Dash");
        _punchAction = InputSystem.actions.FindAction("Punch");

        _interactAction.performed += _ => InteractingObject?.Interact();
        _punchAction.performed += _ => _animator.SetTrigger("Punch");
    }

    private void Update()
    {
        _interactText.SetActive(InteractingObject != null);
        
        if (_playerPunch.IsBeingHit || _isDead) return;
        
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        Vector3 movement = new(
            moveInput.x, 0f, moveInput.y
        );

        Vector3.ClampMagnitude(movement, 1f);
        float moveSpeed = _dashAction.IsPressed() ? _dashSpeed : _speed;
        _controller.SimpleMove(moveSpeed * movement);

        if (movement != Vector3.zero)
        {
            // _model.forward = movement;
            _model.forward = Vector3.Slerp(_model.forward, movement, _rotateRate);
            _animator.SetBool("Running", true);
        }
        else
        {
            _animator.SetBool("Running", false);
        } 
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_playerPunch.IsBeingHit) return;
        Debug.Log($"{gameObject.name} took {damageInfo.Amount} damage.");
        _health -= damageInfo.Amount;
        if (_health <= 0)
        {
            _animator.SetTrigger("Die");
            _controller.enabled = false;
            _isDead = true;
        }
        else
        {
            _animator.SetTrigger("Hit");
            _playerPunch.IsBeingHit = true;
        }
    }
}
