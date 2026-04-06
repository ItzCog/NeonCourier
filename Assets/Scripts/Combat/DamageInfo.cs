public class DamageInfo
{
    public int baseDamage;
    public IDamageSource source;
    
    public enum DamageType { Physical, Skill, Projectile }
    public DamageType damageType;

    public DamageInfo(int baseDamage, IDamageSource source, DamageType damageType)
    {
        this.baseDamage = baseDamage;
        this.source = source;
        this.damageType = damageType;
    }
}