using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;


    public DamageInfo DamageInfo;
    
    private void Update()
    {
        transform.position += _speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(DamageInfo);
            Destroy(gameObject);
        }
        else if (other.gameObject.isStatic)
        {
            Destroy(gameObject);
        }
    }
}
