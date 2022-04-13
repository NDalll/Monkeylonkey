using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized]
    public float turnforce;
    [System.NonSerialized]
    public float initProjSpeed;
    [System.NonSerialized]
    public bool isHoming;
    [System.NonSerialized]
    public float damage;
    private GameObject player;
    private bool isCollided;
    

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        animator = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isHoming)
        {
            rb.AddForce(GetVectorToPlayer());
            rb.velocity = rb.velocity.normalized * initProjSpeed;
            transform.eulerAngles = GetOrientation();
        }
    }

    private Vector3 GetOrientation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, angle);
    }

    private Vector3 GetVectorToPlayer()
    {
        Vector3 vector = player.transform.position - transform.position;
        return vector.normalized * turnforce;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") == false && collision.isTrigger == false)
        {
            rb.velocity = Vector3.zero;
            animator.SetTrigger("ColliderHit");
        }
        if (collision.CompareTag("Player") && !isCollided)
        {
            collision.GetComponent<Player>().dealDamage(damage);
        }
        else
        {
            isCollided = true;
        }
    }

    private void Death()
    {
        GameObject.Destroy(gameObject);
    }
}
