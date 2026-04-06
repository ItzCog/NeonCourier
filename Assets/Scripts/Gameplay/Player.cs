using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IDamageable, IDamageSource
{
    [SerializeField] private float _speed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _rotateRate = 0.5f;
    [SerializeField] private GameObject _interactText;
    [SerializeField] private Transform _model;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 5;
    [SerializeField] private PlayerPunch _playerPunch;
    [SerializeField] private float _invincibilityDuration = 2f;
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private TMP_Text _dpsText;
    [SerializeField] private Transform _punchHand;

    public IInteractable InteractingObject { get; set; }
    
    private CharacterController _controller;
    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _dashAction;
    private InputAction _punchAction;
    
    private GameCamera _camera;
    private int _enemyLayer;
    
    private bool _isPunching = false;
    private HashSet<Collider> _hitEnemies = new();
    
    private bool _isDead = false;
    private bool _isInvincible = false;
    private bool _isBeingHit;

    private float _lastAttackTime;
    
    private struct DamageTime
    {
        public int Damage;
        public float Time;
    }
    private Queue<DamageTime> _damageQueue = new();

    private int _damageBoostTest = 1;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
        _enemyLayer = LayerMask.GetMask("Enemies");

        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Interact");
        _dashAction = InputSystem.actions.FindAction("Dash");
        _punchAction = InputSystem.actions.FindAction("Punch");

        _interactAction.performed += _ => InteractingObject?.Interact();
        _punchAction.performed += _ => Punch();

        _playerPunch.EndedPunch += EndPunch;
        _playerPunch.EndedBeingHit += EndBeingHit;
    }
    
    private void Start()
    {
        _camera = GameCamera.Instance;
    }

    private void Update()
    {
        _interactText.SetActive(InteractingObject != null);
        
        if (_isBeingHit || _isDead) return;
        
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

        while (_damageQueue.Count > 0 && Time.time - _damageQueue.Peek().Time > 1f)
        {
            _damageQueue.Dequeue();
        }

        float dps = 0f;
        foreach (var entry in _damageQueue)
        {
            dps += entry.Damage;
        }

        _dpsText.text = $"{dps} dps";
        
        
        // Testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            print("No damage boost");
            _damageBoostTest = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("+5 damage");
            _damageBoostTest = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("+10% damage");
            _damageBoostTest = 3;
        }
    }

    private void FixedUpdate()
    {
        if (!_isPunching) return;

        var collisions = Physics.OverlapSphere(
            _punchHand.position,
            _weaponData.hitBoxRadius,
            _enemyLayer
        );

        foreach (var collider in collisions)
        {
            if (_hitEnemies.Contains(collider)) continue;
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                var info = new DamageInfo(_weaponData.damage, this, DamageInfo.DamageType.Physical);
                HitEnemy(damageable, info);
                _damageQueue.Enqueue(new DamageTime { Damage = DamageCalculator.Calculate(Modifiers, new(),
                    info), Time = Time.time });
            }

            _hitEnemies.Add(collider);
        }
    }

    public List<Modifier> Modifiers => _damageBoostTest switch
    {
        1 => new List<Modifier>(),
        2 => new List<Modifier> { new (Modifier.Type.Flat, 5) },
        3 => new List<Modifier> { new (Modifier.Type.Percent, 10) },
        _ => throw new InvalidEnumArgumentException()
    };

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isInvincible) return;
        if (_isBeingHit) return;
        Debug.Log($"{gameObject.name} took {damageInfo.baseDamage} damage.");
        _health -= damageInfo.baseDamage;
        if (_health <= 0)
        {
            _animator.SetTrigger("Die");
            _controller.enabled = false;
            _isDead = true;
        }
        else
        {
            _animator.SetTrigger("Hit");
            _isBeingHit = true;
            _isInvincible = true;
            StartCoroutine(RemoveInvincibility());
        }
    }
    
    private void HitEnemy(IDamageable damageable, DamageInfo damageInfo)
    {
        _camera.Shake(1.5f, 0.05f);
        damageable.TakeDamage(damageInfo);
        StartCoroutine(HitStop());
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }

    private void Punch()
    {
        if (_lastAttackTime + _weaponData.attackInterval > Time.time) return;
        
        _lastAttackTime = Time.time;
        _animator.SetTrigger("Punch");
        _isPunching = true;
    }

    private void EndPunch()
    {
        _isPunching = false;
        _hitEnemies.Clear();
    }
    
    private void EndBeingHit()
    {
        _isBeingHit = false;
    }

    private IEnumerator RemoveInvincibility()
    {
        yield return new WaitForSecondsRealtime(_invincibilityDuration);
        _isInvincible = false;
    }
}
