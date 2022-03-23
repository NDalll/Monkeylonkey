using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private GameObject player;
    private Rigidbody2D RB;
    private Animator animator;
    private bool firstSpawn = true;

    [Header("General")]
    public float health;
    public int bodyDamage;
    public int weaponDamage;


    [Header("Jumping Enemy")]
    public bool isJumping;
    public int jumps;
    public float jumpCooldown;
    public float jumpAngle;
    public float jumpMagnitude;
    private bool forward = true;
    private int jumpCount = 0;
    private float cooldownTimer;
    private bool timing;

    [Header("Walking Enemy")]
    public bool isWalking;
    public GameObject startPoint;
    public GameObject endPoint;


    [Header("Ranged Enemy")]
    public bool isRanged;
    public GameObject projectile;
    public float initProjSpeed;
    public float projAccelation;
    public bool isHoming;
    public float fireRate;
    [System.NonSerialized]
    public bool fireEnabled;
    private float fireTimer;

    [Header("Melee Enemy")]
    public bool isMelee;
    public GameObject meleeWeapon;


    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (timing)
        {
            if(Time.time - cooldownTimer >= jumpCooldown)
            {
                timing = false;
                animator.SetTrigger("doneCooldown");
            }
        }
        if (fireEnabled)
        {
            if(Time.time - fireTimer >= fireRate)
            {
                fireProjectile();
                fireTimer = Time.time;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            if (RB.velocity.y <= 0 && firstSpawn != true)
            {
                Debug.Log("groundHit");
                animator.SetTrigger("groundHit");
            }
            else
            {
                firstSpawn = false;
            }

        }
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().dealDamage(bodyDamage);
        }
    }
    
    private void Jump() {
        float radAngle = jumpAngle * Mathf.Rad2Deg;
        if (jumpCount == jumps)
        {
            jumpCount = 0;
            if (forward)
            {
                forward = false;
            } 
            else
            {
                forward = true;
            }
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x*-1, gameObject.transform.localScale.y, 1);

        }
        if (forward)
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle)*-1, Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++;
        }
        else
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    private void fireProjectile()
    {
        GameObject projectile = Instantiate(this.projectile);
        projectile.transform.position = transform.position + new Vector3(0,.5f,0);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.initProjSpeed = initProjSpeed;
        projectileScript.projAccelation = projAccelation;
        projectileScript.isHoming = isHoming;
        projectileScript.damage = weaponDamage;
        projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(GetVectorToPlayer().x, GetVectorToPlayer().y), ForceMode2D.Impulse);
        projectile.transform.eulerAngles = GetOrientation();
    }

    private Vector3 GetVectorToPlayer()
    {
        Vector3 vector = player.transform.position - transform.position;
        return vector.normalized * initProjSpeed;
    }
    private Vector3 GetOrientation()
    {
        float angle = Mathf.Atan2(GetVectorToPlayer().y, GetVectorToPlayer().x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, angle);
    }
    private void StartJumpCooldown()
    {
        cooldownTimer = Time.time;
        timing = true;
    }

    public void StartFire()
    {
        fireTimer = Time.time;
        fireEnabled = true;
    }
    private void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

}
