using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class IsIdle: IState
    {
        private readonly BotBrain brain;

        public IsIdle(BotBrain _brain)
        {
            brain = _brain;
        }
        public void OnEnter()
        {
            brain.Legs.StopMoving();
        }

        public void OnExit()
        {
            if (brain.CurrentTarget != null)
            {
                brain.Animator.SetBool("HasTarget", true);
            }
        }

        public void Tick()
        {
            return;
        }
    }
}