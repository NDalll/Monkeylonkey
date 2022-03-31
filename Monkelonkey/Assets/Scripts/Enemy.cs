using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private GameObject player;
    private Rigidbody2D RB;
    private Animator animator;
    private bool firstSpawn = true;
    private PlayerTrigger playerTrigger;

    [Header("General")]
    public float health;
    public int bodyDamage;
    public int weaponDamage;
    public bool isFollowing;


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
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 currTarget;
    public float walkSpeed;
    public float runSpeed;

    [Header("Ranged Enemy")]
    public bool isRanged;
    public GameObject projectile;
    public float initProjSpeed;
    public float projAccelation;
    public bool isHoming;
    public float fireRate;
    [System.NonSerialized]
    public bool seeingPlayer;
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
        if (isWalking)
        {
            startPos = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0);
            endPos = endPoint.transform.position;

            transform.position = startPos;
            currTarget = endPos;
        }
        playerTrigger = transform.GetChild(0).GetComponent<PlayerTrigger>();
        playerTrigger.damage = bodyDamage;
    }
    private void Update()
    {
        if (isJumping)
        {
            if (timing)
            {
                if (Time.time - cooldownTimer >= jumpCooldown)
                {
                    timing = false;
                    animator.SetTrigger("doneCooldown");
                }
            }
        }

        if (seeingPlayer) 
        { 
            if (isRanged)
            {
                if (Time.time - fireTimer >= fireRate)
                {
                    fireProjectile();
                    fireTimer = Time.time;
                }
            }
        }
        
        if (isWalking)
        {
            if (seeingPlayer)
            {
                Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, 0).normalized;
                transform.Translate(dir * Time.deltaTime * runSpeed);
            }
            else
            {
                Vector2 dir = new Vector2(currTarget.x - transform.position.x, 0).normalized;
                transform.Translate(dir * Time.deltaTime * runSpeed);
                
            }
            
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isJumping)
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
        }
        if (isWalking)
        {
            if (collision.CompareTag("WayPoint"))
            {
                Debug.Log("at target");
                switchTarget();
            }
        }
        
    }
    
    private void Jump() {
        float radAngle = jumpAngle * Mathf.Rad2Deg;
        if (jumpCount == jumps)
        {
            jumpCount = 0;
            forward = switchBool(forward);
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

    public void PlayerEntered()
    {
        if (isRanged)
        {
            fireTimer = Time.time;
        }
        
        seeingPlayer = true;
    }
    public void PlayerExited()
    {
        seeingPlayer = false;
        
    }
    private bool switchBool(bool oldBool)
    {
        bool newBool;
        if (oldBool)
        {
            newBool = false;
        }
        else {
            newBool = true;
        }
        return newBool;
    }
    private void switchTarget()
    {
        if (currTarget == startPos)
        {
            currTarget = endPos;
        }
        else
        {
            currTarget = startPos;
        }
    }
    private void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

}
