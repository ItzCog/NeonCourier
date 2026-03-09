using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Gameplay/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int health;
    public int damage;
    public float moveSpeed;
    public float attackInterval;
    public float chaseRange = 5f;
    public float attackRange = 1f;

    public List<GameObject> itemDrops;
}
