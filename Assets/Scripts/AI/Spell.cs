using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;

    private Collider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(_duration);
        var results =
            Physics.OverlapBox(transform.position, new Vector3(1f, 1f, 1f), Quaternion.identity);
        foreach (var result in results)
        {
            if (result.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(new DamageInfo {Amount = 3});
                break;
            }
        }
        
        Destroy(gameObject);
    }
}
