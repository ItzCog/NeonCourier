
using UnityEditor;
using UnityEngine;

public class MeleeEnemy : PatrolEnemyBase
{
    [SerializeField] private Transform _punch;
    
    private bool _isPunching;
    
    protected override void Update()
    {
        if (_isDead) return;
        if (_isBeingHit)
        {
            return;
        }
        
        base.Update();
        ProcessPunch();
    }
    
    private void ProcessPunch()
    {
        if (!_isPunching) return;
        
        var collisions = Physics.OverlapSphere(
            _punch.position,
            0.1f
        );

        foreach (var collider in collisions)
        {
            if (!collider.TryGetComponent<Player>(out var player)) continue;
            
            player.TakeDamage(new DamageInfo { Amount = 1 });
            _isPunching = false;
            break;
        }
    }

    protected override void Attack()
    {
        var direction = _player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        transform.forward = direction;
        
        _animator.SetTrigger("Punch");
        _lastAttackTime = Time.time;
    }

    protected override bool CanAttack()
    {
        return Vector3.SqrMagnitude(_player.transform.position - transform.position) <=
               _attackRange * _attackRange;
    }

    // Animation Event
    private void StartPunch()
    {
        _isPunching = true;
    }
    
    // Animation Event
    private void EndPunch()
    {
        _isPunching = false;
    }
}

[CustomEditor(typeof(MeleeEnemy))]
public class MeleeEnemyEditor : Editor
{
    void OnSceneGUI()
    {
        var enemy = (MeleeEnemy)target;

        // Draw a position handle at the current targetPosition

        for (int i = 0; i < enemy.PatrolPoints.Length; ++i)
        {
            enemy.PatrolPoints[i] = Handles.PositionHandle(enemy.PatrolPoints[i], Quaternion.identity);
            Handles.Label(enemy.PatrolPoints[i] + Vector3.up * 0.5f, "Patrol Point");
        }
    }
}
