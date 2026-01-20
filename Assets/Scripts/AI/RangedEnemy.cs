using System.Collections;
using System.Collections.Generic;
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
