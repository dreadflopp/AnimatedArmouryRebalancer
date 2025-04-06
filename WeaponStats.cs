namespace AnimatedArmouryRebalancer
{
    public class WeaponStats
    {
        public float Speed { get; set; }
        public float Reach { get; set; }
        public float Stagger { get; set; }
        public int BaseDamage { get; set; }
        public int CriticalDamage { get; set; }

        public WeaponStats(float speed, float reach, float stagger, int baseDamage, int criticalDamage)
        {
            Speed = speed;
            Reach = reach;
            Stagger = stagger;
            BaseDamage = baseDamage;
            CriticalDamage = criticalDamage;
        }
    }
} 