using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected EnemyData _enemyData;
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _flashWhiteDuration = 0.1f;

    protected bool _isDead = false;
    protected bool _isBeingHit = false;
    
    private float _health;
    private Collider _collider;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _health = _enemyData.health;
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        _isBeingHit = true;
        Debug.Log($"{gameObject.name} took {damageInfo.Amount} damage.");
        _health -= damageInfo.Amount;

        StartCoroutine(FlashWhite());

        if (_health <= 0)
        {
            _animator.SetTrigger("Die");
            _isDead = true;
            _collider.enabled = false;
            OnDeath();
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }

    protected virtual void OnDeath()
    {
        
    }

    // Animation Event
    private void EndBeingHit()
    {
        _isBeingHit = false;
    }

    private IEnumerator FlashWhite()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetFloat("_GotHit", 1f);
        }

        yield return new WaitForSecondsRealtime(_flashWhiteDuration);
        
        foreach (var renderer in _renderers)
        {
            renderer.material.SetFloat("_GotHit", 0f);
        }
    }
}