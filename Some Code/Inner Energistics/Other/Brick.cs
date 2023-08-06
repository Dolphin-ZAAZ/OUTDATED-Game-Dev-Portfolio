using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Energistics.Behaviour
{
    public enum BrickType
    {
        Wood,
        Brick,
        ReinforcedBrick,
        Alloy,
        SuperAlloy,
        ImpossibleMaterial
    }
    public class Brick : MonoBehaviour, IForceDamageable
    {
        [SerializeField] protected bool isReinforced = false;
        [SerializeField] protected float reinforcementSize = 3f;
        [SerializeField] protected int brickSubdivisions = 3;
        [SerializeField] protected float normalBrickSize = 1f;
        [SerializeField] protected BrickType brickType = BrickType.Wood;
        [SerializeField] protected float breakForce = 50f;
        [SerializeField] protected float breakTorque = 50f;
        [SerializeField] protected float connectionStrength = 1.5f;
        [SerializeField] protected float massConnectionStrength = 1.5f;
        [SerializeField] protected float mass = 1f;
        [SerializeField] protected float brickSize = 0.75f;
        [SerializeField] protected float drag = 1;
        [SerializeField] protected float angularDrag = 0.2f;
        [SerializeField] protected int jointAmount = 0;
        public int JointAmount { get { return jointAmount; } }
        [SerializeField] protected float deleteTime = 2f;
        [SerializeField] protected bool isImpossible = false;
        [SerializeField] protected float forceDamageMultiplier;
        protected List<FixedJoint> joints = new List<FixedJoint>();
        protected List<GameObject> connectedBodies = new List<GameObject>();
        protected Rigidbody rb;
        protected DefaultDamagable damagable;
        protected DestructionController destructionController;
        float deathTimer = 5f;
        bool shouldDie = false;

        public float ForceDamageMultiplier { get { return forceDamageMultiplier; } }
        private void Awake()
        {
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
            List<RaycastHit> leftHits = new List<RaycastHit>();
            List<RaycastHit> rightHits = new List<RaycastHit>();
            RaycastHit info;
            for (int i = -(brickSubdivisions/2); i < brickSubdivisions - (brickSubdivisions/2); i++)
            {
                if (Physics.Raycast(transform.position + (transform.right *( i / 10f * normalBrickSize)), transform.up, out info, brickSize))
                {
                    hits.Add(info);
                }
                if (Physics.Raycast(transform.position + (transform.right * (i/ 10f * normalBrickSize)), -transform.up, out info, brickSize))
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
        public void OnJointDeath(GameObject damager)
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
        public float ApplyDamage(float velocity)
        {
            damagable.Damage(velocity * forceDamageMultiplier, gameObject);
            return velocity;
        }
    }
}
