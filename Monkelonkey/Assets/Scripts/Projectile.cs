using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized]
    public float initProjSpeed;
    [System.NonSerialized]
    public float projAccelation;
    [System.NonSerialized]
    public bool isHoming;
    [System.NonSerialized]
    public int damage;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().dealDamage(damage);
        }
        if (collision.gameObject.CompareTag("Enemy") == false && collision.isTrigger == false )
        {
            rb.velocity = Vector3.zero;
            animator.SetTrigger("ColliderHit");
        }

    }

    private void Death()
    {
        GameObject.Destroy(gameObject);
        
    }
}
