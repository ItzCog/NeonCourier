using System.Collections;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 5;
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _FlashWhiteDuration = 0.1f;

    private Collider _collider;
    private bool _isDead = false;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        Debug.Log($"{gameObject.name} took {damageInfo.Amount} damage.");
        _health -= damageInfo.Amount;

        StartCoroutine(FlashWhite());

        if (_health <= 0)
        {
            _animator.SetTrigger("Die");
            _isDead = true;
            _collider.enabled = false;
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }

    private IEnumerator FlashWhite()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetFloat("_GotHit", 1f);
        }

        yield return new WaitForSecondsRealtime(_FlashWhiteDuration);

        foreach (var renderer in _renderers)
        {
            renderer.material.SetFloat("_GotHit", 0f);
        }
    }
}
