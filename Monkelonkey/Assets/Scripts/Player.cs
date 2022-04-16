using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //vigtigt da scriptet skal �ndre ui-elementerne
using UnityEngine.SceneManagement; //vigtigt da scriptet skal skifte scene
using UnityEngine.InputSystem; //vigtigt da scriptet bruger unity's nye input-system

public class Player : MonoBehaviour //Dette script er ansvarlig for alt bev�gelse af spilleren
{
    private Gamecontroller gamecontroller;
    private Camera Cam;
    public float moveSpeed;
    public float maxFallSpeed;
    public Sprite fallSprite;
    public float jumpHeight;
    public SpriteRenderer player;
    public Rigidbody2D RB;
    public float runAccel;
    public float runDeccel;
    public float velPower;
    public float frictionAmount;
    private PlayerControls playerControls;
    public CapsuleCollider2D playerCollider;
    public float coyoteTime;
    private float lastGroundedTime = 0;
    private float lastJumpedTime = 0;
    private float timePlayed = 0;
    public float grappleMultipier;
    private bool isGrappling = false;
    private bool isFlipped;

    public float health;
    public Slider healthBar;
    [System.NonSerialized]
    public bool isDead;
    public float grappleOffset;

    private Vector3 gPosition;
    private GameObject grappleP;
    private GameObject[] grapplePoints;
    [System.NonSerialized]
    public List<GameObject> nearGPoints;
    private LineRenderer lr;
    private GameObject nearestGrapple;
    private bool canGrapple;

    private GameObject tail;
    public int iFrameBlinks;
    private int iFrameBlinkCount = 0;
    private float iFrameTimer;
    public float iFrameInterval;
    private bool invincible;
    private Animator animator; 
    [SerializeField] private LayerMask platformLayerMask;

    public GameObject throwingGrapple;
    public GameObject banana;
    public float fireAngle;
    public float fireMagnetuide;
    public float grappleFireMagnetuide;
    public float rotateSpeed;
    
    

