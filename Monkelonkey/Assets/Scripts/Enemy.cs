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
        playerTrigger = transform.GetChild(0).GetComponent<PlayerTrigger>(); //finder colideren på der tjekker for playeren
        playerTrigger.damage = bodyDamage; //definere hvor meget skade kroppen gør til spilleren
        ogRunSpeed = runSpeed; //gemmer den originale Runspeed
        ogWalkSpeed = walkSpeed; //gemmer den originale Walkspeed
    }
    private void Update() //kaldes hver frame
    {
        if (isJumping) //tjekker om det er en hoppende fjende
        {
            if (colTiming) //Timeren for hvor ofte den hopper
            {
                if (Time.time - cooldownTimer >= jumpCooldown) //tjekker om der er cooldownet bør være ovre
                {
                    colTiming = false; //stopper timeren
                    animator.SetTrigger("doneCooldown"); //fortæller animatoren at ventetiden er ovre
                }
            }
        }

        if (seeingPlayer) //tjekker om den kan se spilleren
        {
            if (isRanged) //hvis den er ranged vil den skyde efter spilleren på en timer
            {
                if (Time.time - fireTimer >= fireRate)
                {
                    fireProjectile();
                    fireTimer = Time.time;
                }
            }
        }

        if (isWalking) //hvis det er en gående fjende
        {
            if (seeingPlayer && isFollowing) //hvis den kan se spilleren og er en following fjende
            {
                Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, 0).normalized; //finder retningen mod spilleren
                transform.Translate(dir * Time.deltaTime * runSpeed); //flytte fjenden
                SetLookDirection(dir); //kalder funktionen der vender fjenden
            }
            else //Dette er sandt når den enten ikke kan se spilleren eller ikke er en following enemy
            {
                Vector2 dir = new Vector2(currTarget.x - transform.position.x, 0).normalized; //finder retingen
                transform.Translate(dir * Time.deltaTime * walkSpeed); //flytter fjenden
                SetLookDirection(dir); //kalder funktionen der vender fjenden
            }
            float distance = CalcXDistance(); //kalder funktionen der finder afstanden mellem fjendens mål og fjenden
            if (distance < 0.5f) //hvis afstanden er lav nok, vender den sig om
            {
                switchTarget();
            }
        }
        if (isMelee) //tjekker om det er en melee enemy
        {
            if (melee) //tjekker om våbenet er aktivt
            {
                MeleeAttack(); //kalder angrebet
            }
        }
        if (alert) //hvis den ser spilleren vil den fortsætte i et stykke tid selvom spilleren er ude af dens syn
        {
            if (Time.time - alertTimer >= alertTime) 
            {
                alert = false;
                seeingPlayer = false;
            }
        }
    }

    float CalcXDistance()//finder afstanden mellem fjenden og dens mål
    {
        float distance;
        distance = Mathf.Abs(transform.position.x - currTarget.x);
        return distance;
    }
    private void SetLookDirection(Vector2 dir) //finder retningen mod målet
    {
        if (dir.x > 0)
        {
            transform.localScale = new Vector3(scale.x, scale.y, scale.z); //vender fjender
            lookDirection = "right";
        }
        else
        {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.y);//vender fjender
            lookDirection = "left";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //kaldes når et objekt går ind i dens collider
    {
        if (isJumping) //hvis det er en hoppende fjende
        {
            if (collision.CompareTag("Ground")) //tjekker om det var jorden den colliderede med
            {
                if (RB.velocity.y <= 0 && firstSpawn != true) //tjekker om den falder og det ikke er det første hop
                {
                    Debug.Log("groundHit");
                    animator.SetTrigger("groundHit"); //fotæller animatoren at den ramte jorden
                }
                else
                {
                    firstSpawn = false; //ellers blive det fortalt at det ikke længere er det første hop
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //når noget forlader colideren
    {
        if (isWalking && !seeingPlayer) //hvis den er walking men ikke ser playeren
        {
            if (collision.CompareTag("WayPoint")) //hvis det er et waypoint
            {
                collision.GetComponent<CircleCollider2D>().enabled = true; //enabler waypointets collider
            }
        }
    }
    private void Jump() //funktionen der får den til at hoppe
    {
        float radAngle = jumpAngle * Mathf.Rad2Deg; //finder vinklen den skal hoppe i
        if (jumpCount == jumps) //hvis den har hoppet det antal gange den skal
        {
            jumpCount = 0; //genstarter optællingen
            forward = switchBool(forward); //skifter mål
            gameObject.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1); //vænder den om

        }
        if (forward) //hvis den hopper fremad hopper den med vinklen fremad
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle) * -1, Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++; //tæller op
        }
        else  //hvis den hopper bagud hopper den med vinklen bagud
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++; //tæller op
        }
    }
    public void LedgeJump()//hopper over en kant hvis den støder på den
    {
        if (!ledgeJumpCooldown)
        {
            RB.AddForce(Vector2.up * 9, ForceMode2D.Impulse);//skubber den opad
            ledgeJumpCooldown = true; 
            runSpeed = 0; //sætter dens fart til 0 så den ikke sider fast i væggen
            walkSpeed = 0; //sætter dens fart til 0 så den ikke sider fast i væggen
            Invoke("LegdeJumpCooldown", 1f); //starter en timer på 1 sekund
        }
    }
    public void LegdeJumpCooldown()//timeren til ledgehoppet
    {
        ledgeJumpCooldown = false;
        runSpeed = ogRunSpeed;
        walkSpeed = ogWalkSpeed;
    }
    private void fireProjectile()//skyder projektilet
    {
        GameObject projectile = Instantiate(this.projectile); //laver projektilet
        projectile.transform.position = transform.position + new Vector3(0, .5f, 0);//sætter den på fjendens position
        Projectile projectileScript = projectile.GetComponent<Projectile>();//finder dets script
        projectileScript.initProjSpeed = initProjSpeed; //sætter hastigheden
        projectileScript.turnforce = turnForce; //sætter deres turnForce
        projectileScript.isHoming = isHoming; //fortæller at den er homing eller ikke
        projectileScript.damage = rangedDamage; //sætter dets skade
        projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(GetVectorToPlayer().x, GetVectorToPlayer().y), ForceMode2D.Impulse); //tilfører den kraften
        projectile.transform.eulerAngles = GetOrientation(); //rotere den
    }

    private Vector3 GetVectorToPlayer() //finder normalvektoren mod spilleren
    {
        Vector3 vector = player.transform.position - transform.position;
        return vector.normalized * initProjSpeed;
    }
    private Vector3 GetOrientation() //finder retningen mod spilleren
    {
        float angle = Mathf.Atan2(GetVectorToPlayer().y, GetVectorToPlayer().x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, angle);
    }
    private void StartJumpCooldown() //timer til hoppet
    {
        cooldownTimer = Time.time;
        colTiming = true;
    }
    public void StartAlertTimer(){ //alerttimeren
        alertTimer = Time.time;
        alert = true;
    }

    public void PlayerEntered() //funktionen der kaldes når fjenden ser spilleren
    {
        if (isRanged) //hvis den er ranged
        {
            fireTimer = Time.time; //starter skyde timeren
        }
        if (isMelee)
        {
            StartMelee(); //starter melee angrebet
        }
        alert = false; 
        seeingPlayer = true;
    }
    public void PlayerExited() //kaldes når spilleren forlader enemiens syn
    {
        if (isMelee) //hvis den er melee kaldes at den skal stoppe med at angribe
        {
            StopMelee();
        }
        StartAlertTimer(); //starter timeren før den går tilbage til sin bane
    }
    private bool switchBool(bool oldBool) //funktion der inverterer en bool
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
    private void switchTarget() //skifter mål
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
    private void ResetTrigger(string triggerName) //funktion til at genstarte parametre på animatoren
    {
        animator.ResetTrigger(triggerName);
    }
    private void MeleeAttack() //melee angrebet
    {
        meleeWeapon.transform.Rotate(new Vector3(0, 0, -500) * Time.deltaTime); //svinger sværdet
        Debug.Log(meleeWeapon.transform.localEulerAngles.z);
        if (Mathf.Abs(meleeWeapon.transform.localEulerAngles.z - 190) < 5) //hvis den er nået til bunden af slaget genstartes det
        {
            meleeWeapon.transform.localEulerAngles = new Vector3(0, 0, 63);
        }
    }
    public void StartMelee() //starter melee
    {
        meleeWeapon.SetActive(true);
        melee = true;
    }
    public void StopMelee() //stopper melee
    {
        melee = false;
        meleeWeapon.SetActive(false);
    }
    public void dealDamage(float damage)  //funktionen der kaldes når den bliver gjort skade
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); //reference til spritet på fjenden
        health -= damage; //skader den lig med den mængde skade det der kaldte den gør
        if(health == 0) //tjek om den er død
        {
            ScoreManager.grappleScore += worth; //giv flere grapple punkter
            ScoreManager.enemiesDefeated++; //øg mængden af fjender drabt
            GameObject.Destroy(gameObject); //fjern fjenden
        }
        spriteRenderer.color = Color.red; //gør den rød
        Invoke("turnWhite", 0.1f); // gør den hvid efter 0,1 sekund så den lige når at blinke rød
    }
    private void turnWhite() //gør den hvid
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }
}
