using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("General")] //de genrelle variabler:
    private GameObject player;
    [System.NonSerialized] public Rigidbody2D RB;
    private Animator animator;
    private bool firstSpawn = true;
    private PlayerTrigger playerTrigger;
    private Vector3 scale;
    private string lookDirection;
    public float health;
    public int bodyDamage;
    public int worth;
    public float alertTime;
    private float alertTimer;
    private bool alert;
    private bool ledgeJumpCooldown;



    [Header("Jumping Enemy")] //variablerne for de hoppende fjender:
    public bool isJumping;
    public int jumps;
    public float jumpCooldown;
    public float jumpAngle;
    public float jumpMagnitude;
    private bool forward = true;
    private int jumpCount = 0;
    private float cooldownTimer;
    private bool colTiming;

    [Header("Walking Enemy")] //variablerne for de gående fjender:
    public bool isWalking;
    public GameObject startPoint;
    public GameObject endPoint;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 currTarget;
    public float walkSpeed;
    public float runSpeed;
    public bool isFollowing;
    [System.NonSerialized] public float ogRunSpeed;
    [System.NonSerialized] public float ogWalkSpeed;


    [Header("Ranged Enemy")] //variablerne for de ranged fjender
    public bool isRanged;
    public float rangedDamage;
    public GameObject projectile;
    public float initProjSpeed;
    public float turnForce;
    public bool isHoming;
    public float fireRate;
    [System.NonSerialized]
    public bool seeingPlayer;
    private float fireTimer;

    [Header("Melee Enemy")] //variablerne for melee fjenderne
    public bool isMelee;
    public GameObject meleeWeaponPrefab;
    private GameObject meleeWeapon;
    public float meleeDamage;
    private bool melee;

    // Start is called before the first frame update
    void Start()
    {
        if (isMelee) //tjekker om det er en melee fjende
        {
            meleeWeapon = Instantiate(meleeWeaponPrefab, gameObject.transform); //laver våbenet på spillerens position 
            meleeWeapon.GetComponent<MeleeWeapon>().damage = meleeDamage; //definerer skaden af våbnet
            meleeWeapon.SetActive(false); //disabler våbenet
        }
        
        scale = transform.localScale; //definere scalen til enemiens localscale
        player = GameObject.FindWithTag("Player"); //finder playeren
        RB = GetComponent<Rigidbody2D>(); //definerer rigdidbodien på fjenden
        animator = GetComponent<Animator>(); //definerer animatoren på fjenden
        if (isWalking) //tjekker om det er en gående fjende
        {
            startPos = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0); //sætter startpositionen til positionen af startpunket defineret i inspektoren
            endPos = endPoint.transform.position; //sætter slutpositionen til positionen af startpunket defineret i inspektoren

            transform.position = startPos; //flytter fjenden til startpunktet
            currTarget = endPos; //sætter dens mål til slutpositionen
        }
        playerTrigger = transform.GetChild(0).GetComponent<PlayerTrigger>();
        playerTrigger.damage = bodyDamage;
        ogRunSpeed = runSpeed;
        ogWalkSpeed = walkSpeed;
    }
    private void Update()
    {
        if (isJumping)
        {
            if (colTiming)
            {
                if (Time.time - cooldownTimer >= jumpCooldown)
                {
                    colTiming = false;
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
            if (seeingPlayer && isFollowing)
            {
                Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, 0).normalized;
                transform.Translate(dir * Time.deltaTime * runSpeed);
                SetLookDirection(dir);
            }
            else
            {
                Vector2 dir = new Vector2(currTarget.x - transform.position.x, 0).normalized;
                transform.Translate(dir * Time.deltaTime * walkSpeed);
                SetLookDirection(dir);
            }
            float distance = CalcXDistance();
            if (distance < 0.5f)
            {
                switchTarget();
            }
        }
        if (isMelee)
        {
            if (melee)
            {
                MeleeAttack();
            }
        }
        if (alert)
        {
            if (Time.time - alertTimer >= alertTime)
            {
                alert = false;
                seeingPlayer = false;
            }
        }
    }

    float CalcXDistance()
    {
        float distance;
        distance = Mathf.Abs(transform.position.x - currTarget.x);
        return distance;
    }
    private void SetLookDirection(Vector2 dir)
    {
        if (dir.x > 0)
        {
            transform.localScale = new Vector3(scale.x, scale.y, scale.z);
            lookDirection = "right";
        }
        else
        {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.y);
            lookDirection = "left";
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
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isWalking && !seeingPlayer)
        {
            if (collision.CompareTag("WayPoint"))
            {
                collision.GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }
    private void Jump() {
        float radAngle = jumpAngle * Mathf.Rad2Deg;
        if (jumpCount == jumps)
        {
            jumpCount = 0;
            forward = switchBool(forward);
            gameObject.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);

        }
        if (forward)
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle) * -1, Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++;
        }
        else
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++;
        }
    }
    public void LedgeJump()
    {
        if (!ledgeJumpCooldown)
        {
            RB.AddForce(Vector2.up * 9, ForceMode2D.Impulse);
            ledgeJumpCooldown = true;
            runSpeed = 0;
            walkSpeed = 0;
            Invoke("LegdeJumpCooldown", 1f);
        }
    }
    public void LegdeJumpCooldown()
    {
        ledgeJumpCooldown = false;
        runSpeed = ogRunSpeed;
        walkSpeed = ogWalkSpeed;
    }
    private void fireProjectile()
    {
        GameObject projectile = Instantiate(this.projectile);
        projectile.transform.position = transform.position + new Vector3(0, .5f, 0);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.initProjSpeed = initProjSpeed;
        projectileScript.turnforce = turnForce;
        projectileScript.isHoming = isHoming;
        projectileScript.damage = rangedDamage;
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
        colTiming = true;
    }
    public void StartAlertTimer(){
        alertTimer = Time.time;
        alert = true;
    }

    public void PlayerEntered()
    {
        if (isRanged)
        {
            fireTimer = Time.time;
        }
        if (isMelee)
        {
            StartMelee();
        }
        alert = false;
        seeingPlayer = true;
    }
    public void PlayerExited()
    {
        if (isMelee)
        {
            StopMelee();
        }
        StartAlertTimer();
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
    private void MeleeAttack()
    {
        meleeWeapon.transform.Rotate(new Vector3(0, 0, -500) * Time.deltaTime);
        Debug.Log(meleeWeapon.transform.localEulerAngles.z);
        if (Mathf.Abs(meleeWeapon.transform.localEulerAngles.z - 190) < 5)
        {
            meleeWeapon.transform.localEulerAngles = new Vector3(0, 0, 63);
        }
    }
    public void StartMelee()
    {
        meleeWeapon.SetActive(true);
        melee = true;
    }
    public void StopMelee()
    {
        melee = false;
        meleeWeapon.SetActive(false);
    }
    public void dealDamage(float damage)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        health -= damage;
        if(health == 0)
        {
            ScoreManager.grappleScore += worth;
            ScoreManager.enemiesDefeated++;
            GameObject.Destroy(gameObject);
        }
        spriteRenderer.color = Color.red;
        Invoke("turnWhite", 0.1f);
    }
    private void turnWhite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }
}
