using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable, IDamageSource
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected EnemyData _enemyData;
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _flashWhiteDuration = 0.1f;

    protected bool _isDead = false;
    protected bool _isBeingHit = false;
    protected GameObject _player;
    
    private float _health;
    private Collider _collider;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _health = _enemyData.health;
    }
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        _isBeingHit = true;
        
        int damage = DamageCalculator.Calculate(damageInfo.source.Modifiers, null, damageInfo);
        Debug.Log($"{gameObject.name} took {damage} damage.");
        _health -= damage;

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
        
        DamageCalculator.DamageDealt(damageInfo.source, this, damageInfo.baseDamage);
    }

    public List<Modifier> Modifiers => new();

    protected virtual void OnDeath()
    {
        SpawnDrop();
    }

    private void SpawnDrop()
    {
        if (_enemyData.weaponDrops.Count == 0) return;
        float p = Random.Range(0f, 1f);
        WeaponData drop = null;
        foreach (var entry in _enemyData.weaponDrops)
        {
            p -= entry.prob;
            if (p < 0f)
            {
                drop = entry.data;
                break;
            }
        }

        if (drop == null || drop == _player.GetComponent<Player>()._weaponData) return;
        var dropObj = Instantiate(drop.dropPrefab, transform.position, Quaternion.identity);
        dropObj.GetComponent<WeaponDrop>().data = drop;
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