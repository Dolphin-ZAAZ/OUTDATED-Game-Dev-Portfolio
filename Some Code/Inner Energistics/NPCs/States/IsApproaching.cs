using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class IsApproaching: IState
    {
        private readonly BotBrain brain;

        public IsApproaching(BotBrain _brain)
        {
            brain = _brain;
        }
        public void OnEnter()
        {
            brain.Animator.SetBool("InReach", false);
        }

        public void OnExit()
        {
            brain.Animator.SetBool("InReach", true);
        }

        public void Tick()
        {
            brain.Legs.Follow();
        }
    }
}