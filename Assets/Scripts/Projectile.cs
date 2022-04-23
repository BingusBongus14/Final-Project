using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public BossController BossController;
    
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
    
    void Update()
    {
        if(transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }
        HardEnemyController p = other.collider.GetComponent<HardEnemyController>();
        if (p != null)
        {
            p.Fix();
        }
        BossController g = other.collider.GetComponent<BossController>();
        if (g != null)
        {
            g.ChangeHealth(-1);
        }
    
        Destroy(gameObject);
    }
}
