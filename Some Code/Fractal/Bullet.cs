using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fractal.NPC
{
    public class Bullet : MonoBehaviour
    {
        public NPCWeapon weapon;

        Vector2 targetPosition;

        float bulletSpeed;
        float bulletLifetime;

        [Header("Bullet Info")]
        public int bulletDamage;
        public GameObject motherGun;

        void Start()
        {
            bulletLifetime = weapon.bulletLifetime;
            bulletSpeed = weapon.bulletSpeed;
            if(weapon.targetEnemy != null)
            {
                targetPosition = new Vector2(weapon.targetEnemy.transform.position.x, weapon.targetEnemy.transform.position.y)
                              + (new Vector2(weapon.targetEnemy.transform.position.x, weapon.targetEnemy.transform.position.y) 
                               - new Vector2(transform.position.x, transform.position.y)).normalized * weapon.stats.viewDistance;
            }
            bulletDamage = weapon.bulletDamage;
        }
        void Update()
        {
            bulletLifetime -= Time.deltaTime;
            float step = bulletSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (bulletLifetime < 0 || Vector3.Distance(this.transform.position, targetPosition) <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<NPCStats>() && collision.GetComponent<NPCBehaviour>().birthed == false)
            {
                if (collision.GetComponent<NPCStats>().npcID != weapon.stats.npcID)
                {
                    if ((collision.gameObject.GetComponent<NPCStats>().health - bulletDamage) <= 0 )
                    {
                        weapon.enemyIsTargeted = false;
                        weapon.enemyRecentlyKilled = true;
                        weapon.npc.targetPosition = collision.transform.position;
                    }
                    collision.gameObject.GetComponent<NPCStats>().health -= bulletDamage;
                    collision.gameObject.GetComponent<NPCStats>().ScaleUpdate();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}

