public interface IDamageable
{
    public void TakeDamage(DamageInfo damageInfo);
}

public struct DamageInfo
{
    public int Amount { get; set; }
}
