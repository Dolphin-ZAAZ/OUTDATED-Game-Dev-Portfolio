using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    AudioSource buySound;
    AudioSource pickUp;
    AudioSource invincePickUp;
    AudioSource instaPickUp;
    AudioSource slowPickUp;

    [Header("Balancing")]
    [SerializeField] int medkitIncrease = 75;
    [SerializeField] int healthIncrease = 75;
    [SerializeField] int damageIncrease = 4;
    [SerializeField] public float sprintIncrease = 3f;
    [SerializeField] public float sprintMax = 102f;
    [SerializeField] public int globalPriceIncrease = 50;

    [Header("Runes")]
    [SerializeField] public bool instaActive = false;
    [SerializeField] float instaCountdown = 10f;
    [SerializeField] float instaTimer = 10f;
    [SerializeField] int previousDamage;
    [SerializeField] bool previousDamageSet;
    [SerializeField] Text instaText;
    [SerializeField] Image instaIcon;

    [SerializeField] public bool invincibilityActive = false;
    [SerializeField] float invincibilityCountdown = 10f;
    [SerializeField] float invincibilityTimer = 10f;
    [SerializeField] int previousHealth;
    [SerializeField] bool previousHealthSet;
    [SerializeField] Text invincibilityText;
    [SerializeField] Image invincibilityIcon;

    [SerializeField] public bool slowActive = false;
    [SerializeField] float slowCountdown = 10f;
    [SerializeField] float slowTimer = 10f;
    [SerializeField] float previousSlow;
    [SerializeField] bool previousSlowSet;
    [SerializeField] bool slowSet;
    [SerializeField] float previousSpeed;
    [SerializeField] bool previousSpeedSet;
    [SerializeField] bool speedSet;
    [SerializeField] Text slowText;
    [SerializeField] Image slowIcon;

    [Header("Shop Prices and UI")]
    [SerializeField] public int medKitPrice = 200;
    [SerializeField] public Button medKitButton;
    [SerializeField] public int healthBuffPrice = 200;
    [SerializeField] public int healthBuffPriceIncrease = 200;
    [SerializeField] public Button healthBuffButton;
    [SerializeField] public int damageBuffPrice = 200;
    [SerializeField] public int damageBuffPriceIncrease = 200;
    [SerializeField] public Button damageBuffButton;
    [SerializeField] public int sprintBuffPrice = 200;
    [SerializeField] public int sprintBuffPriceIncrease = 200;
    [SerializeField] public Button sprintBuffButton;

    [Header("Player Stats")]
    [SerializeField] public int playerHealth = 100;
    [SerializeField] public int maxPlayerHealth = 100;
    [SerializeField] public int killCount = 0;
    [SerializeField] public int money = 0;

    [Header("Buy Menu Logic")]
    public bool buyMenuActive;
    public bool startNextWave;

    [Header("Drag and Drop")]
    public ParticleSystem deathParticles;
    public AudioSource deathAudio;

    // Start is called before the first frame update
    void Start()
    {
        buySound = GameObject.Find("BuyMenuSound").GetComponent<AudioSource>();
        pickUp = GameObject.Find("RunePickupSound").GetComponent<AudioSource>();
        invincePickUp = GameObject.Find("InvincePickUp").GetComponent<AudioSource>();
        instaPickUp = GameObject.Find("InstaPickUp").GetComponent<AudioSource>();
        slowPickUp = GameObject.Find("SlowPickUp").GetComponent<AudioSource>();

        buyMenuActive = false;
        startNextWave = false;

        medKitButton = GameObject.Find("MedKit").GetComponent<Button>();
        damageBuffButton = GameObject.Find("DamageBuff").GetComponent<Button>();
        healthBuffButton = GameObject.Find("HealthBuff").GetComponent<Button>();
        sprintBuffButton = GameObject.Find("SprintBuff").GetComponent<Button>();

        instaIcon = GameObject.Find("InstakillIcon").GetComponent<Image>();
        instaIcon.color = new Color(1f, 1f, 1f, 0.2f);

        instaText = GameObject.Find("InstakillText").GetComponent<Text>();
        instaText.color = new Color(1f, 1f, 1f, 0.5f);
        instaText.text = "0s";
        instaTimer = instaCountdown;

        invincibilityIcon = GameObject.Find("InvincibilityIcon").GetComponent<Image>();
        invincibilityIcon.color = new Color(1f, 1f, 1f, 0.2f);

        invincibilityText = GameObject.Find("InvincibilityText").GetComponent<Text>();
        invincibilityText.color = new Color(1f, 1f, 1f, 0.5f);
        invincibilityText.text = "0s";
        invincibilityTimer = invincibilityCountdown;

        slowIcon = GameObject.Find("SlowIcon").GetComponent<Image>();
        slowIcon.color = new Color(1f, 1f, 1f, 0.2f);

        slowText = GameObject.Find("SlowText").GetComponent<Text>();
        slowText.color = new Color(1f, 1f, 1f, 0.5f);
        slowText.text = "0s";
        slowTimer = slowCountdown;
        slowSet = false;

        GameObject.Find("MedKitText").GetComponent<Text>().text = "Med Kit(" + medkitIncrease + "HP): $" + medKitPrice;
        GameObject.Find("HealthBuffText").GetComponent<Text>().text = "Health Buff: $" + healthBuffPrice;
        GameObject.Find("DamageText").GetComponent<Text>().text = "Damage Buff: $" + damageBuffPrice;
        GameObject.Find("SprintText").GetComponent<Text>().text = "Sprint Buff: $" + sprintBuffPrice;

        GameObject.Find("Canvas").GetComponent<UserInterface>().buyMenu.SetActive(false);
        Time.timeScale = 1f;
        GameObject.Find("Canvas").GetComponent<UserInterface>().paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerDeath
        if (playerHealth <= 0)
        {
            PlayerDeath();
        }

        //MedKit
        if (money >= medKitPrice && playerHealth != maxPlayerHealth)
        {
            medKitButton.interactable = true;
        }
        else
        {
            medKitButton.interactable = false;
        }

        //DamageBuff
        if (money >= damageBuffPrice)
        {
            damageBuffButton.interactable = true;
        }
        else
        {
            damageBuffButton.interactable = false;
        }

        //HealthBuff
        if (money >= healthBuffPrice)
        {
            healthBuffButton.interactable = true;
        }
        else
        {
            healthBuffButton.interactable = false;
        }

        //SprintBuff
        if (money >= sprintBuffPrice)
        {
            if (GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed < sprintMax)
            {
                sprintBuffButton.interactable = true;
            }
            else
            {
                sprintBuffButton.interactable = false;
                GameObject.Find("SprintText").GetComponent<Text>().text = "Sprint Buff: MAX";
            }
        }
        else
        {
            sprintBuffButton.interactable = false;
        }

        //Insta-kill
        if (instaActive)
        {
            instaText.color = new Color(1f, 1f, 1f, 1f);
            instaText.text = "" + Mathf.FloorToInt(instaTimer + 1) + "s";
            float period = 0.75f;
            float cycles = Time.time / period;
            if (instaTimer < 5f)
            {
                instaIcon.color = new Color(1f, 1f, 1f, ((Mathf.Sin(cycles * (Mathf.PI * 2)) + 1f) / 2.5f) + 0.2f);
            }
            else
            {
                instaIcon.color = new Color(1f, 1f, 1f, 1f);
            }
            instaTimer -= Time.deltaTime;
            if (!previousDamageSet)
            {
                pickUp.Play();
                instaPickUp.Play();
                previousDamage = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalBulletDamage;
                previousDamageSet = true;
            }
            GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalBulletDamage = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemyHealth;
            if (instaTimer <= 0f || GameObject.Find("SpawningSystem").GetComponent<Spawning>().waveComplete)
            {
                instaActive = false;
                instaIcon.color = new Color(1f, 1f, 1f, 0.2f);
                instaText.color = new Color(1f, 1f, 1f, 0.5f);
                instaText.text = "0s";
                instaTimer = instaCountdown;
                GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalBulletDamage = previousDamage;
                previousDamageSet = false;
            }
        }
        //Invincibility
        if (invincibilityActive)
        {
            invincibilityText.color = new Color(1f, 1f, 1f, 1f);
            invincibilityText.text = "" + Mathf.FloorToInt(invincibilityTimer + 1) + "s";
            invincibilityIcon.color = new Color(1f, 1f, 1f, 1f);
            float period = 0.75f;
            float cycles = Time.time / period;
            if (invincibilityTimer < 5f)
            {
                invincibilityIcon.color = new Color(1f, 1f, 1f, ((Mathf.Sin(cycles * (Mathf.PI * 2)) + 1f) / 2.5f) + 0.2f);
            }
            else
            {
                invincibilityIcon.color = new Color(1f, 1f, 1f, 1f);
            }
            invincibilityTimer -= Time.deltaTime;
            if (!previousHealthSet)
            {
                pickUp.Play();
                invincePickUp.Play();
                previousHealth = playerHealth;
                previousHealthSet = true;
            }
            playerHealth = Mathf.FloorToInt(Mathf.Infinity) - 500;
            if (invincibilityTimer <= 0f || GameObject.Find("SpawningSystem").GetComponent<Spawning>().waveComplete)
            {
                invincibilityActive = false;
                invincibilityIcon.color = new Color(1f, 1f, 1f, 0.2f);
                invincibilityText.color = new Color(1f, 1f, 1f, 0.5f);
                invincibilityText.text = "0s";
                invincibilityTimer = invincibilityCountdown;
                playerHealth = previousHealth;
                previousHealthSet = false;
            }
        }
        //Slow
        if (slowActive)
        {
            slowText.color = new Color(1f, 1f, 1f, 1f);
            slowText.text = "" + Mathf.FloorToInt(slowTimer + 1) + "s";
            slowIcon.color = new Color(1f, 1f, 1f, 1f);
            float period = 0.75f;
            float cycles = Time.time / period;
            if (slowTimer < 5f)
            {
                slowIcon.color = new Color(1f, 1f, 1f, ((Mathf.Sin(cycles * (Mathf.PI * 2)) + 1f) / 2.5f) + 0.2f);
            }
            else
            {
                slowIcon.color = new Color(1f, 1f, 1f, 1f);
            }
            slowTimer -= Time.deltaTime;
            if (!previousSlowSet)
            {
                //previousSlow = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed;
                previousSlowSet = true;
                pickUp.Play();
                slowPickUp.Play();
            }
            if (!slowSet)
            {
                //GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed = GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed / 2;
                slowSet = true;
            }
            if (!previousSpeedSet)
            {
                previousSpeed = GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed;
                previousSpeedSet = true;
            }
            if (!speedSet)
            {
                GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed = GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed * 1.5f;
                speedSet = true;
            }
            if (slowTimer <= 0f || GameObject.Find("SpawningSystem").GetComponent<Spawning>().waveComplete)
            {
                slowActive = false;
                slowIcon.color = new Color(1f, 1f, 1f, 0.2f);
                slowText.color = new Color(1f, 1f, 1f, 0.5f);
                slowText.text = "0s";
                slowTimer = slowCountdown;
                if (GameObject.Find("SpawningSystem").GetComponent<Spawning>().waveComplete)
                {
                    //GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed = previousSlow + GameObject.Find("SpawningSystem").GetComponent<Spawning>().enemySpeedIncrease;
                }
                else
                {
                    //GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalEnemySpeed = previousSlow;
                }
                GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed = previousSpeed;
                previousSlowSet = false;
                previousSpeedSet = false;
                speedSet = false;
                slowSet = false;
            }
        }
    }
    public void PlayerDeath()
    {
        GameObject.Find("Canvas").GetComponent<UserInterface>().killsV = killCount;
        GameObject.Find("Canvas").GetComponent<UserInterface>().timeV = GameObject.Find("Canvas").GetComponent<UserInterface>().playtime;
        GameObject.Find("Canvas").GetComponent<UserInterface>().waveV = GameObject.Find("SpawningSystem").GetComponent<Spawning>().waveCount;
        GameObject.Find("Canvas").GetComponent<UserInterface>().dead = true;
        GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().Pause();
        Instantiate(deathAudio);
        Instantiate(deathParticles, transform.position, transform.rotation);
        this.gameObject.SetActive(false);
    }
    public void MedKit()
    {
        buySound.Play();
        if (playerHealth <= maxPlayerHealth - medkitIncrease)
        {
            money -= medKitPrice;
            playerHealth += medkitIncrease;
        }
        else
        {
            money -= medKitPrice;
            playerHealth = maxPlayerHealth;
        }

        startNextWave = true;

        GameObject.Find("MedKitText").GetComponent<Text>().text = "Med Kit(" + medkitIncrease + "HP): " + medKitPrice;
    }
    public void DamageBuff()
    {
        buySound.Play();
        money -= damageBuffPrice;
        GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalBulletDamage += damageIncrease;
        damageBuffPrice += damageBuffPriceIncrease;
        damageBuffPriceIncrease += globalPriceIncrease;
        GameObject.Find("DamageText").GetComponent<Text>().text = "Damage Buff: $" + damageBuffPrice;

        startNextWave = true;
    }
    public void HealthBuff()
    {
        buySound.Play();
        money -= healthBuffPrice;
        maxPlayerHealth += healthIncrease;
        playerHealth = maxPlayerHealth;
        healthBuffPrice += healthBuffPriceIncrease;
        healthBuffPriceIncrease += globalPriceIncrease;
        GameObject.Find("HealthBuffText").GetComponent<Text>().text = "Health Buff: $" + healthBuffPrice;

        startNextWave = true;
    }
    public void SprintBuff()
    {
        buySound.Play();
        money -= sprintBuffPrice;
        GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed += sprintIncrease;
        if (GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed >= sprintMax)
        {
            GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed = sprintMax;
            GameObject.Find("SprintText").GetComponent<Text>().text = "Sprint Buff: MAX";
        }
        else
        {
            sprintBuffPrice += sprintBuffPriceIncrease;
            sprintBuffPriceIncrease += globalPriceIncrease;
            GameObject.Find("SprintText").GetComponent<Text>().text = "Sprint Buff: $" + sprintBuffPrice;
        }

        startNextWave = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("BuyTrigger"))
        {
            buyMenuActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BuyTrigger"))
        {
            buyMenuActive = false;
        }
    }
}
