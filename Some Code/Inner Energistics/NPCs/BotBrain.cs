using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;
using System;

namespace Energistics.Enemies
{
    public class BotBrain : MonoBehaviour
    {
        StateMachine stateMachine;
        Animator animator;
        public Animator Animator { get { return animator; } }

        [Header("Stats")]
        [SerializeField] float reactionTime = 0.2f;
        [SerializeField] float agroRadius = 20;

        [Header("Target Info")]
        [SerializeField] List<Transform> potentialTargets = new List<Transform>();
        Transform currentTarget;
        public Transform CurrentTarget { get { return currentTarget; } }

        [Header("Body Parts")]
        [SerializeField] BotEars ears;
        public BotEars Ears { get { return ears; } }
        [SerializeField] BotEyes eyes;
        public BotEyes Eyes { get { return eyes; } }
        [SerializeField] BotLegs legs;
        public BotLegs Legs { get { return legs; } }
        [SerializeField] BotArms arms;
        public BotArms Arms { get { return arms; } }

        bool inReach = false;
        private void Awake()
        {
            BotInitialization();
        }

        protected void BotInitialization()
        {
            legs = GetComponent<BotLegs>();
            ears = GetComponent<BotEars>();
            eyes = GetComponent<BotEyes>();
            arms = GetComponent<BotArms>();
            animator = GetComponent<Animator>();
            stateMachine = new StateMachine();

            var idle = new IsIdle(this);
            var approaching = new IsApproaching(this);
            var attacking = new IsAttacking(this);

            stateMachine.AddTransition(idle, approaching, HasTarget());
            stateMachine.AddTransition(approaching, attacking, InReach());
            stateMachine.AddTransition(approaching, idle, HasNoTarget());
            stateMachine.AddTransition(attacking, approaching, NotInReach());
            stateMachine.AddTransition(attacking, idle, HasNoTarget());

            stateMachine.SetState(idle);

            Func<bool> HasTarget() => () => currentTarget != null;
            Func<bool> HasNoTarget() => () => currentTarget == null;
            Func<bool> InReach() => () => inReach;
            Func<bool> NotInReach() => () => !inReach;
        }

        private void Update()
        {
            UpdateLoop();
        }

        protected void UpdateLoop()
        {
            stateMachine.Tick();
            AddPotentialTargets();
            if (currentTarget != null)
            {
                if (arms.CheckReachToTarget(currentTarget.position))
                {
                    arms.Attack();
                    inReach = true;
                }
                else
                {
                    arms.StopAttack();
                    inReach = false;
                }
            }
        }

        public void ChangeReactionTime(float _time)
        {
            reactionTime = _time;
        }
        public void ChangeAgroRadius(float _radius)
        {
            agroRadius = _radius;
        }
        public void AddPotentialTargets()
        {
            potentialTargets.Clear();
            potentialTargets.AddRange(eyes.VisibleTargets);
            potentialTargets.AddRange(ears.AudibleTargets);
            SelectTarget();
        }
        
        public void SelectTarget()
        {
            if (potentialTargets.Count > 0)
            {
                currentTarget = potentialTargets[0];
                legs.SetTarget(currentTarget);
                IDamageable damageable;
                if (currentTarget.TryGetComponent(out damageable))
                {
                    arms.SetTarget(damageable);
                }
            }
            else
            {
                currentTarget = null;
            }
        }
    }
}