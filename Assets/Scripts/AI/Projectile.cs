using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    
    private void Update()
    {
        transform.position += _speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(new DamageInfo { Amount = 1 });
            Destroy(gameObject);
        }
        else if (other.gameObject.isStatic)
        {
            Destroy(gameObject);
        }
    }
}
