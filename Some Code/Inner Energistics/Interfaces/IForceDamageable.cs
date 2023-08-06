using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public interface IForceDamageable
    {
        public float ForceDamageMultiplier { get; }
        public float ApplyDamage(float velocity);
    }
}