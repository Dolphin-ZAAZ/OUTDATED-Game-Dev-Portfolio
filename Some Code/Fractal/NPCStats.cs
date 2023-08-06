using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Fractal.NPC
{
    public class NPCStats : MonoBehaviour
    {
        NPCBehaviour npc;
        NPCWeapon weapon;
        OverlayHud hud;

        [Header("Info")]
        public bool isChild = false;
        public int npcID;
        public Color colorID;
        public LayerMask enemyLayer;
        int blobsEatenCount = 0;

        Spawner spawner;

        [Header("NPC Stats")]
        public int health = 10;
        public int size = 10;
        public float reactionTime = 0.5f;
        public float movementSpeed = 1250f;
        public float viewDistance = 50f;
        public float combatDistance;

        public int healthIncrement = 1;
        public int sizeIncrement = 1;
        public int reproductionCeiling = 100;
        public int reproductionRange = 10;
        public int upgradeCeiling = 20;
        public int upgradeRange = 2;

        [Header("Text")]
        public TMP_Text textHealth;
        public TMP_Text textSize;

        [Header("Upgrade Stats")]
        [SerializeField] List<string> upgradeHistory = new List<string>();
        public float speedUpgradeMin = 0.02f;
        public float speedUpgradeMax = 0.06f;
        public int attackDamageUpgradeMin = 1;
        public int attackDamageUpgradeMax = 2;
        public float attackSpeedUpgradeMin = 0.10f;
        public float attackSpeedUpgradeMax = 0.20f;
        public float reactionTimeUpgradeMin = 0.05f;
        public float reactionTimeUpgradeMax = 0.10f;
        public float reproductionRateUpgradeMin = 0.05f;
        public float reproductionRateUpgradeMax = 0.10f;
        public float viewDistanceUpgradeMin = 0.05f;
        public float viewDistanceUpgradeMax = 0.10f;
        public float attackRangeUpgradeMin = 0.03f;
        public float attackRangeUpgradeMax = 0.06f;
        public float upgradeRateUpgradeMin = 0.06f;
        public float upgradeRateUpgradeMax = 0.09f;

        float speedBase;
        int attackDamageBase;
        float attackSpeedBase;
        float reactionTimeBase;
        float reproductionBase;
        float viewDistanceBase;
        float attackRangeBase;
        float upgradeRateBase;

        [HideInInspector] public bool eatenBlob = false;
        bool shouldReproduce;
        float outOfBoundsTimer = 10f;
        public float outOfBoundsTime = 10f;
        UpgradeType upgradeType;

        private void Start()
        {
            spawner = FindObjectOfType<Spawner>();
            npc = GetComponent<NPCBehaviour>();
            weapon = GetComponent<NPCWeapon>();
            upgradeCeiling = 10;
            if (isChild == false)
            {
                //upgradeCeiling = Random.Range(upgradeCeiling - upgradeRange, upgradeCeiling + upgradeRange);
                reproductionCeiling = Random.Range(reproductionCeiling - reproductionRange, reproductionCeiling + reproductionRange);
                colorID = Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.5f, 1f, 1f, 1f);
                GetComponentInChildren<SpriteRenderer>().color = colorID;
                speedBase = movementSpeed;
                attackDamageBase = weapon.bulletDamage;
                attackSpeedBase = weapon.fireRate;
                reactionTimeBase = reactionTime;
                reproductionBase = reproductionCeiling;
                viewDistanceBase = viewDistance;
                attackRangeBase = combatDistance;
                upgradeRateBase = upgradeCeiling;
            }
            hud = GameObject.Find("Overlay").GetComponent<OverlayHud>();
            hud.UpdateSpeciesRank();
        }
        private void Update()
        {
            if (Vector3.Distance(this.transform.position, spawner.transform.position) > spawner.playFieldSize / 2)
            {
                outOfBoundsTimer -= Time.deltaTime;
                if (outOfBoundsTimer <= 0f)
                {
                    spawner.speciesRank[npcID] -= 1;
                    if (spawner.speciesRank[npcID] <= 0)
                    {
                        spawner.speciesRank.Remove(npcID);
                    }
                    spawner.npcAmount -= 1;
                    hud.UpdateSpeciesCount();
                    hud.UpdateSpeciesRank();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                outOfBoundsTimer = outOfBoundsTime;
            }
            if (size >= reproductionCeiling)
            {
                Reproduce();
            }
            if (blobsEatenCount >= upgradeCeiling)
            {
                Upgrade();
                blobsEatenCount = 0;
            }
            if (health <= 0)
            {
                for (int i = 0; i < size; i++)
                {
                    Vector2 blobSpawnPosition = new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle * (size / 10);
                    Instantiate(spawner.blobObject, new Vector3 (blobSpawnPosition.x, blobSpawnPosition.y, 1), Quaternion.identity);
                    Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, viewDistance, enemyLayer);
                    for (int iI = 0; iI < hit.Length; iI++)
                    {
                        if (hit[iI].GetComponent<NPCWeapon>())
                        {
                            hit[iI].GetComponent<NPCWeapon>().enemyIsTargeted = false;
                        }
                    }
                }
                spawner.speciesRank[npcID] -= 1;
                if (spawner.speciesRank[npcID] <= 0)
                {
                    spawner.speciesRank.Remove(npcID);
                    hud.UpdateSpeciesCount();
                    hud.UpdateSpeciesRank();
                }
                spawner.npcAmount -= 1;
                hud.UpdateCellCount();
                hud.UpdateSpeciesRank();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<Blob>() && npc.birthed == false && npc.reproductionTimer <= 0)
            {
                Blob currentBlob = collider.gameObject.GetComponent<Blob>();
                blobsEatenCount++;
                size++;
                health++;
                ScaleUpdate();
                eatenBlob = true;
                Destroy(currentBlob.gameObject);
            }
        }

        public void ScaleUpdate()
        {
            textSize.text = size.ToString();
            textHealth.text = health.ToString();
            transform.localScale = new Vector3(size / 10f, size / 10f, 1f);
        }
        public void Reproduce()
        {
            size = Mathf.FloorToInt(size / 2f);
            health = size;
            ScaleUpdate();
            GameObject childNPC = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            childNPC.GetComponent<NPCStats>().isChild = true;
            spawner.npcAmount++;
            spawner.speciesRank[npcID] += 1;
            hud.UpdateCellCount();
            hud.UpdateSpeciesRank();
            npc.reproductionTimer = 2.1f;
        }

        public void Upgrade()
        {
            upgradeType = (UpgradeType)Random.Range(0, System.Enum.GetValues(typeof(UpgradeType)).Length);
            switch (upgradeType)
            {
                case UpgradeType.Speed:
                    movementSpeed = movementSpeed + speedBase * Random.Range(speedUpgradeMin, speedUpgradeMax);
                    upgradeHistory.Add("Movement Speed");
                    break;
                case UpgradeType.AttackDamage:
                    weapon.bulletDamage = weapon.bulletDamage + attackDamageBase * Random.Range(attackDamageUpgradeMin, attackDamageUpgradeMax);
                    upgradeHistory.Add("Attack Damage");
                    break;
                case UpgradeType.AttackSpeed:
                    weapon.fireRate = weapon.fireRate - weapon.fireRate * Random.Range(attackSpeedUpgradeMin, attackSpeedUpgradeMax);
                    upgradeHistory.Add("Attack Speed");
                    break;
                //case UpgradeType.ReactionTime:
                //    reactionTime = reactionTime + reactionTimeBase * Random.Range(reactionTimeUpgradeMin, reactionTimeUpgradeMax);
                //    upgradeHistory.Add("Reaction Time");
                //    break;
                //case UpgradeType.ReproductionRate:
                //    if ((float)reproductionCeiling - reproductionBase * Random.Range(reproductionRateUpgradeMin, reproductionRateUpgradeMax) < reproductionCeiling - 1)
                //    {
                //        reproductionCeiling = Mathf.RoundToInt((float)reproductionCeiling - reproductionBase * Random.Range(reproductionRateUpgradeMin, reproductionRateUpgradeMax));
                //    }
                //    else
                //    {
                //        reproductionBase = reproductionBase * Random.Range(reproductionRateUpgradeMin, reproductionRateUpgradeMax);
                //    }
                //    upgradeHistory.Add("Reproduction Rate");
                //    break;
                //case UpgradeType.ViewDistance:
                //    viewDistance = viewDistance + viewDistanceBase * Random.Range(viewDistanceUpgradeMin, viewDistanceUpgradeMax);
                //    upgradeHistory.Add("View Distance");
                //    break;
                //case UpgradeType.AttackRange:
                //    combatDistance = combatDistance + attackRangeBase * Random.Range(attackRangeUpgradeMin, attackRangeUpgradeMax);
                //    upgradeHistory.Add("Attack Range");
                //    break;
                //case UpgradeType.UpgradeRate:
                //    if ((float)upgradeCeiling - upgradeRateBase * Random.Range(upgradeRateUpgradeMin, upgradeRateUpgradeMax) < upgradeCeiling - 1)
                //    {
                //        upgradeCeiling = Mathf.RoundToInt((float)upgradeCeiling - upgradeRateBase * Random.Range(upgradeRateUpgradeMin, upgradeRateUpgradeMax));
                //    }
                //    else
                //    {
                //        upgradeRateBase = upgradeRateBase * Random.Range(upgradeRateUpgradeMin, upgradeRateUpgradeMax);
                //    }
                //    upgradeHistory.Add("Upgrade Rate");
                //    break;
            }
        }
    }
    public enum UpgradeType
    {
        Speed, // 2-6% linear
        AttackDamage, // 2-6% linear
        AttackSpeed, // x5-10% linear
        //ReactionTime, // x5-10% linear
        //ReproductionRate, // x2-3% linear
        //ViewDistance, // x5-10% linear
        //AttackRange, // x3-6% linear
        //UpgradeRate // x2-3% linear
    }
}

