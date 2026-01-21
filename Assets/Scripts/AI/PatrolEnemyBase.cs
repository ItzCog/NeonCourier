using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PatrolEnemyBase : EnemyBase
{
    [FormerlySerializedAs("_patrolPoints")] public Vector3[] PatrolPoints;
    [SerializeField] private float _aggroRange = 5f;
    [SerializeField] protected float _attackRange = 1f;
    [FormerlySerializedAs("_punchInterval")] [SerializeField] private float _attackInterval = 1f;

    protected enum EnemyState
    {
        Patrol, Chase, Attack, Idle
    }

    protected EnemyState _enemyState = EnemyState.Patrol;
    protected GameObject _player;
    private NavMeshAgent _agent;
    
    protected float _lastAttackTime;
    private int _currentPatrolIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    protected virtual void Update()
    {
        if (_isDead) return;
        if (_isBeingHit)
        {
            _agent.isStopped = true;
            return;
        }
        
        switch (_enemyState)
        {
            case EnemyState.Patrol:
                _animator.SetBool("Running", true);
                if (IsPlayerInAggroRange())
                {
                    _enemyState = EnemyState.Chase;
                }
                else if (PatrolPoints.Length == 0)
                {
                    _enemyState = EnemyState.Idle;
                }
                else
                {
                    Patrol();
                }
                break;
            
            case EnemyState.Chase:
                _animator.SetBool("Running", true);
                _agent.SetDestination(_player.transform.position);
                _agent.isStopped = false;
                
                if (CanAttack())
                {
                    _enemyState = EnemyState.Attack;
                }
                break;
            
            case EnemyState.Attack:
                _animator.SetBool("Running", false);
                _agent.isStopped = true;

                if (Time.time - _lastAttackTime > _attackInterval)
                {
                    if (CanAttack())
                    {
                        Attack();
                    }
                    else
                    {
                        _enemyState = EnemyState.Chase;
                    }
                }

                break;
            
            case EnemyState.Idle:
                _animator.SetBool("Running", false);
                if (IsPlayerInAggroRange())
                {
                    _enemyState = EnemyState.Chase;
                }
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Patrol()
    {
        _agent.SetDestination(PatrolPoints[_currentPatrolIndex]);
        if (Vector3.SqrMagnitude(transform.position - PatrolPoints[_currentPatrolIndex]) <=
            _agent.stoppingDistance * _agent.stoppingDistance)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % PatrolPoints.Length;
            // print("Arrived");
        }
    }

    private bool IsPlayerInAggroRange()
    {
        return Vector3.SqrMagnitude(_player.transform.position - transform.position) <=
               _aggroRange * _aggroRange;
    }
    
    protected virtual bool CanAttack()
    {
        return false;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _agent.isStopped = true;
    }

    protected virtual void Attack()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

[CustomEditor(typeof(PatrolEnemyBase))]
public class PatrolEnemyEditor : Editor
{
    void OnSceneGUI()
    {
        var enemy = (PatrolEnemyBase)target;

        // Draw a position handle at the current targetPosition

        for (int i = 0; i < enemy.PatrolPoints.Length; ++i)
        {
            enemy.PatrolPoints[i] = Handles.PositionHandle(enemy.PatrolPoints[i], Quaternion.identity);
            Handles.Label(enemy.PatrolPoints[i] + Vector3.up * 0.5f, "Patrol Point");
        }
    }
}

