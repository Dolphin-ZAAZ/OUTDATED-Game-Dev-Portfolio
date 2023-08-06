using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class BotLegs : MonoBehaviour
    {
        bool isFollowing = false;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float distanceToRecalculate = 1f;
        float speed = 3.5f;
        BotBrain brain;

        Transform target;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            
            brain = GetComponent<BotBrain>();
        }
        public void ChangeSpeed(float _speed)
        {
            agent.speed = _speed;
        }
        public void MoveTo(Vector3 destination)
        {
            agent.SetDestination(destination);
            agent.isStopped = false;
        }

        public void QueueMovement(Vector3 destination)
        {
            
        }

        public void MovementPaused(bool status)
        {
            agent.isStopped = status;
        }

        public void StopMoving()
        {

        }

        public void MoveRandomly()
        {

        }
        public void SetTarget(Transform _target)
        {
            target = _target;
        }
        public void Follow()
        {
            if (target != null)
            {
                if (isFollowing == false)
                {
                    MoveTo(target.position);
                    isFollowing = true;
                }
                if (Vector3.Distance(agent.transform.position, agent.nextPosition) < distanceToRecalculate)
                {
                    MoveTo(target.position);
                    isFollowing = true;
                }
                if (Vector3.Distance(agent.transform.position, target.position) < brain.Arms.ReachDistance)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                isFollowing = false;
            }
        }
    }
}