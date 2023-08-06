using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fractal.NPC
{
    public class NPCWeapon : MonoBehaviour
    {
        [Header("Gun Info")]
        [SerializeField] GameObject bulletObject;
        float bulletTimer;
        float reactionTimer;

        [HideInInspector] public NPCBehaviour npc;
        [HideInInspector] public NPCStats stats;
        [HideInInspector] BoxCollider2D boxCollider;

        [Header("Gun Stats")]
        public int bulletDamage;
        public float bulletLifetime;
        public float bulletSpeed;
        public float fireRate;

        [Header("Target Info")]
        public LayerMask enemyLayer;
        public KdTree<NPCStats> npcs = new KdTree<NPCStats>();
        public KdTree<NPCStats> enemies = new KdTree<NPCStats>();
        public GameObject targetEnemy;
        public bool enemyIsTargeted;
        public bool enemyRecentlyKilled;

        private void Start()
        {
            stats = GetComponentInParent<NPCStats>();
            npc = GetComponentInParent<NPCBehaviour>();
            boxCollider = GetComponent<BoxCollider2D>();
        }
        private void Update()
        {
            reactionTimer -= Time.deltaTime;
            bulletTimer -= Time.deltaTime;
            if (Vector3.Distance(transform.position, npc.targetPosition) < 0.01f && enemyRecentlyKilled == true)
            {
                enemyRecentlyKilled = false;
            }
            if (reactionTimer <= 0)
            {
                DetectNPC();
                boxCollider = gameObject.GetComponent<BoxCollider2D>();
                Collider2D[] overlap = Physics2D.OverlapAreaAll(boxCollider.bounds.min, boxCollider.bounds.max);
                if (overlap.Length > 1)
                {
                    npc.colliding = true;
                }
                else
                {
                    npc.colliding = false;
                    npc.colliderPositionSet = false;
                }
                reactionTimer = stats.reactionTime;
            }
            if (enemyIsTargeted)
            {
                FireBullet();
            }
        }

        private void FireBullet()
        {
            if (bulletTimer <= 0)
            {
                GameObject thisBullet = Instantiate(bulletObject, this.transform.position, Quaternion.identity);
                thisBullet.GetComponent<Bullet>().weapon = this;
                bulletTimer = fireRate;
            }
        }
        public void DetectNPC()
        {
            enemies.Clear();
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, stats.viewDistance, enemyLayer);
            foreach (Collider2D item in hit)
            {
                if (item.GetComponent<NPCStats>())
                {
                    if (item.gameObject != this.gameObject)
                    {
                        if (item.GetComponent<NPCStats>().npcID != this.stats.npcID)
                        {
                            enemies.Add(item.GetComponent<NPCStats>());
                        }
                    }
                }
            }
            if (enemies.Count >= 1)
            {
                targetEnemy = enemies.FindClosest(transform.position).gameObject;
                enemyIsTargeted = true;
            }
        }
    }
}

