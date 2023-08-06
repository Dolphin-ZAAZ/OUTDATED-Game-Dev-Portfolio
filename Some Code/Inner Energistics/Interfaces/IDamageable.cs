using UnityEngine;

namespace Energistics.Behaviour
{
    public interface IDamageable
    {
        public int Hardness { get; }
        void Damage(float amount, GameObject damager);
        float GetHealth();
        bool Killed();
        void Death(GameObject damager);
    }
}