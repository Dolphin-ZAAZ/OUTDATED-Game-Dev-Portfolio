using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public class BeamBrick : Brick
    {
        [SerializeField] LayerMask beamLayer;
        [SerializeField] float beamSize = 10f;
        List<Rigidbody> leftBodies = new List<Rigidbody>();
        List<Rigidbody> rightBodies = new List<Rigidbody>();
        List<Rigidbody> upBodies = new List<Rigidbody>();
        List<Rigidbody> downBodies = new List<Rigidbody>();
        List<Rigidbody> forwardBodies = new List<Rigidbody>();
        List<Rigidbody> backwardBodies = new List<Rigidbody>();

        private void Awake()
        {
            brickSize = normalBrickSize * beamSize;
            if (GetComponent<Rigidbody>() == false)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.useGravity = false;
                rb.mass = mass;
                rb.drag = drag;
                rb.angularDrag = angularDrag;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
            else
            {
                rb = gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.useGravity = false;
                rb.mass = mass;
                rb.drag = drag;
                rb.angularDrag = angularDrag;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
            damagable = this.GetComponent<DefaultDamagable>();
            damagable.death.AddListener(OnJointDeath);
            destructionController = GetComponentInParent<DestructionController>();
            if (isReinforced)
            {
                brickSize = brickSize * reinforcementSize;
            }
        }
        void Start()
        {
            List<RaycastHit> hits = new List<RaycastHit>();
            List<RaycastHit> leftHits = new List<RaycastHit>();
            List<RaycastHit> rightHits = new List<RaycastHit>();
            List<RaycastHit> upHits = new List<RaycastHit>();
            List<RaycastHit> downHits = new List<RaycastHit>();
            List<RaycastHit> forwardHits = new List<RaycastHit>();
            List<RaycastHit> backwardHits = new List<RaycastHit>();
            brickSize = normalBrickSize * beamSize;
            for (int i = -(brickSubdivisions / 2); i < brickSubdivisions - (brickSubdivisions / 2); i++)
            {
                
            }
            for (int i = -brickSubdivisions/2; i < brickSubdivisions - (brickSubdivisions/2); i++)
            {
                RaycastHit info;
                if (Physics.Raycast(transform.position + (transform.right * (i / 10f * normalBrickSize)), transform.up, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.right * (i / 10f * normalBrickSize)), -transform.up, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.up * (i / 10f * normalBrickSize)), transform.forward, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.up * (i / 10f * normalBrickSize)), -transform.forward, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.forward * (i / 10f * normalBrickSize)), transform.right, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.forward * (i / 10f * normalBrickSize)), -transform.right, out info, normalBrickSize))
                {
                    hits.Add(info);
                }
                RaycastHit[] allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), transform.right, brickSize, beamLayer);
                rightHits.AddRange(allHits);
                allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), -transform.right, brickSize, beamLayer);
                leftHits.AddRange(allHits);
                allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), transform.up, brickSize, beamLayer);
                upHits.AddRange(allHits);
                allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), -transform.up, brickSize, beamLayer);
                downHits.AddRange(allHits);
                allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), transform.forward, brickSize, beamLayer);
                forwardHits.AddRange(allHits);
                allHits = Physics.RaycastAll(transform.position + (transform.forward * (i / 10f * normalBrickSize)), -transform.forward, brickSize, beamLayer);
                backwardHits.AddRange(allHits);
            }
            for (int i = 0; i < leftHits.Count; i++)
            {
                if (leftBodies.Contains(leftHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    leftBodies.Add(leftHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(leftHits[i].collider.gameObject);
                }
            }
            for (int i = 0; i < rightHits.Count; i++)
            {
                if (rightBodies.Contains(rightHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    rightBodies.Add(rightHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(rightHits[i].collider.gameObject); ;
                }
            }
            for (int i = 0; i < upHits.Count; i++)
            {
                if (upBodies.Contains(upHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    upBodies.Add(upHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(upHits[i].collider.gameObject);
                }
            }
            for (int i = 0; i < downHits.Count; i++)
            {
                if (downBodies.Contains(downHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    downBodies.Add(downHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(downHits[i].collider.gameObject); ;
                }
            }
            for (int i = 0; i < forwardHits.Count; i++)
            {
                if (forwardBodies.Contains(forwardHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    forwardBodies.Add(forwardHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(forwardHits[i].collider.gameObject);
                }
            }
            for (int i = 0; i < backwardHits.Count; i++)
            {
                if (backwardBodies.Contains(backwardHits[i].collider.GetComponent<Rigidbody>()) == false)
                {
                    backwardBodies.Add(backwardHits[i].collider.GetComponent<Rigidbody>());
                    connectedBodies.Add(backwardHits[i].collider.gameObject); ;
                }
            }
            for (int i = 0; i < hits.Count; i++)
            {
                if (connectedBodies.Contains(hits[i].collider.gameObject) == false)
                {
                    connectedBodies.Add(hits[i].collider.gameObject);
                }
            }
            for (int i = 0; i < connectedBodies.Count; i++)
            {
                if (connectedBodies[i].GetComponent<Brick>())
                {
                    FixedJoint joint = new FixedJoint();
                    joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = connectedBodies[i].GetComponent<Rigidbody>();
                    joint.enableCollision = true;
                    joint.breakForce = breakForce;
                    joint.breakTorque = breakTorque;
                    joint.connectedMassScale = connectionStrength;
                    joint.massScale = massConnectionStrength;
                    joint.enablePreprocessing = true;
                    joints.Add(joint);
                    jointAmount++;
                }
            }
            if (isImpossible == false)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
            }
            if (destructionController != null)
            {
                destructionController.CountJoints(gameObject);
                destructionController.SetInitialJointCount();
            }
        }
        private void OnJointBreak(float breakForce)
        {
            jointAmount--;
            BreakApart();
        }
        new public void OnJointDeath(GameObject damager)
        {
            jointAmount--;
            BreakApart();
            for (int i = 0; i < connectedBodies.Count; i++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(connectedBodies[i].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int ii = 0; ii < joiners.Count; ii++)
                    {
                        if (joiners[ii].connectedBody == rb)
                        {
                            Destroy(joiners[ii]);
                        }
                    }
                }
            }
            damager.SetActive(false);
        }

        private void BreakApart()
        {
            for (int ii = 0; ii < rightBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(rightBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < leftBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == leftBodies[i])
                            {
                                Destroy(joiners[iii]);

                            }
                        }
                        if (joiners[iii].connectedBody == rb)
                        {
                            Destroy(joiners[iii]);
                        }
                    }
                }
            }
            for (int ii = 0; ii < leftBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(leftBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < rightBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == rightBodies[i])
                            {
                                Destroy(joiners[iii]);
                            }
                        }
                    }
                }
            }
            for (int ii = 0; ii < upBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(upBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < downBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == downBodies[i])
                            {
                                Destroy(joiners[iii]);

                            }
                        }
                        if (joiners[iii].connectedBody == rb)
                        {
                            Destroy(joiners[iii]);
                        }
                    }
                }
            }
            for (int ii = 0; ii < downBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(downBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < upBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == upBodies[i])
                            {
                                Destroy(joiners[iii]);
                            }
                        }
                    }
                }
            }
            for (int ii = 0; ii < forwardBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(forwardBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < backwardBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == backwardBodies[i])
                            {
                                Destroy(joiners[iii]);

                            }
                        }
                        if (joiners[iii].connectedBody == rb)
                        {
                            Destroy(joiners[iii]);
                        }
                    }
                }
            }
            for (int ii = 0; ii < backwardBodies.Count; ii++)
            {
                List<FixedJoint> joiners = new List<FixedJoint>();
                joiners.AddRange(backwardBodies[ii].gameObject.GetComponents<FixedJoint>());
                if (joiners.Count > 0)
                {
                    for (int iii = 0; iii < joiners.Count; iii++)
                    {
                        for (int i = 0; i < forwardBodies.Count; i++)
                        {
                            if (joiners[iii].connectedBody == forwardBodies[i])
                            {
                                Destroy(joiners[iii]);
                            }
                        }
                    }
                }
            }
        }
    }
}