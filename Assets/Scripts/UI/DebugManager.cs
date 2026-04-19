using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHealth;
    [SerializeField] private TMP_Text playerWeapon;
    [SerializeField] private TMP_Text room;
    
    [SerializeField] private Slider playerDamage;
    [SerializeField] private TMP_Text playerDamageText;
    [SerializeField] private Slider playerMoveSpeed;
    [SerializeField] private TMP_Text playerMoveSpeedText;
    [SerializeField] private Slider enemyAttackCooldown;
    [SerializeField] private TMP_Text enemyAttackText;

    [SerializeField] private Transform damageInfo;

    [SerializeField] private TMP_Dropdown timeScale;

    private Player _player;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        DamageCalculator.OnDamageDealt += OnDamageDealt;
    }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        room.text = $"{FindObjectsOfType<EnemyBase>().Length} enemies";
    }

    private void Update()
    {
        playerHealth.text = $"Player Health = {_player.GetHealth()}";
        playerWeapon.text = $"Weapon = {_player.GetWeapon().name}";
        
        playerDamageText.text = playerDamage.value.ToString();
        playerMoveSpeedText.text = playerMoveSpeed.value.ToString();
        enemyAttackText.text = enemyAttackCooldown.value.ToString();
        
        _player.SetDamage((int)playerDamage.value);
        _player.SetSpeed(playerMoveSpeed.value);

        Time.timeScale = timeScale.value switch
        {
            0 => 0.5f,
            1 => 1f,
            2 => 2f,
            _ => 1f
        };

        if (Input.GetKeyDown(KeyCode.F1))
        {
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
        }
    }

    private void OnDamageDealt(IDamageSource dealer, IDamageable receiver, int amount)
    {
        string dealerName = ((Component)dealer).gameObject.name;
        string receiverName = ((Component)receiver).gameObject.name;

        var newEntry = new GameObject();
        var text = newEntry.AddComponent<TextMeshProUGUI>();
        text.text = $"{dealerName} -> {receiverName} ({amount})";
        text.fontSize = 24f;
        text.enableWordWrapping = false;
        
        newEntry.transform.SetParent(damageInfo.transform);
    }
}
