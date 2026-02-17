using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private Transform _punchHand;
    [SerializeField] private float _punchRadius = 0.1f;
    [SerializeField] private float _cameraShakeAmplitude = 1.5f;
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _hitStopDuration = 0.1f;
    [SerializeField] private float _hitStopTimescale = 0.1f;
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private TMP_Text _dpsText;

    public bool IsBeingHit { get; set; }
    
    private bool _isPunching = false;
    private HashSet<Collider> _hitEnemies = new();
    private int _enemyLayer;
    private GameCamera _camera;

    private struct DamageTime
    {
        public int Damage;
        public float Time;
    }
    private Queue<DamageTime> _damageQueue = new();

    private void Start()
    {
        _enemyLayer = LayerMask.GetMask("Enemies");
        _camera = GameCamera.Instance;
    }

    private void Update()
    {
        while (_damageQueue.Count > 0 && Time.time - _damageQueue.Peek().Time > 1f)
        {
            _damageQueue.Dequeue();
        }

        float dps = 0f;
        foreach (var entry in _damageQueue)
        {
            dps += entry.Damage;
        }

        _dpsText.text = $"{dps} dps";
    }

    private void FixedUpdate()
    {
        if (!_isPunching) return;

        var collisions = Physics.OverlapSphere(
            _punchHand.position,
            _punchRadius,
            _enemyLayer
        );

        foreach (var collider in collisions)
        {
            if (_hitEnemies.Contains(collider)) continue;
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                HitEnemy(damageable, new DamageInfo { Amount = _weaponData.damage });
                _damageQueue.Enqueue(new DamageTime { Damage = _weaponData.damage, Time = Time.time });
            }

            _hitEnemies.Add(collider);
        }
    }

    private void HitEnemy(IDamageable damageable, DamageInfo damageInfo)
    {
        _camera.Shake(_cameraShakeAmplitude, _cameraShakeDuration);
        damageable.TakeDamage(damageInfo);
        StartCoroutine(HitStop());
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = _hitStopTimescale;
        yield return new WaitForSecondsRealtime(_hitStopDuration);
        Time.timeScale = 1f;
    }

    // Animation Event
    private void StartPunch()
    {
        _isPunching = true;
        _hitEnemies.Clear();
    }

    // Animation Event
    private void EndPunch()
    {
        _isPunching = false;
    }
    
    // Animation Event
    private void EndBeingHit()
    {
        IsBeingHit = false;
    }
}
