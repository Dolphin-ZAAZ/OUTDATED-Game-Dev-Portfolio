using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public class Shotgun : Gun
    {
        [SerializeField] int gauge = 8;
        public Shotgun(float _damage, float _spread, float _recoil, int _gauge) : base(_damage, _spread, _recoil)
        {
            gauge = _gauge;
        }
        override public void OnUse()
        {
            ResetTriggers();
            animator.SetBool("IsAttacking", true);
            for (int i = 0; i < gauge; i++)
            {
                Fire();
            }
            ResetTriggers();
        }
    }
}