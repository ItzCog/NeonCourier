using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Gameplay/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public int damage;
    public float attackInterval;
    public int range;
    public float hitBoxRadius = 0.1f;
    public GameObject vfx;
}
