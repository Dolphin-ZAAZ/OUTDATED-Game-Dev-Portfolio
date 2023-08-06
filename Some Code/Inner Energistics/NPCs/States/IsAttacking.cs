using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class IsAttacking: IState
    {
        private readonly BotBrain brain;

        public IsAttacking(BotBrain _brain)
        {
            brain = _brain;
        }
        public void OnEnter()
        {
            brain.Animator.SetBool("InReach", true);
        }

        public void OnExit()
        {
            brain.Animator.SetBool("InReach", false);
            if (brain.CurrentTarget == null)
            {
                brain.Animator.SetBool("HasTarget", false);
            }
        }

        public void Tick()
        {
            return;
        }
    }
}