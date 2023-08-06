using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fractal.NPC
{
    public class NPCBehaviour : MonoBehaviour
    {
        NPCStats stats;
        NPCWeapon weapon;

        [HideInInspector] public GameObject targetPickup;

        [HideInInspector] public KdTree<Blob> blobs = new KdTree<Blob>();

        [HideInInspector] public Vector3 targetPosition;

        public bool colliding = false;
        public bool colliderPositionSet = false;
        Vector2 colliderPosition = new Vector2();

        public float detectBackupTimeMax = 4f;
        public float detectBackupTimeMin = 2f;
        float detectBackupTime = 2f;
        float detectBackupTimer = 2f;
        float birthStray = 2f;
        public bool birthed = false;
        bool searching = false;

        public float reproductionTimer = 0f;


        [Header("Layers")]
        public LayerMask pickupLayer;

        private void Start()
        {
            stats = GetComponent<NPCStats>();
            weapon = GetComponent<NPCWeapon>();
            if (stats.isChild)
            {
                targetPosition = new Vector3(0, 0, 0);
                colliding = true;
                birthStray = 2f;
                birthed = true;
            }
            else
            {
                DetectBlob();
                detectBackupTime = Random.Range(detectBackupTimeMin, detectBackupTimeMax);
                birthed = false;
            }
        }
        private void Update()
        {
            float step = stats.movementSpeed * Time.deltaTime;
            detectBackupTimer -= Time.deltaTime;
            reproductionTimer -= Time.deltaTime;
            if (detectBackupTimer <= 0f)
            {
                DetectBlob();
                detectBackupTimer = detectBackupTime;
            }
            if (birthStray > 0f)
            {
                birthStray -= Time.deltaTime;
                if (birthStray <= 0f)
                {
                    birthed = true;
                }
            }
            else if (birthed == true)
            {
                DetectBlob();
                weapon.DetectNPC();
                birthed = false;
            }
            if (colliding)
            {
                if (reproductionTimer <= 0)
                {
                    if (colliderPositionSet == false)
                    {
                        Vector2 tempRandom = Random.insideUnitCircle.normalized * stats.viewDistance;
                        Vector3 tempRandom3D = tempRandom;
                        colliderPosition = this.transform.position + tempRandom3D;
                        colliderPositionSet = true;
                    }
                    else if (Vector2.Distance(this.transform.position, colliderPosition) < 1f)
                    {
                        Vector2 tempRandom = Random.insideUnitCircle.normalized * stats.viewDistance;
                        Vector3 tempRandom3D = tempRandom;
                        colliderPosition = this.transform.position + tempRandom3D;
                        colliderPositionSet = true;
                    }
                    targetPosition = colliderPosition;
                }
            }
            if (weapon.enemyIsTargeted)
            {
                if (weapon.targetEnemy != null)
                {
                    if (Vector2.Distance(transform.position, weapon.targetEnemy.transform.position) > stats.combatDistance)
                    {
                        targetPosition = weapon.targetEnemy.transform.position;
                    }
                    else if (Vector2.Distance(transform.position, weapon.targetEnemy.transform.position) < stats.combatDistance - 2f)
                    {
                        Vector2 enemyDirection = new Vector2(weapon.targetEnemy.transform.position.x, weapon.targetEnemy.transform.position.y)
                              + (new Vector2(weapon.targetEnemy.transform.position.x, weapon.targetEnemy.transform.position.y)
                               - new Vector2(transform.position.x, transform.position.y)).normalized * -1 * weapon.stats.viewDistance;
                        Vector3 v3Direction = enemyDirection;
                        targetPosition = v3Direction;
                    }
                }
            }
            this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (stats.eatenBlob == true || weapon.enemyIsTargeted == false) 
            {
                DetectBlob();
                weapon.DetectNPC();
            }
        }
        
        void DetectBlob()
        {
            blobs.Clear();
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 20, pickupLayer);
            if (hit == null)
            {
                hit = Physics2D.OverlapCircleAll(transform.position, stats.viewDistance, pickupLayer);
            }
            foreach (Collider2D item in hit)
            {
                if (item.GetComponent<Blob>())
                {
                    blobs.Add(item.GetComponent<Blob>());
                    targetPickup = blobs.FindClosest(transform.position).gameObject;
                    if (weapon.enemyRecentlyKilled == false)
                    {
                        targetPosition = targetPickup.transform.position;
                    }
                    searching = false;
                    stats.eatenBlob = false;
                }
            }
            if (targetPickup == null)
            {
                searching = true;
            }
            if (searching == true)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 1f)
                {
                    targetPosition = transform.position + new Vector3(Random.insideUnitCircle.x * stats.viewDistance, Random.insideUnitCircle.y * stats.viewDistance, 1f);
                    if (Vector3.Distance(targetPosition, new Vector3 (0,0,0)) > 500f)
                    {
                        targetPosition = transform.position + new Vector3(Random.insideUnitCircle.x * stats.viewDistance, Random.insideUnitCircle.y * stats.viewDistance, 1f);
                    }
                }
            }
        }
    }
}

