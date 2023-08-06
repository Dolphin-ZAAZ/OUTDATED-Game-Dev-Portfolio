using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;
using TMPro;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class FullyAutomaticGun : Gun
    {
        [Header("Gun Stats")]
        [SerializeField] protected float rateOfFire;
        float fireRateDelta;
        float attackDelta;

        public FullyAutomaticGun(float _damage, float _rateOfFire, float _spread, float _recoil) : base(_damage, _spread, _recoil)
        {
            damage = _damage;
            currentSpread = _spread;
            recoil = _recoil;
            rateOfFire = _rateOfFire;
        }
        private void Start()
        {
            fireRateDelta = 60 / rateOfFire;
            attackDelta = -1f;
        }
        public void ResetAttackDelta()
        {
            attackDelta = -1f;
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsHitting", false);
        }
        override public void OnUse()
        {
            ResetTriggers();
            isUsing = true;
        }
        public void AutomaticFire()
        {
            attackDelta -= Time.deltaTime;
            if (attackDelta <= 0f && attackDelta != -1f)
            {
                attackDelta = fireRateDelta;
                animator.SetBool("IsAttacking", true);
                Fire();
            }
            else if (attackDelta != -1f)
            {
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsHitting", false);
            }
        }
        new public void Fire()
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