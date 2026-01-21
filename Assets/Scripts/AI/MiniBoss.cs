using System;
using UnityEngine;
using UnityEngine.AI;

public class MiniBoss : EnemyBase
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float _aggroRange = 5f;
    [SerializeField] private float _attackRange = 2f;

    private GameObject _player;
    private NavMeshAgent _agent;

    private bool _isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void Update()
    {
        if (_isBeingHit)
        {
            _agent.isStopped = true;
            return;
        }

        float sqrDist = Vector3.SqrMagnitude(_player.transform.position - transform.position);
        if (sqrDist > _aggroRange * _aggroRange)
        {
            _animator.SetBool("Running", false);
            return;
        }
        
        if (_isAttacking) return;

        if (sqrDist < _attackRange * _attackRange)
        {
            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            transform.forward = direction;
            
            _animator.SetTrigger("CastSpell");
            _agent.isStopped = true;
            _isAttacking = true;
        }
        else
        {
            _agent.isStopped = false;
            _animator.SetBool("Running", true);
            _agent.SetDestination(_player.transform.position);
        }
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        _isAttacking = false;
        base.TakeDamage(damageInfo);
    }

    // Animation Event
    private void Cast()
    {
        print("attack");
        Instantiate(spellPrefab, _player.transform.position, Quaternion.identity);
    }

    // Animation Event
    private void EndCast()
    {
        _isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
