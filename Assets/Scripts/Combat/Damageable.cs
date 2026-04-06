using System.Collections.Generic;

public interface IDamageable
{
    public void TakeDamage(DamageInfo damageInfo);
}

public interface IDamageSource
{
    public List<Modifier> Modifiers { get; }
}
