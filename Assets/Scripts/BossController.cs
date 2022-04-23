using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int maxHealth = 3;
    int currentHealth;
    public ParticleSystem hitEffectPrefab;

    public float speed;
    public float changeTime = 3.0f;

    public ParticleSystem smokeEffect;
    
    new Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;
    
    Animator animator;
    
    public RubyController RubyController;

    public AudioSource audiosource;
    public AudioClip hitboss;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        
        GameObject rubycontrollerObject = GameObject.FindWithTag("RubyController");

        if(rubycontrollerObject != null)
        {
            RubyController = rubycontrollerObject.GetComponent<RubyController>();
            print ("Found the RubyController Sctript!");
        }
        if (RubyController == null)
        {
            print ("Cannot find GameController Script!");
        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        Vector2 position = rigidbody2D.position;
        if (speed != 0)
        {
            position.x = position.x + Time.deltaTime * speed * direction;
        animator.SetFloat("Move X", direction);
        }
        
        
        rigidbody2D.MovePosition(position);
    }
    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth +"/" + maxHealth);
        ParticleSystem hitEffect= Instantiate(hitEffectPrefab, rigidbody2D.position + Vector2.up * 1.5f, Quaternion.identity);

        if (currentHealth == 0)
        {
            Fix();
        }
        if (amount < 0)
        {
            audiosource.clip = hitboss;
                audiosource.Play();
            audiosource.loop = false;
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }
    
    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();

        if (RubyController != null)
        {
            RubyController.ChangeScore();
        }

    }
}