using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class IsAgro: IState
    {
        private readonly BotBrain brain;

        public IsAgro(BotBrain _brain)
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