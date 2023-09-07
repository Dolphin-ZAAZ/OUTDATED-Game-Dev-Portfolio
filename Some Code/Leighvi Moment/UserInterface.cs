using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    Text startCountdown;
    float deathMenuTimer;
    public float deathMenuTime = 2f;

    AudioSource click;

    Text healthText;
    Text playtimeText;
    Image healthBar;

    Text killCountText;
    Text moneyText;
    Text maxHealthText;
    //Text runSpeedText;
    Text sprintSpeedText;
    Text bulletDamageText;

    Text waveCountText;
    Text waveTimerText;
    Text enemyHealthText;
    Text enemyCountText;
    Text attackDamageText;
    Text enemySpeedText;

    Text kills;
    Text wave;
    Text time;
    public int killsV;
    public int waveV;
    public float timeV;

    GameObject pausemenu;
    GameObject deathMenu;
    GameObject generalInfo;
    GameObject playerInfo;
    GameObject enemyInfo;
    GameObject shopReminder;
    GameObject shopMenuText;
    bool shopReminderCancel;
    public GameObject buyMenu;

    public GameObject mapOverlay;

    Spawning spawning;
    PlayerBehaviour playerBehavoiur;

    public bool paused = true;
    public bool dead = false;

    public float playtime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        shopReminder = GameObject.Find("ShopReminder");

        click = GameObject.Find("PauseMenuSound").GetComponent<AudioSource>();

        spawning = GameObject.Find("SpawningSystem").GetComponent<Spawning>();
        playerBehavoiur = GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>();

        dead = false;

        startCountdown = GameObject.Find("StartCountdown").GetComponent<Text>();
        deathMenuTimer = deathMenuTime;

        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        playtimeText = GameObject.Find("PlaytimeText").GetComponent<Text>();
        healthBar = GameObject.Find("ScalarBar").GetComponent<Image>();

        killCountText = GameObject.Find("KillCountText").GetComponent<Text>();
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        maxHealthText = GameObject.Find("MaxHealthText").GetComponent<Text>();
        //runSpeedText = GameObject.Find("RunSpeedText").GetComponent<Text>();
        sprintSpeedText = GameObject.Find("SprintSpeedText").GetComponent<Text>();
        bulletDamageText = GameObject.Find("BulletDamageText").GetComponent<Text>();

        waveCountText = GameObject.Find("WaveCountText").GetComponent<Text>();
        waveTimerText = GameObject.Find("WaveTimerText").GetComponent<Text>();
        enemyHealthText = GameObject.Find("EnemyHealthText").GetComponent<Text>();
        enemyCountText = GameObject.Find("EnemyCountText").GetComponent<Text>();
        enemySpeedText = GameObject.Find("EnemySpeedText").GetComponent<Text>();
        attackDamageText = GameObject.Find("AttackDamageText").GetComponent<Text>();

        mapOverlay = GameObject.Find("MapOverlay");

        kills = GameObject.Find("Kills").GetComponent<Text>();
        wave = GameObject.Find("Wave").GetComponent<Text>();
        time = GameObject.Find("Time").GetComponent<Text>();

        generalInfo = GameObject.Find("GeneralInfo");
        playerInfo = GameObject.Find("PlayerInfo");
        enemyInfo = GameObject.Find("EnemyInfo");

        
        shopMenuText = GameObject.Find("ShopMenuText");
        shopReminderCancel = false;

        pausemenu = GameObject.Find("PauseMenu");
        pausemenu.SetActive(false);
        buyMenu = GameObject.Find("Buy Menu");
        deathMenu = GameObject.Find("GameOverMenu");

        deathMenu.SetActive(false);

        playtime = -spawning.startCountdown;

        mapOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            deathMenuTimer -= Time.deltaTime;
        }
        #region Pause Menu
        if (Input.GetButtonDown("Cancel") && pausemenu.activeSelf == true)
        {
            pausemenu.SetActive(false);

            if (buyMenu.activeSelf == true || mapOverlay.activeSelf == true)
            {
                paused = true;

            }
            else
            {
                paused = false;
            }
        }
        else if (Input.GetButtonDown("Cancel") && pausemenu.activeSelf == false && deathMenu.activeSelf == false)
        {
            pausemenu.SetActive(true);
            paused = true;
        }
        #endregion
        #region Buy Menu
        if (spawning.downtimeTimer > 55f && !shopReminderCancel && !paused)
        {
            shopReminder.SetActive(true);
            if (buyMenu.activeSelf)
            {
                shopReminder.SetActive(false);
                shopReminderCancel = true;
            }
        }
        else
        {
            shopReminder.SetActive(false);
        }
        if (spawning.downtimeTimer < 55f)
        {
            shopReminderCancel = false;
        }
        if (playerBehavoiur.buyMenuActive && spawning.downtimeTimer > 0 && buyMenu.activeSelf == false  )
        {
            shopMenuText.SetActive(true);
        }
        else
        {
            shopMenuText.SetActive(false);
        }
        if (Input.GetButtonDown("Buy Menu") && buyMenu.activeSelf == false && pausemenu.activeSelf == false && deathMenu.activeSelf == false && playerBehavoiur.buyMenuActive && spawning.downtimeTimer > 0)
        {
            buyMenu.SetActive(true);
            paused = true;

        }
        else if (Input.GetButtonDown("Buy Menu") && buyMenu.activeSelf == true && pausemenu.activeSelf == false)
        {
            buyMenu.SetActive(false);
            paused = false;
        }

        if (deathMenu.activeSelf == true)
        {
            buyMenu.SetActive(false);
        }
        #endregion
        #region Death Menu
        if (dead == true && deathMenuTimer <= 0)
        {
            deathMenu.SetActive(true);
            kills.text = "Kills: " + killsV;
            time.text = "Time: " + timeV + "s";
            wave.text = "Wave: " + waveV;
            generalInfo.SetActive(false);
            playerInfo.SetActive(false);
            enemyInfo.SetActive(false);
            shopMenuText.SetActive(false);
            GameObject.Find("WaveInfo").SetActive(false);
            GameObject.Find("MinimapBorder").SetActive(false);
            GameObject.Find("MinimapOutline").SetActive(false);
            GameObject.Find("Runes").SetActive(false);
            GameObject.Find("Abilities").SetActive(false);
            GameObject.Find("Minimap").SetActive(false);
            GameObject.Find("MoneyText").SetActive(false);
            GameObject.Find("MapOverlay").SetActive(false);
        }
        #endregion
        #region Map Overlay
        if (Input.GetButton("MapOverlay"))
        {
            if (pausemenu.activeSelf == false && buyMenu.activeSelf == false && deathMenu.activeSelf == false)
            {
                GameObject.Find("OpenMap").GetComponent<Text>().text = " ";
                mapOverlay.SetActive(true);
            }
        }
        else
        {
            GameObject.Find("OpenMap").GetComponent<Text>().text = "Open Map (Tab)";
            mapOverlay.SetActive(false);
        }
        #endregion
        #region Pause Mechanic
        if (paused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        #endregion
        #region Start Countdown
        if (spawning.startCountdown > 0)
        {
            startCountdown.text = "Starting in: " + Mathf.Round(spawning.startCountdown * 100) * 0.01f + "s";
        }
        else
        {
            startCountdown.text = "";
        }
        #endregion
        #region Text Update
        playtime += Time.deltaTime;
        if (GameObject.Find("PlayerCharacter").GetComponent<PlayerBehaviour>().invincibilityActive)
        {
            healthText.text = "Health: Infinite HP";
        }
        else
        {
            healthText.text = "Health: " + playerBehavoiur.playerHealth + "HP";
        }
        playtimeText.text = "Playtime: " + Mathf.Round(playtime * 100) * 0.01f + "s";

        healthBar.fillAmount = (float)playerBehavoiur.playerHealth / playerBehavoiur.maxPlayerHealth;

        killCountText.text = "Kill Count: " + playerBehavoiur.killCount;
        moneyText.text = "$" + playerBehavoiur.money;
        maxHealthText.text = "Max Health: " + playerBehavoiur.maxPlayerHealth + "HP";
        //runSpeedText.text = "Walk Speed: " + GameObject.Find("PlayerCharacter").GetComponent<Movement>().walkSpeed + "m/s";
        sprintSpeedText.text = "Sprint Speed: " + GameObject.Find("PlayerCharacter").GetComponent<Movement>().sprintSpeed + "m/s";

        bulletDamageText.text = "Bullet Damage: " + spawning.globalBulletDamage + "HP";
        waveCountText.text = "Wave: " + spawning.waveCount;
        enemyHealthText.text = "Enemy Health: " + spawning.globalEnemyHealth + "HP";
        enemyCountText.text = "Enemies On Map: " + spawning.enemyCount;
        enemySpeedText.text = "Enemy Speed: " + spawning.globalEnemySpeed + "m/s";
        attackDamageText.text = "Enemy Attack Damage: " + spawning.globalAttackDamage + "HP";

        if (spawning.downtimeTimer <= 0f)
        {
            waveTimerText.text = "Enemies Remaining: " + spawning.enemiesRemainingCountdown;
        }
        else
        {
            waveTimerText.text = "Downtime: " + Mathf.Round(spawning.downtimeTimer * 100) * 0.01f + "s";
        }
        #endregion
    }
    public void Resume()
    {
        pausemenu.SetActive(false);
        click.Play();
        if (buyMenu.activeSelf == true)
        {
            paused = true;
        }
        else
        {
            paused = false;
        }
    }
    public void Restart()
    {
        click.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToDesktop()
    {
        click.Play();
        Application.Quit();
    }
    public void QuitToMainMenu()
    {
        click.Play();
        SceneManager.LoadScene("MainMenu");
    }
}
