using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawning : MonoBehaviour
{
    [Header("Balancing")]
    [SerializeField] int killMoneyIncrease = 1;
    [SerializeField] public int enemyHealthIncrease = 150;
    [SerializeField] public int attackDamageIncrease = 2;
    [SerializeField] public float enemySpeedIncrease = 0.5f;
    [SerializeField] public int enemyCountIncrease = 4;
    [SerializeField] public int enemiesRemainingIncrease = 30;
    [SerializeField] public float maxEnemySpeed = 30f;
    [SerializeField] public int maxEnemiesRemaining = 300;
    [SerializeField] int maxEnemyCount = 50;
    [SerializeField] int incenDamageIncrease = 10;
    [SerializeField] int rightDamageIncrease = 35;

    [Header("Rune Balancing")]
    [SerializeField] public int moneyRunePrice = 1000;
    [SerializeField] public int moneyRuneIncrease = 500;
    public float cashRuneIncrease = 1.5f;
    public bool cashRuneActive = false;
    public float cashRuneTimer = 0f;

    Text cashRuneText;
    Image cashRuneIcon;

    [Header("Globals")]
    [SerializeField] public int globalEnemyHealth = 100;
    [SerializeField] public int globalAttackDamage = 5;
    [SerializeField] public float globalEnemySpeed = 5f;
    [SerializeField] public int globalBulletDamage = 10;
    [SerializeField] public int globalKillMoney = 5;
    [SerializeField] public int globalRightDamage = 100;
    [SerializeField] public int globalIncenDamage = 20;
    [SerializeField] public bool globalRightActivate;


    [Header("The Rest")]
    [SerializeField] public float startCountdown = 10f;

    [SerializeField] GameObject enemy;

    [SerializeField] AudioSource waveStart;
    bool waveStartPlayed = false;
    [SerializeField] AudioSource waveEnd;

    [SerializeField] float downtimeCountdown = 30f;
    [SerializeField] public float downtimeTimer;

    [SerializeField] public int waveCount;
    [SerializeField] public bool waveComplete;

    [SerializeField] public int baseEnemyCount = 50;
    [SerializeField] public int enemyCount;
    [SerializeField] public int enemiesRemaining = 300;
    [SerializeField] public int enemiesRemainingCountdown = 300;



    [SerializeField] GameObject[] spawnPoint;
    [SerializeField] int randSpawn;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoints");
        waveStart = GameObject.Find("WaveStart").GetComponent<AudioSource>();
        waveEnd = GameObject.Find("WaveEnd").GetComponent<AudioSource>();
        cashRuneIcon = GameObject.Find("CashIcon").GetComponent<Image>();
        cashRuneText = GameObject.Find("CashText").GetComponent<Text>();
        randSpawn = Random.Range(0, spawnPoint.Length);
        downtimeTimer = 0f;
        enemiesRemainingCountdown = enemiesRemaining;
        waveComplete = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        startCountdown -= Time.fixedDeltaTime;
        downtimeTimer -= Time.fixedDeltaTime;
        cashRuneTimer -= Time.fixedDeltaTime;
        if (cashRuneTimer > 0)
        {
            cashRuneActive = true;
            cashRuneText.text = "" + Mathf.FloorToInt(cashRuneTimer + 1) + "s";
            cashRuneText.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            cashRuneActive = false;
            cashRuneText.text = "0s";
        }
        if (cashRuneTimer < 5 && cashRuneTimer > 0)
        {
            float period = 0.75f;
            float cycles = Time.time / period;
            cashRuneIcon.color = new Color(1f, 1f, 1f, ((Mathf.Sin(cycles * (Mathf.PI * 2)) + 1f) / 2.5f) + 0.2f);
        }
        else if (cashRuneTimer > 5)
        {
            cashRuneIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            cashRuneIcon.color = new Color(1f, 1f, 1f, 0.2f);
            cashRuneText.color = new Color(1f, 1f, 1f, 0.5f);
        }

        if (GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().startNextWave)
        {
            downtimeTimer = 0f;
            GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().startNextWave = false;
        }

        if(startCountdown <= 0 && waveStartPlayed == false)
        {
            waveStart.Play();
            waveStartPlayed = true;

        }    

        if (enemiesRemainingCountdown <= 0 && waveComplete == false)
        {
            waveEnd.Play();
            downtimeTimer = downtimeCountdown;
            waveComplete = true;
            print("wave ended");

            if (maxEnemiesRemaining > enemiesRemaining)
            {
                enemiesRemaining += enemiesRemainingIncrease;
            }
            else
            {
                enemiesRemaining = maxEnemiesRemaining;
            }

            enemiesRemainingCountdown = enemiesRemaining;

            if (baseEnemyCount < maxEnemyCount)
            {
                baseEnemyCount += enemyCountIncrease;
            }
            else
            {
                baseEnemyCount = maxEnemyCount;
            }

            globalKillMoney += killMoneyIncrease;
            moneyRunePrice += moneyRuneIncrease;
            globalEnemyHealth += enemyHealthIncrease;
            globalAttackDamage += attackDamageIncrease;
            globalIncenDamage += incenDamageIncrease;
            globalRightDamage += rightDamageIncrease;

            if (globalEnemySpeed >= maxEnemySpeed)
            {
                globalEnemySpeed = maxEnemySpeed;
            }
            else
            {
                globalEnemySpeed += enemySpeedIncrease;
            }

            waveCount++;
        }

        if (enemiesRemainingCountdown <= enemyCount)
        {
            enemyCount = enemiesRemainingCountdown;
        }
        else
        {
            enemyCount = baseEnemyCount;
        }

        if (downtimeTimer <= 0 && waveComplete == true)
        {
            print("wave started");
            waveStart.Play();
            waveComplete = false;
           
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length < enemyCount && startCountdown < 0 && enemiesRemainingCountdown > 0 && downtimeTimer <= 0f)
        {
            randSpawn = Random.Range(0, spawnPoint.Length);
            Instantiate(enemy, spawnPoint[randSpawn].transform.position, Quaternion.identity);
        }
    }
}
