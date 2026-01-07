using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private GameObject _interactText;

    public IInteractable InteractingObject { get; set; }
    
    private CharacterController _controller;
    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _dashAction;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Interact");
        _dashAction = InputSystem.actions.FindAction("Dash");

        _interactAction.performed += _ => InteractingObject?.Interact();
    }

    private void Update()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        Vector3 movement = new(
            moveInput.x, 0f, moveInput.y
        );

        float moveSpeed = _dashAction.IsPressed() ? _dashSpeed : _speed;
        _controller.Move(moveSpeed * Time.deltaTime * movement);

        _interactText.SetActive(InteractingObject != null);
    }
}
