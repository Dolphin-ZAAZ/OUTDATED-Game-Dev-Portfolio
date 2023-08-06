using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class BotArms : MonoBehaviour
    {
        bool attacking;
        float itimer = 0f;
        float attackTime = 2f;
        [SerializeField] float timerOffset = 1f;
        float attackDamage = 1f;
        float reachDistance = 1f;
        public float ReachDistance { get { return reachDistance; } }
        IDamageable target;
        [SerializeField] AudioSource attackSound;
        private void Start()
        {
            itimer = timerOffset;
        }
        private void Update()
        {
            if (target != null)
            {
                if (attacking)
                {
                    itimer -= Time.deltaTime;
                    if (itimer < 0)
                    {
                        itimer = attackTime;
                        target.Damage(attackDamage, gameObject);
                        attackSound.Play();
                    }
                }
                else
                {
                    itimer = timerOffset;
                }
            }
        }
        public void ChangeAttackTime(float _time)
        {
            attackTime = _time;
        }
        public void ChangeReach(float _reach)
        {
            reachDistance = _reach;
        }
        public void ChangeAttackDamage(float _damage)
        {
            attackDamage = _damage;
        }
        public bool CheckReachToTarget(Vector3 _target)
        {
            if (_target != null)
            {
                if (Vector3.Distance(transform.position, _target) <= reachDistance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void SetTarget(IDamageable _target)
        {
            target = _target;
        }
        public void Attack()
        {
            attacking = true;
        }

        public void StopAttack()
        {
            attacking = false;
        }
    }
}