    private void Start() //bliver kaldt p� den f�rste frame
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); //finder cameraet s� vi kan bruge den senere
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>(); //finder GameControlleren s� vi kan bruge den til at sende varibler til andre scener
        tail = GameObject.FindGameObjectWithTag("Tail"); //finder halen
        animator = GetComponent<Animator>(); //finder animatoren defineret i inspektoren
        nearGPoints = new List<GameObject>(); //listen af potentielle grapplepoints
        healthBar.maxValue = health; //sikre at healthbaren starter fyldt ud
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple"); //s�tter alle grapplepunkterne i listen
        lr = gameObject.GetComponent<LineRenderer>(); //finder linerenderen til halen i inspektoren
        
    }
    private void FixedUpdate() //bliver kaldt i en sat framerate, s� programmet ikke bliver langsommere hvis ens computer er langsommere
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>(); //l�ser inputtet fra vores inputcontroller (Unity's nye input system)
        
        if (!isGrappling) //sikre at dette ikke k�re mens man grappler
        {
            if (input.x < 0) //flipper spilleren s� man vender den vej der passer til inputtet
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(-1, 1, 1); //vi flipper var at �ndre i gameobjectets scale for ogs� at flippe dets children
                tail.transform.localScale = new Vector3(-1, 1, 1); //vi flipper var at �ndre i gameobjectets scale for ogs� at flippe dets children
                isFlipped = true;

            }
            if (input.x > 0) //flipper spilleren s� man vender den vej der passer til inputtet
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(1, 1, 1); //vi flipper var at �ndre i gameobjectets scale for ogs� at flippe dets children
                tail.transform.localScale = new Vector3(1, 1, 1); //vi flipper var at �ndre i gameobjectets scale for ogs� at flippe dets children
                isFlipped = false;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (input.x == 0) //stopper l�beanimationen
        {
            animator.SetBool("Running", false);
        }


        float targetSpeed = input.x * moveSpeed; //Beregner den hastighed vi gerne vil opn�
        float speedDif = targetSpeed - RB.velocity.x; //Finder forskellen mellem den nuv�rende hastighed og den �nskede hastighed
        float accelRate;
        
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel : runDeccel; //�ndre accelerationen baseret p� om vi fors�ge at stoppe op eller l�be hurtigere.
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif); //tilf�jer accelerationen til hastigheden. Dette er opl�ftet i velPower for at accelerationen �ges ved h�jere hastigheder. Til sidst ganges det med Sign for at f� retningen tilbage efter vi normaliserede hastigeden
        
        RB.AddForce(movement * Vector2.right); //tilf�jer hastigheden i x-aksen

        //Friction:
        if (IsGrounded() && input.x == 0 && isGrappling != true) //sikre at spilleren kun bliver tilf�rt friktion n�r de er p� jorden og ikke inputter en retning, samt at de ikke grappler.
        {
            float amount = Mathf.Min(Mathf.Abs(RB.velocity.x), Mathf.Abs(frictionAmount)); //finder den mindste v�rdi mellem hastigheden og spillerens friktion
            amount *= Mathf.Sign(RB.velocity.x); //sikre at retningen friktionen bliver tilf�jet i er korrekt
            RB.AddForce(Vector2.right * -amount, ForceMode2D.Impulse); //tilf�rer spilleren friktionen
        }
    }
    
    private void Update() //bliver kaldt hver frame
    {
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple");
        //Max fallspeed:
        if (RB.velocity.y < maxFallSpeed * -1)
        {
            RB.velocity = new Vector2 (RB.velocity.x, maxFallSpeed * -1);
        }

        //timer (for coyote time):
        lastGroundedTime += Time.deltaTime; 
        lastJumpedTime += Time.deltaTime;

        //playtime:
        if (!isDead) //t�ller hvor lang tid spilleren har v�ret i live
        {
            timePlayed =+ timePlayed + Time.deltaTime;
        }

        //jump
        if ((IsGrounded()||(lastGroundedTime < coyoteTime && lastJumpedTime > coyoteTime * 2)) && playerControls.Default.Jump.triggered) //sikre at spilleren kun kan hoppe n�r de enten er p� jorden eller n�r tiden siden spilleren sidst forlod jorden er lavere end coyotetimen
        {
            Jump(); //kalder hop funktionen
            animator.SetBool("Jumping", true); //starter hop animationen
        }

        //healthbar
        healthBar.value = health; //opdatere healthbaren s� den passer til spillerens liv
        if (health <= 0) //Tjekker om spilleren b�r v�re d�d
        {
            isDead = true; //fort�ller de andre scripts at spillen er d�d
            invincible = false; //stopper spillerens IFrames
            playerControls.Disable(); //g�r s� spilleren ikke kan bev�ge sig
            gamecontroller.gameWon = false; //fort�ller gamecontrolleren at spilleren tabte
            Invoke("LoadGameOverScene", 1f); //kalder LoadGameOverScene efter 1 sekund, s� spilleren kan n� at registrer at de d�de inden spillet skifter scene.
        }
        
        if (playerControls.Default.Grapple.WasReleasedThisFrame()) //tjekker om spilleren gav slip p� grapple knappen og stopper spilleren fra at graple hvis det er sandt
        {
            isGrappling = false;
            animator.SetBool("Grappeling", false); 
            if (grappleP != null)
            {
                if (grappleP.GetComponent<Grapplepoint>().trowable){ //hvis spilleren graplede til et midlertidigt grapplepoint bliver det slettet
                    Destroy(grappleP);
                }
            }
        }

        nearestGrapple = GetNearstGrapple(); //GetNearestGrapple for at finde hvilket grapplepoint der er t�ttest p� spilleren
        foreach(GameObject x in grapplePoints) //g�r igennem alle grapplepunkterne og s�tter deres st�rrelse til det normale
        {
            x.transform.localScale = new Vector3(1, 1, 1);
        }
        if(nearestGrapple != null) //sikre at der ikke sker errors
        {
            nearestGrapple.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); //g�r det n�rmeste grapplepunkt stort
        }
        Grapple();//kalder Grapple funktionen
        if (IsGrounded()) //Kalder IsGrounded funktionen til at tjekke om spilleren st�r p� jorden, hvis de g�r stoppes fall animationen
        {
            animator.SetBool("Falling", false);
        }
        if (IsGrounded() == false) //hvis de ikke er p� jorden startes fall animationen
        {
            animator.SetBool("Falling", true);
        }
        if (invincible) //hvis spilleren er ud�delig
        {
            if(iFrameBlinkCount != iFrameBlinks) //hvis spilleren ikke har blinket det antal gange de skal:
            {
                if (Time.time - iFrameTimer >= iFrameInterval) //hvis der er g�et lang nok tid siden sidste blink blinker spilleren igen
                {
                    if (player.enabled) //hvis spilleren er enabled bliver han disabled
                    {
                        player.enabled = false;
                    }
                    else //hvis spilleren er disabled bliver han enabled og blink counteren t�lles op
                    {
                        player.enabled = true;
                        iFrameBlinkCount++;
                    }
                    iFrameTimer = Time.time;
                    
                }
            }
            else //stopper IFramesne
            {
                iFrameBlinkCount = 0;
                invincible = false;
            }   
        }
        if (playerControls.Default.Attack.WasPressedThisFrame()) //hvis spilleren har inputtet et angreb
        {
            if (ScoreManager.bananaScore > 0) //tjekker om spilleren har nok bananer
            {
                GameObject banana = Instantiate(this.banana); //laver en banan

                BananaPickUp bananaScript = banana.GetComponent<BananaPickUp>(); //finder scriptet p� bananen
                bananaScript.isPickUp = false; //�ndre den til et projektil
                bananaScript.rotateSpeed = rotateSpeed * Random.Range(0.4f, 1); //giver bananen en tilf�ldig rotation
                banana.transform.position = transform.position; //sikre at bananen er ovenp� spilleren
                Rigidbody2D brb = banana.GetComponent<Rigidbody2D>(); //finder bananens rigidbody2d
                Vector3 mousePos = Mouse.current.position.ReadValue(); //finder musens position
                Vector3 aimDirection = Cam.ScreenToWorldPoint(mousePos); //omdanner mousepos som er et screenspace til et worldspace
                aimDirection = new Vector3(aimDirection.x - RB.position.x, aimDirection.y - RB.position.y, 0f).normalized; //finder vektoren mellem spilleren og musen
                Vector2 Force = new Vector2(aimDirection.x, aimDirection.y+0.35f) * fireMagnetuide; //finder kraften der skal tilf�res til bananen
                brb.bodyType = RigidbodyType2D.Dynamic; //�ndre rigidbody typen fra static til dynamic s� vi kan tilf�rer den krafter
                brb.AddForce(new Vector2(Force.x + RB.velocity.x/10, Force.y), ForceMode2D.Impulse); // tilf�rer kraften samt en tiende-del af spilleren hastighed s� bananen ikke er for meget langsommere end spilleren
                ScoreManager.bananaScore--; //s�nker m�ngden af bananer spilleren har
            }
        }
        if (playerControls.Default.Throw.WasPressedThisFrame()) //Denne funktion fungere p� samme m�de som banan kastet bare med variablerne for kastepunkterne i stedet
        {
            if (ScoreManager.grappleScore > 0) 
            {
                GameObject thrownPoint = Instantiate(throwingGrapple);
                thrownPoint.transform.position = transform.position;
                Rigidbody2D grb = thrownPoint.GetComponent<Rigidbody2D>();
                Vector3 mousePos = Mouse.current.position.ReadValue();
                Vector3 aimDirection = Cam.ScreenToWorldPoint(mousePos);
                aimDirection = new Vector3(aimDirection.x - RB.position.x, aimDirection.y - RB.position.y, 0f).normalized;
                Vector2 Force = new Vector2(aimDirection.x, aimDirection.y) * grappleFireMagnetuide;
                grb.AddForce(new Vector2(Force.x, Force.y), ForceMode2D.Impulse);
                ScoreManager.grappleScore--;
            }
        }
    }

    private Vector3 GetOrientationGrapple(Vector2 direction) //denne funktion er den der roterere spilleren n�r de grappeler
    {
        if (!isFlipped) //sikre at det ser rigtigt ud om spilleren er flipped eller ikke
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //
            return new Vector3(0, 0, angle + 180 + grappleOffset);
        }
        else
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return new Vector3(0, 0, angle - grappleOffset);
        }
    }

    private void Grapple() //Funktionen ansvarlig for grapple
    {
        if(canGrapple) //tjekker om der er nogen punkter i n�rheden
        {
            if (playerControls.Default.Grapple.IsPressed()) //tjekker om grapple er blevet inputtet
            {

                if (isGrappling == false) //l�ser spilleren fast til det grapple point de grappler til
                {
                    isGrappling = true;
                    animator.SetBool("Grappeling", true);
                    gPosition = nearestGrapple.transform.position;
                    grappleP = nearestGrapple;
                }

                Vector2 direction = new Vector2(gPosition.x - gameObject.transform.position.x, gPosition.y - gameObject.transform.position.y).normalized; //finder vektoren mellem spilleren og punktet
                direction = new Vector2(direction.x * grappleMultipier, direction.y * grappleMultipier); //l�gger grapple multiplieren til denne retning
                Vector3 tailPoint = transform.GetChild(0).position; // finder halepunktet p� spilleren (child af spilleren)
                lr.enabled = true; //Enabler linerendereren
                lr.SetPosition(0, tailPoint); //s�tter dens position til at v�re imellem halepunktet og grapplepunktet
                lr.SetPosition(1, grappleP.transform.position);
                Vector3 angle = GetOrientationGrapple(direction); //finder rotationen spilleren skal vende ved at kalde GetOrientationGrapple
                player.transform.eulerAngles = angle; //s�tter rotationen
                RB.AddForce(direction * Time.deltaTime * 1000); //skubber spilleren mod punktet
            }
            else
            {
                lr.enabled = false; //disabler linerenderen
            }
        }
        else
        {
            lr.enabled = false; //disabler linerenderen
        }
        
    }
    private GameObject GetNearstGrapple() //funktionen til at finde det n�rmeste grapple punkt
    {
        if (nearGPoints.Count != 0 && nearGPoints[0].GetComponent<Grapplepoint>().isThrown == false)//tjekker om der er punkter i listen og at det punkt der er t�ttest p� ikke er i gang med at flyve
        {
            canGrapple = true;
            GameObject nearstPoint = nearGPoints[0]; //s�tter det n�rmeste punkt til det f�rste punkt i listen
            for (int i = 1; i < nearGPoints.Count; i++) //for loop der g�r igennem hvert punkt i listen og finder det n�rmeste punkt
            {
                if (Vector2.Distance(nearGPoints[i].transform.position, gameObject.transform.position) < Vector2.Distance(nearstPoint.transform.position, gameObject.transform.position))
                {
                    if(nearGPoints[i].GetComponent<Grapplepoint>().isThrown == false)
                    {
                        nearstPoint = nearGPoints[i];
                    }     
                }
            }
            return nearstPoint; //returnere det fundne punkt
        }
        if (playerControls.Default.Grapple.IsPressed() == false) //hvis spilleren giver slip bliver cangrapple false
        {
            canGrapple = false;
        }
        return null; //hvis ikke den overst�ende if-s�tning er sand returneres NULL
    }


    private bool IsGrounded() //Checker om spiller er grounded
    {
        
        float extraHeightRaycast = 0.4f; //den ekstra dybde der skal tjekkes under spilleren
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightRaycast, platformLayerMask); //Raycaster en kasse under spilleren
        Color rayColor;
        if (raycastHit.collider != null) //hvis den er i kontakt med noget der er inde for den layermask defineret i kassen
        {
            rayColor = Color.green; //s�tter farven af kassen til gr�n (Kan ikke ses i spillet, til debugging)
            lastGroundedTime = 0; //genstarter last grounded timeren

        } else {
            rayColor = Color.red; //s�tter farven af kassen til r�d (Kan ikke ses i spillet, til debugging)
        }
        //Tegner de 3 linje i debuggeren der udg�r hvor kassen er:
        Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightRaycast), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightRaycast), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + extraHeightRaycast), Vector2.right * (playerCollider.bounds.extents.x), rayColor);
        return raycastHit.collider != null; //retunerer om der var en colision
    }

    public void JumpFinished() //slutter hop animationen
    {
        animator.SetBool("Jumping", false);
    }

    public void Jump() //Funktionen for hoppet
    {
        if (RB.velocity.y < 0) //Hvis spilleren falder vil hoppet stoppe dem inden de tilf�res en kraft
        {
            RB.velocity = new Vector2(RB.velocity.x, 0);
        }

        RB.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse); //tilf�jer kraften af jumpheight til spilleren
        lastJumpedTime = 0; //genstarter last jumped timeren
    }

   
    public void dealDamage(float damage) //funktionen der bliver kaldt n�r spilleren tager skade
    {
        if(invincible == false && !isDead) //tjekker om spilleren er ud�delig
        {
            health -= damage; //fjerner den m�ngde liv der bliver sendt n�r funktionen kaldes
            invincible = true; //g�r spilleren ud�delig
            iFrameTimer = Time.time; //genstarter iFrametimeren
        }
    }
    public void LoadGameOverScene() //loader gameoverscenen
    {
        //definerer variblerne i gamecontrolleren s� de kan sendes over n�r scenen skifter og spilleren forsvinder:
        gamecontroller.timePlayed = timePlayed; 
        gamecontroller.enemiesDefeated = ScoreManager.enemiesDefeated;
        gamecontroller.bananas = ScoreManager.bananaScore;
        gamecontroller.bananasCollected = ScoreManager.totalBananas;
        gamecontroller.floorsBeaten = ScoreManager.stagesCleared;
        SceneManager.LoadScene("Gameover"); //skifter scenen
    }

    private void Awake() //bliver kaldt f�r den f�rste frame hvor objectet er aktiv
    {
        playerControls = new PlayerControls(); //definere inputcontrolleren
    }
    private void OnEnable() //bliver kaldt n�r den bliver enablelet
    {
        playerControls.Enable(); //enabler inputcontrolleren
    }
    private void OnDisable() //bliver kaldt n�r den bliver disablelet
    {
        playerControls.Disable(); //disabler inputcontrolleren
    }
}
