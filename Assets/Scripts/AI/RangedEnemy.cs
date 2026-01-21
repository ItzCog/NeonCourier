using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RangedEnemy : PatrolEnemyBase
{
    [SerializeField] private Transform _hand;
    [SerializeField] private GameObject _projectilePrefab;
    
    protected override void Attack()
    {
        var direction = _player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        transform.forward = direction;
        
        _animator.SetTrigger("Punch");
        _lastAttackTime = Time.time;
    }
    
    // Animation Event
    private void Throw()
    {
        var direction = _player.transform.position - _hand.position + Vector3.up * 1f;
        direction.Normalize();
        
        Instantiate(
            _projectilePrefab,
            _hand.position,
            Quaternion.LookRotation(direction)
        );
    }
}

[CustomEditor(typeof(RangedEnemy))]
public class RangedEnemyEditor : Editor
{
    private void OnSceneGUI()
    {
        var enemy = (RangedEnemy)target;

        // Draw a position handle at the current targetPosition

        for (int i = 0; i < enemy.PatrolPoints.Length; ++i)
        {
            enemy.PatrolPoints[i] = Handles.PositionHandle(enemy.PatrolPoints[i], Quaternion.identity);
            Handles.Label(enemy.PatrolPoints[i] + Vector3.up * 0.5f, "Patrol Point");
        }
    }
}
