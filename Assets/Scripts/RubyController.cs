using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


﻿public class RubyController : MonoBehaviour
{
    public float speed = 3f;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    public GameObject bossPrefab;
    private Vector2 pos;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip WinSong;
    public AudioClip GameOver;

    public ParticleSystem healthEffectPrefab;
    public ParticleSystem hitEffectPrefab;

    public Text score;
    private int scoreValue;
    public GameObject WinTextObject;
    public GameObject LoseTextObject;

    public bool IsAlive;
    public GameObject Level1OverText;

    public bool gameOver = false;
    public bool level1;

    public Text Cogs;
    public int Ammo;
    public AudioClip collectedClip;

    private float activeMoveSpeed;
    public float dashSpeed;
    public float dashLength = .5f, dashCooldown = 1f;
    private float dashCounter;
    private float dashCoolCounter;
    public AudioClip dashnoise;
    public TrailRenderer dashtrail;
    
    
    // Start is called before the first frame update
    void Start()
    {
        dashtrail.enabled = false;
        activeMoveSpeed = speed;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        score.text = scoreValue.ToString();

        WinTextObject.SetActive(false);
        LoseTextObject.SetActive(false);
        Level1OverText.SetActive(false);
        IsAlive = true;
        

        SetCogsText();
        Cogs.text = "Cogs: " + Ammo.ToString();
        Ammo = 4;

        level1 = true;
        GameObject Jambi = GameObject.FindWithTag("Jambi");
        if(Jambi != null)
        {
            level1 = true;
        }
        if (Jambi == null)
        {
            level1 = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

         if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(dashCoolCounter <= 0 && dashCounter <= 0 && move.magnitude != 0)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
                audioSource.clip = dashnoise;
                    audioSource.Play();
                audioSource.loop = false;
                dashtrail.enabled = true;
            }
        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            if( dashCounter <= 0)
            {
                activeMoveSpeed = speed;
                dashCoolCounter = dashCooldown;
                dashtrail.enabled = false;
            }
        }

        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
               RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null && scoreValue == 4)
                {
                     SceneManager.LoadScene("Main 1");
                }

                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
        score.text = "Robots Fixed: " + scoreValue.ToString();

        if (currentHealth == 0)
        {
            LoseTextObject.SetActive(true);
            speed = 0.0f;
            isInvincible = true;
            if (currentHealth == 0)
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                 // this loads the currently active scene
            }
        }
        if (currentHealth == 0 && IsAlive)
        {
            IsAlive = false;
            GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().mute = true;
            audioSource.clip = GameOver;
                audioSource.Play();
            audioSource.loop = false;
        
        }
        if (scoreValue == 5 && level1 == false)
        {
            isInvincible = true;
            WinTextObject.SetActive(true);

             if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
                 // this loads the currently active scene
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + activeMoveSpeed * horizontal * Time.deltaTime;
        position.y = position.y + activeMoveSpeed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
            ParticleSystem hitEffect= Instantiate(hitEffectPrefab, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);    
        }
        
        if (amount > 0)
        {
            ParticleSystem healthEffect= Instantiate(healthEffectPrefab, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore()
    {
        scoreValue += 1;
        score.text = "Robots Fixed: " + score.ToString();

        if (scoreValue == 4 && level1 == false)
        {
            pos = new Vector2(3f,1.63f);
            GameObject bossObject = Instantiate(bossPrefab, pos, Quaternion.identity);
        }
        
        if (scoreValue == 5 && level1 == false)
        {
             GameObject.Find("BackgroundMusic").GetComponent<AudioSource>().mute = true;
         
            audioSource.clip = WinSong;
                audioSource.Play();
            audioSource.loop = false;
            WinTextObject.SetActive(true);
            isInvincible = true;

             if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
                 // this loads the currently active scene
            }
        }

        if (scoreValue == 4 && level1 == true)
        {
            Level1OverText.SetActive(true);
        }
    }

    void SetCogsText()
        {
            Cogs.text = "Cogs: " + Ammo.ToString();
        }
    public void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.CompareTag("Ammo") && Ammo < 4)
        {
            other.gameObject.SetActive(false);
            Ammo = Ammo + 4;
            Cogs.text = "Cogs: " + Ammo.ToString();
            ParticleSystem healthEffect= Instantiate(healthEffectPrefab, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
            PlaySound(collectedClip);
        }
    }
    
    void Launch()
    {
        if (Ammo > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
        Ammo = Ammo - 1;
        Cogs.text = "Cogs: " + Ammo.ToString();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
        
}