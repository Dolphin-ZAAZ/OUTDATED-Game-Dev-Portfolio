using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class BotStats : MonoBehaviour, IDamageable
    {
        [SerializeField] float health = 1000;
        [SerializeField] float movementSpeed = 3f;
        [SerializeField] float attackDamage = 10f;
        [SerializeField] float attackReach = 1f;
        [SerializeField] float agroRadius = 20f;
        [SerializeField] float reactionTime = 0.2f;
        [SerializeField] float attackTime = 2f;
        [SerializeField] bool isKilled = false;
        [SerializeField] BotBrain brain;
        Spawner spawner;
        public int Hardness => 1;
        private void Start()
        {
            brain = GetComponent<BotBrain>();
            brain.Legs.ChangeSpeed(movementSpeed);
            brain.Arms.ChangeReach(attackReach);
            brain.Arms.ChangeAttackDamage(attackDamage);
            brain.Arms.ChangeAttackTime(attackTime);
            brain.ChangeAgroRadius(agroRadius);
            brain.ChangeReactionTime(reactionTime);

            spawner = GetComponentInParent<Spawner>();
        }

        public void Damage(float amount, GameObject damager)
        {
            health -= amount;
            if (health <= 0) 
            { 
                Death(damager); 
            }
        }

        public void Death(GameObject damager)
        {
            if (spawner != null)
            {
                spawner.RemoveActiveEnemy(gameObject);
            }
            gameObject.SetActive(false);
        }

        public float GetHealth()
        {
            return health;
        }

        public bool Killed()
        {
            return isKilled;
        }
    }
}