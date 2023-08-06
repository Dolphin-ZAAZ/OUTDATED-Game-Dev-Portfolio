using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Energistics.Behaviour
{
    public class DestructionController : MonoBehaviour
    {
        float jointDestructionPercentage = 0f;
        float brickDestructionPercentage = 0f;
        float structuralPercentage = 100f;
        int initialConnectionsAmount = 0;
        int connectionsAmount = 0;
        Dictionary<Brick, int> brickJointCounter = new Dictionary<Brick, int>();
        int breaks = 0;
        bool broken = false;
        public bool Broken { get { return broken; } }

        [SerializeField] float structuralPercentageCutoff = 40f;
        [SerializeField] float despawnRadius = 40f;
        [SerializeField] bool shouldDissolve = true;
        [SerializeField] int dissolveChunkSize = 3;
        [SerializeField] float dissolveTimeStep = 0.5f;
        [SerializeField] float dissolveDelay = 5f;
        [SerializeField] bool hasFunction = false;
        [SerializeField] GameObject functionObject;
        bool disolving;
        float dissolveTimer;

        List<Brick> allBricks = new List<Brick>();
        List<Brick> activeBricks = new List<Brick>();
        private void Awake()
        {
            allBricks.AddRange(GetComponentsInChildren<Brick>());
            activeBricks.AddRange(GetComponentsInChildren<Brick>());
            for (int i = 0; i < allBricks.Count; i++)
            {
                allBricks[i].GetComponent<DefaultDamagable>().death.AddListener(RemoveActiveBrick);
                allBricks[i].GetComponent<DefaultDamagable>().death.AddListener(CountJoints);
            }
            brickDestructionPercentage = 100f;
            broken = false;
        }
        private void Update()
        {
            if (disolving)
            {
                dissolveTimer -= Time.deltaTime;
                if (dissolveTimer <= 0f)
                {
                    for (int i = 0; i < dissolveChunkSize; i++)
                    {
                        if (activeBricks.Count > 0)
                        {
                            activeBricks[0].gameObject.SetActive(false);
                            RemoveActiveBrick(activeBricks[0].gameObject);
                        }
                        else
                        {
                            disolving = false;
                            break;
                        }
                    }
                    dissolveTimer = dissolveTimeStep;
                }
            }
        }
        public void CountJoints(GameObject thingy)
        {
            for (int i = 0; i < activeBricks.Count; i++)
            {
                brickJointCounter[activeBricks[i]] = activeBricks[i].JointAmount;
            }
            connectionsAmount = brickJointCounter.Sum(x => x.Value);
            float tempInitialConnections = initialConnectionsAmount;
            float tempConnections = connectionsAmount;
            jointDestructionPercentage = (tempConnections / tempInitialConnections) * 100f;
        }
        public void SetInitialJointCount()
        {
            initialConnectionsAmount = connectionsAmount;
        }
        public void CheckStructuralIntegrity()
        {
            if (brickDestructionPercentage < jointDestructionPercentage)
            {
                structuralPercentage = brickDestructionPercentage;
            }
            else
            {
                structuralPercentage = jointDestructionPercentage;
            }
            if (structuralPercentage < structuralPercentageCutoff)
            {
                broken = true;
                if (hasFunction)
                {
                    functionObject.SetActive(false);
                }
                disolving = true;
                dissolveTimer = dissolveDelay;
            }
        }
        private void RemoveActiveBrick(GameObject brick)
        {
            float _activeBricks = activeBricks.Count;
            float _allBricks = allBricks.Count;
            brickDestructionPercentage = (_activeBricks/ _allBricks) * 100f;
            CheckStructuralIntegrity();
            activeBricks.Remove(brick.GetComponent<Brick>());
            brickJointCounter.Remove(brick.GetComponent<Brick>());
        }
    }
}