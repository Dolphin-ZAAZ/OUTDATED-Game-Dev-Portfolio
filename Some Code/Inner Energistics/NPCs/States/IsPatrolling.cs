using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class IsPatrolling: IState
    {
        private readonly BotBrain brain;

        public IsPatrolling(BotBrain _brain)
        {
            brain = _brain;
        }
        public void OnEnter()
        {
            return;
        }

        public void OnExit()
        {
            return;
        }

        public void Tick()
        {
            brain.Legs.MoveRandomly();
        }
    }
}