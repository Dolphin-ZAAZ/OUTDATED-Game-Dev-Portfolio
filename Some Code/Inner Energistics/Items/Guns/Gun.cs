using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;
using TMPro;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class Gun : Item
    {
        [Header("Gun Stats")]
        [SerializeField] protected float range;
        protected float baseSpread;
        [SerializeField] protected float currentSpread = 0.05f;
        [SerializeField] [Range(1,5)] protected float focusSpread = 2f;
        [SerializeField] protected float recoil;
        [SerializeField] protected float horizontalRecoilInverse = 100;
        [SerializeField] protected float verticalRecoilInverse = 50;
        [SerializeField] protected float damage;
        [SerializeField] protected float reloadTime;
        [SerializeField] protected float bulletSpeed = 100f;
        [SerializeField] protected float bulletWeight = 1f;
        [SerializeField] protected bool explosiveBullets = false;
        [SerializeField] protected float explosiveForce = 100f;
        [SerializeField] protected float explosionRadius = 10f;
        public float BulletWeight { get { return bulletWeight; } }

        [Header("Config")]
        [SerializeField] protected Transform viewPoint;
        [SerializeField] protected Transform barrelPosition;
        [SerializeField] protected GameObject bullet;
        [SerializeField] protected LayerMask selfLayer;
        [SerializeField] protected LayerMask moveablesLayer;
        protected BulletPool bulletPool;
        protected PlayerMovement playerMovement;

        public Gun(float _damage, float _spread, float _recoil)
        {
            damage = _damage;
            currentSpread = _spread;
            recoil = _recoil;
        }
        private void Awake()
        {
            base.InitializeItem();
            viewPoint = FindObjectOfType<Camera>().transform;

            GameObject bulletPoolObject = new GameObject("Bullet Pool");
            bulletPool = bulletPoolObject.AddComponent<BulletPool>();
            bulletPool.SetBullet(bullet);

            baseSpread = currentSpread;

            playerMovement = FindObjectOfType<PlayerMovement>();
        }
        private void LateUpdate()
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsHitting", false);
        }

        override public void OnUse()
        {
            animator.SetBool("IsAttacking", true);
            Fire();
        }
        public override void Focus()
        {
            currentSpread = baseSpread / focusSpread;
        }
        public override void Equiped()
        {
            currentSpread = baseSpread;
        }
        public Vector3 ApplySpread()
        {
            Vector3 bulletDirection = viewPoint.transform.forward;
            bulletDirection += new Vector3(Random.Range(-currentSpread, currentSpread),Random.Range(-currentSpread, currentSpread), Random.Range(-currentSpread, currentSpread));
            return bulletDirection.normalized;
        }
        public void Fire()
        {
            RaycastHit hit;
            Vector3 spread = ApplySpread();
            if (Physics.Raycast(viewPoint.transform.position, spread, out hit, range, ~selfLayer))
            {
                Rigidbody targetRB;
                bulletPool.SpawnNextBullet(barrelPosition.position, hit.point, bulletSpeed, damage);
                bulletPool.SpawnNextDecal(hit.point, hit.collider.transform, hit.normal);
                IDamageable target;
                if (hit.transform.TryGetComponent(out target))
                {
                    target.Damage(damage, this.gameObject);
                    if (hit.collider.gameObject.TryGetComponent(out targetRB))
                    {
                        if (explosiveBullets == false)
                        {
                            targetRB.AddForceAtPosition(spread * bulletWeight, hit.point, ForceMode.Impulse);
                        }
                        if (explosiveBullets == true)
                        {
                            targetRB.AddExplosionForce(explosiveForce * 1000f, hit.point, explosionRadius);
                        }
                    }
                    animator.Play("Hit Marker", 1);
                    animator.SetBool("IsHitting", true);
                }
                else if (explosiveBullets == true)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(hit.point, explosionRadius, spread, explosionRadius, moveablesLayer);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        hits[i].collider.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce * 1000f, hit.point, explosionRadius);
                    }
                }
                playerMovement.MoveMouse(-(recoil / verticalRecoilInverse), Random.Range(-(recoil / horizontalRecoilInverse), (recoil / horizontalRecoilInverse)));
            }
        }
    }
}