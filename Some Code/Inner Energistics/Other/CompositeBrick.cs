using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Energistics.Behaviour
{
    public class CompositeBrick : Brick
    {
        [SerializeField] List<Rigidbody> children = new List<Rigidbody>();
        [SerializeField] float childMass = 1f;

        float deathTimer = 5f;
        bool shouldDie = false;
        private void Awake()
        {
            children.AddRange(GetComponentsInChildren<Rigidbody>());
            for (int i = 0; i < children.Count; i++)
            {
                children[i].detectCollisions = false;
                children[i].constraints = RigidbodyConstraints.None;
                children[i].useGravity = false;
                children[i].mass = mass;
                children[i].drag = drag;
                children[i].angularDrag = angularDrag;
                children[i].collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
            brickSize = normalBrickSize;
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
                brickSize = normalBrickSize * reinforcementSize;
            }
        }
        void Start()
        {

            List<RaycastHit> hits = new List<RaycastHit>();
            RaycastHit info;
            for (int i = -(brickSubdivisions / 2); i < brickSubdivisions - (brickSubdivisions / 2); i++)
            {
                if (Physics.Raycast(transform.position + (transform.right * (i / 10f * normalBrickSize)), transform.up, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.right * (i / 10f * normalBrickSize)), -transform.up, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.up * (i / 10f * normalBrickSize)), transform.forward, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.up * (i / 10f * normalBrickSize)), -transform.forward, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.forward * (i / 10f * normalBrickSize)), transform.right, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.forward * (i / 10f * normalBrickSize)), -transform.right, out info, brickSize))
                {
                    hits.Add(info);
                }
            }
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].transform.parent == transform)
                {
                    continue;
                }
                if (hits[i].transform == transform)
                {
                    continue;
                }
                if (connectedBodies.Contains(hits[i].collider.gameObject))
                {
                    continue;
                }
                else
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
                else
                {
                    connectedBodies.Remove(connectedBodies[i]);
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
            for (int i = 0; i < children.Count; i++)
            {
                children[i].constraints = RigidbodyConstraints.None;
            }
        }
        new public void OnJointDeath(GameObject damager)
        {
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
            for (int i = 0; i < children.Count; i++)
            {
                children[i].transform.parent = null;
                children[i].constraints = RigidbodyConstraints.None;
                children[i].detectCollisions = true;
                children[i].useGravity = true;
            }
            rb.detectCollisions = false;
            damager.SetActive(false);
        }
        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody colliderRB;
            if (collision.gameObject.TryGetComponent(out colliderRB))
            {
                float velocityMagnitude = rb.GetRelativePointVelocity(colliderRB.velocity).magnitude;
                ApplyDamage(velocityMagnitude);
            }
            else
            {
                float velocityMagnitude = rb.velocity.magnitude;
                ApplyDamage(velocityMagnitude);
            }
        }
    }
}
