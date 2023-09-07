using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float enemySpeed = 5f;
    [SerializeField] public int enemyHealth = 100;
    [SerializeField] public int killMoney = 5;
    [SerializeField] public int attackDamage = 5;
    [SerializeField] float attackTime = 0.5f;
    [SerializeField] float attackTimer = 0.5f;
    float lightTimer;

    public GameObject hitSound;
    public GameObject explosionSound;
    public GameObject explosionParticles;
    public GameObject hitParticle;
    public AudioSource carSound;
    public ParticleSystem shockParticles;
    public AudioSource shock;

    [SerializeField] GameObject shockLight;

    bool rightActivate;
    int rightDamage = 150;
    int incenDamage = 20;
    float incenTick = 0.5f;

    bool shockPlayed = false;
    bool shockParticlesPlayed = false;

    bool cashRuneActive = false;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemyHealth;
        enemySpeed = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed;
        attackDamage = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalAttackDamage;
        killMoney = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalKillMoney;
        rightDamage = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalRightDamage;
        incenDamage = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalIncenDamage;
        shockParticles = this.gameObject.GetComponentInChildren<ParticleSystem>();
        carSound = GetComponent<AudioSource>();

        shockLight.SetActive(false);

        attackTimer = 0.001f;

        rightActivate = false;
    }

    private void Update()
    {
        lightTimer -= Time.deltaTime;
        incenTick -= Time.deltaTime;
        rightActivate = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalRightActivate;
        cashRuneActive = GameObject.Find("SpawningSystem").GetComponent<Spawning>().cashRuneActive;
        if (cashRuneActive)
        {
            killMoney = Mathf.FloorToInt(GameObject.Find("SpawningSystem").GetComponent<Spawning>().cashRuneIncrease * GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalKillMoney);
        }
        else
        {
            killMoney = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalKillMoney;
        }
        if (enemyHealth <= 0)
        {
            if (GameObject.FindGameObjectsWithTag("ExplosionSound").Length <= 5)
            {
                Instantiate(explosionSound);
            }
            Instantiate(explosionParticles, transform.position, transform.rotation);
            GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().killCount += 1;
            GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().money += killMoney;
            GameObject.Find("SpawningSystem").GetComponent<Spawning>().enemiesRemainingCountdown--;
            Destroy(this.gameObject);
        }

        if (GameObject.Find("Canvas").GetComponent<UserInterface>().dead || GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            carSound.Pause();
            shock.Pause();
        }
        else
        {
            carSound.UnPause();
            shock.UnPause();
        }
    }

    public void DoDamage(int damageAmount)
    {
        enemyHealth -= damageAmount;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().playerHealth -= attackDamage;
                attackTimer = attackTime;
            }

            if (!shockPlayed)
            {
                shock.Play();
                shockPlayed = true;
            }

            if (!shockParticlesPlayed)
            {
                shockParticles.Play();
                shockParticlesPlayed = true;
            }
            if (lightTimer < 0f)
            {
                shockLight.SetActive(!shockLight.activeSelf);
                lightTimer = 0.05f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            shock.Stop();
            shockPlayed = false;
            shockParticles.Stop();
            shockParticlesPlayed = false;
            shockLight.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            DoDamage(GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalBulletDamage);
            /*
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            */
            Instantiate(hitSound);
            Instantiate(hitParticle, collision.transform.position, new Quaternion(collision.transform.rotation.x, collision.transform.rotation.y, collision.transform.rotation.z - 180, 1));
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("OutwardsBlast"))
        {
            if (Input.GetButtonDown("Right Click Ability") && !rightActivate)
            {
                DoDamage(rightDamage);
                Instantiate(hitSound);
            }
        }
        if (collision.CompareTag("Fire"))
        {
            if (incenTick < 0)
            {
                DoDamage(incenDamage);
                Instantiate(hitSound);
                incenTick = 0.5f;
            }
        }
    }
}
