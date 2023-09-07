using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    public GameObject fireNade;
    public float nadeForce = 50f;

    [SerializeField] AudioSource gunLoop;
    [SerializeField] AudioSource gunEnd;

    [SerializeField] ParticleSystem muzzleParticle;
    [SerializeField] GameObject muzzleLight;

    [SerializeField] float incenCooldown = 20f;
    [SerializeField] float incenCooldownTimer = 0;

    Text incenText;
    Image incenScalar;
    Image incenGrey;
    Image incenControlGrey;

    // Start is called before the first frame update
    void Start()
    {
        gunLoop = GameObject.Find("GunLoop").GetComponent<AudioSource>();
        gunEnd = GameObject.Find("GunEnd").GetComponent<AudioSource>();

        incenText = GameObject.Find("IncenText").GetComponent<Text>();
        incenGrey = GameObject.Find("IncenGrey").GetComponent<Image>();
        incenScalar = GameObject.Find("IncenScalar").GetComponent<Image>();
        incenControlGrey = GameObject.Find("IncenControlGrey").GetComponent<Image>();

        gunEnd.playOnAwake = false;
        incenCooldownTimer = 0;
    }
    private void Update()
    {
        incenCooldownTimer -= Time.deltaTime;
        if(incenCooldownTimer < 0)
        {
            incenText.gameObject.SetActive(false);
            incenControlGrey.gameObject.SetActive(false);
            incenScalar.gameObject.SetActive(false);
            incenGrey.gameObject.SetActive(false);
        }
        else
        {
            incenText.gameObject.SetActive(true);
            incenControlGrey.gameObject.SetActive(true);
            incenScalar.gameObject.SetActive(true);
            incenGrey.gameObject.SetActive(true);

            incenText.text = "" + Mathf.FloorToInt(incenCooldownTimer + 1);
            incenText.color = new Color(1f, 1f, 1f, 1f);
            incenScalar.fillAmount = incenCooldownTimer / incenCooldown;
        }

        if (GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            gunLoop.gameObject.SetActive(false);
            gunEnd.Pause();
        }

        if (Input.GetButton("Fire1") && GameObject.Find("Canvas").GetComponent<UserInterface>().paused == false)
        {
            muzzleLight.SetActive(!muzzleLight.activeSelf);
        }
        else
        {
            muzzleLight.SetActive(false);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
        else
        {
            gunLoop.gameObject.SetActive(false);
            gunEnd.gameObject.SetActive(true);
            gunEnd.playOnAwake = true;
            gunEnd.UnPause();
            muzzleParticle.Stop();
        }
        if (Input.GetButtonDown("Nade") && incenCooldownTimer < 0 && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            GameObject fNade = Instantiate(fireNade, this.transform.position, this.transform.rotation);
            incenCooldownTimer = incenCooldown;
        }
        else if (Input.GetButtonDown("Nade") && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            GameObject.Find("ErrorSound").GetComponent<AudioSource>().Play();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(this.transform.up * bulletForce,ForceMode2D.Impulse);
        gunLoop.gameObject.SetActive(true);
        gunEnd.gameObject.SetActive(false);
        muzzleParticle.Play();
    }
}
