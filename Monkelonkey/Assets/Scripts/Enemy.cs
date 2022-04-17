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

    [Header("Walking Enemy")] //variablerne for de g�ende fjender:
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
            meleeWeapon = Instantiate(meleeWeaponPrefab, gameObject.transform); //laver v�benet p� spillerens position 
            meleeWeapon.GetComponent<MeleeWeapon>().damage = meleeDamage; //definerer skaden af v�bnet
            meleeWeapon.SetActive(false); //disabler v�benet
        }
        
        scale = transform.localScale; //definere scalen til enemiens localscale
        player = GameObject.FindWithTag("Player"); //finder playeren
        RB = GetComponent<Rigidbody2D>(); //definerer rigdidbodien p� fjenden
        animator = GetComponent<Animator>(); //definerer animatoren p� fjenden
        if (isWalking) //tjekker om det er en g�ende fjende
        {
            startPos = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, 0); //s�tter startpositionen til positionen af startpunket defineret i inspektoren
            endPos = endPoint.transform.position; //s�tter slutpositionen til positionen af startpunket defineret i inspektoren

            transform.position = startPos; //flytter fjenden til startpunktet
            currTarget = endPos; //s�tter dens m�l til slutpositionen
        }
        playerTrigger = transform.GetChild(0).GetComponent<PlayerTrigger>(); //finder colideren p� der tjekker for playeren
        playerTrigger.damage = bodyDamage; //definere hvor meget skade kroppen g�r til spilleren
        ogRunSpeed = runSpeed; //gemmer den originale Runspeed
        ogWalkSpeed = walkSpeed; //gemmer den originale Walkspeed
    }
    private void Update() //kaldes hver frame
    {
        if (isJumping) //tjekker om det er en hoppende fjende
        {
            if (colTiming) //Timeren for hvor ofte den hopper
            {
                if (Time.time - cooldownTimer >= jumpCooldown) //tjekker om der er cooldownet b�r v�re ovre
                {
                    colTiming = false; //stopper timeren
                    animator.SetTrigger("doneCooldown"); //fort�ller animatoren at ventetiden er ovre
                }
            }
        }

        if (seeingPlayer) //tjekker om den kan se spilleren
        {
            if (isRanged) //hvis den er ranged vil den skyde efter spilleren p� en timer
            {
                if (Time.time - fireTimer >= fireRate)
                {
                    fireProjectile();
                    fireTimer = Time.time;
                }
            }
        }

        if (isWalking) //hvis det er en g�ende fjende
        {
            if (seeingPlayer && isFollowing) //hvis den kan se spilleren og er en following fjende
            {
                Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, 0).normalized; //finder retningen mod spilleren
                transform.Translate(dir * Time.deltaTime * runSpeed); //flytte fjenden
                SetLookDirection(dir); //kalder funktionen der vender fjenden
            }
            else //Dette er sandt n�r den enten ikke kan se spilleren eller ikke er en following enemy
            {
                Vector2 dir = new Vector2(currTarget.x - transform.position.x, 0).normalized; //finder retingen
                transform.Translate(dir * Time.deltaTime * walkSpeed); //flytter fjenden
                SetLookDirection(dir); //kalder funktionen der vender fjenden
            }
            float distance = CalcXDistance(); //kalder funktionen der finder afstanden mellem fjendens m�l og fjenden
            if (distance < 0.5f) //hvis afstanden er lav nok, vender den sig om
            {
                switchTarget();
            }
        }
        if (isMelee) //tjekker om det er en melee enemy
        {
            if (melee) //tjekker om v�benet er aktivt
            {
                MeleeAttack(); //kalder angrebet
            }
        }
        if (alert) //hvis den ser spilleren vil den forts�tte i et stykke tid selvom spilleren er ude af dens syn
        {
            if (Time.time - alertTimer >= alertTime) 
            {
                alert = false;
                seeingPlayer = false;
            }
        }
    }

    float CalcXDistance()//finder afstanden mellem fjenden og dens m�l
    {
        float distance;
        distance = Mathf.Abs(transform.position.x - currTarget.x);
        return distance;
    }
    private void SetLookDirection(Vector2 dir) //finder retningen mod m�let
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

    private void OnTriggerEnter2D(Collider2D collision) //kaldes n�r et objekt g�r ind i dens collider
    {
        if (isJumping) //hvis det er en hoppende fjende
        {
            if (collision.CompareTag("Ground")) //tjekker om det var jorden den colliderede med
            {
                if (RB.velocity.y <= 0 && firstSpawn != true) //tjekker om den falder og det ikke er det f�rste hop
                {
                    Debug.Log("groundHit");
                    animator.SetTrigger("groundHit"); //fot�ller animatoren at den ramte jorden
                }
                else
                {
                    firstSpawn = false; //ellers blive det fortalt at det ikke l�ngere er det f�rste hop
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //n�r noget forlader colideren
    {
        if (isWalking && !seeingPlayer) //hvis den er walking men ikke ser playeren
        {
            if (collision.CompareTag("WayPoint")) //hvis det er et waypoint
            {
                collision.GetComponent<CircleCollider2D>().enabled = true; //enabler waypointets collider
            }
        }
    }
    private void Jump() //funktionen der f�r den til at hoppe
    {
        float radAngle = jumpAngle * Mathf.Rad2Deg; //finder vinklen den skal hoppe i
        if (jumpCount == jumps) //hvis den har hoppet det antal gange den skal
        {
            jumpCount = 0; //genstarter opt�llingen
            forward = switchBool(forward); //skifter m�l
            gameObject.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1); //v�nder den om

        }
        if (forward) //hvis den hopper fremad hopper den med vinklen fremad
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle) * -1, Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++; //t�ller op
        }
        else  //hvis den hopper bagud hopper den med vinklen bagud
        {
            RB.AddForce(new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)) * jumpMagnitude, ForceMode2D.Impulse);
            jumpCount++; //t�ller op
        }
    }
    public void LedgeJump()//hopper over en kant hvis den st�der p� den
    {
        if (!ledgeJumpCooldown)
        {
            RB.AddForce(Vector2.up * 9, ForceMode2D.Impulse);//skubber den opad
            ledgeJumpCooldown = true; 
            runSpeed = 0; //s�tter dens fart til 0 s� den ikke sider fast i v�ggen
            walkSpeed = 0; //s�tter dens fart til 0 s� den ikke sider fast i v�ggen
            Invoke("LegdeJumpCooldown", 1f); //starter en timer p� 1 sekund
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
        projectile.transform.position = transform.position + new Vector3(0, .5f, 0);//s�tter den p� fjendens position
        Projectile projectileScript = projectile.GetComponent<Projectile>();//finder dets script
        projectileScript.initProjSpeed = initProjSpeed; //s�tter hastigheden
        projectileScript.turnforce = turnForce; //s�tter deres turnForce
        projectileScript.isHoming = isHoming; //fort�ller at den er homing eller ikke
        projectileScript.damage = rangedDamage; //s�tter dets skade
        projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(GetVectorToPlayer().x, GetVectorToPlayer().y), ForceMode2D.Impulse); //tilf�rer den kraften
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

    public void PlayerEntered() //funktionen der kaldes n�r fjenden ser spilleren
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
    public void PlayerExited() //kaldes n�r spilleren forlader enemiens syn
    {
        if (isMelee) //hvis den er melee kaldes at den skal stoppe med at angribe
        {
            StopMelee();
        }
        StartAlertTimer(); //starter timeren f�r den g�r tilbage til sin bane
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
    private void switchTarget() //skifter m�l
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
    private void ResetTrigger(string triggerName) //funktion til at genstarte parametre p� animatoren
    {
        animator.ResetTrigger(triggerName);
    }
    private void MeleeAttack() //melee angrebet
    {
        meleeWeapon.transform.Rotate(new Vector3(0, 0, -500) * Time.deltaTime); //svinger sv�rdet
        Debug.Log(meleeWeapon.transform.localEulerAngles.z);
        if (Mathf.Abs(meleeWeapon.transform.localEulerAngles.z - 190) < 5) //hvis den er n�et til bunden af slaget genstartes det
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
    public void dealDamage(float damage)  //funktionen der kaldes n�r den bliver gjort skade
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); //reference til spritet p� fjenden
        health -= damage; //skader den lig med den m�ngde skade det der kaldte den g�r
        if(health == 0) //tjek om den er d�d
        {
            ScoreManager.grappleScore += worth; //giv flere grapple punkter
            ScoreManager.enemiesDefeated++; //�g m�ngden af fjender drabt
            GameObject.Destroy(gameObject); //fjern fjenden
        }
        spriteRenderer.color = Color.red; //g�r den r�d
        Invoke("turnWhite", 0.1f); // g�r den hvid efter 0,1 sekund s� den lige n�r at blinke r�d
    }
    private void turnWhite() //g�r den hvid
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }
}
