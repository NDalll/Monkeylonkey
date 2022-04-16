using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //vigtigt da scriptet skal ændre ui-elementerne
using UnityEngine.SceneManagement; //vigtigt da scriptet skal skifte scene
using UnityEngine.InputSystem; //vigtigt da scriptet bruger unity's nye input-system

public class Player : MonoBehaviour //Dette script er ansvarlig for alt bevægelse af spilleren
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
    
    

    private void Start() //bliver kaldt på den første frame
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); //finder cameraet så vi kan bruge den senere
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>(); //finder GameControlleren så vi kan bruge den til at sende varibler til andre scener
        tail = GameObject.FindGameObjectWithTag("Tail"); //finder halen
        animator = GetComponent<Animator>(); //finder animatoren defineret i inspektoren
        nearGPoints = new List<GameObject>(); //listen af potentielle grapplepoints
        healthBar.maxValue = health; //sikre at healthbaren starter fyldt ud
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple"); //sætter alle grapplepunkterne i listen
        lr = gameObject.GetComponent<LineRenderer>(); //finder linerenderen til halen i inspektoren
        
    }
    private void FixedUpdate() //bliver kaldt i en sat framerate, så programmet ikke bliver langsommere hvis ens computer er langsommere
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>(); //læser inputtet fra vores inputcontroller (Unity's nye input system)
        
        if (!isGrappling) //sikre at dette ikke køre mens man grappler
        {
            if (input.x < 0) //flipper spilleren så man vender den vej der passer til inputtet
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(-1, 1, 1); //vi flipper var at ændre i gameobjectets scale for også at flippe dets children
                tail.transform.localScale = new Vector3(-1, 1, 1); //vi flipper var at ændre i gameobjectets scale for også at flippe dets children
                isFlipped = true;

            }
            if (input.x > 0) //flipper spilleren så man vender den vej der passer til inputtet
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(1, 1, 1); //vi flipper var at ændre i gameobjectets scale for også at flippe dets children
                tail.transform.localScale = new Vector3(1, 1, 1); //vi flipper var at ændre i gameobjectets scale for også at flippe dets children
                isFlipped = false;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (input.x == 0) //stopper løbeanimationen
        {
            animator.SetBool("Running", false);
        }


        float targetSpeed = input.x * moveSpeed; //Beregner den hastighed vi gerne vil opnå
        float speedDif = targetSpeed - RB.velocity.x; //Finder forskellen mellem den nuværende hastighed og den ønskede hastighed
        float accelRate;
        
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel : runDeccel; //ændre accelerationen baseret på om vi forsøge at stoppe op eller løbe hurtigere.
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif); //tilføjer accelerationen til hastigheden. Dette er opløftet i velPower for at accelerationen øges ved højere hastigheder. Til sidst ganges det med Sign for at få retningen tilbage efter vi normaliserede hastigeden
        
        RB.AddForce(movement * Vector2.right); //tilføjer hastigheden i x-aksen

        //Friction:
        if (IsGrounded() && input.x == 0 && isGrappling != true) //sikre at spilleren kun bliver tilført friktion når de er på jorden og ikke inputter en retning, samt at de ikke grappler.
        {
            float amount = Mathf.Min(Mathf.Abs(RB.velocity.x), Mathf.Abs(frictionAmount)); //finder den mindste værdi mellem hastigheden og spillerens friktion
            amount *= Mathf.Sign(RB.velocity.x); //sikre at retningen friktionen bliver tilføjet i er korrekt
            RB.AddForce(Vector2.right * -amount, ForceMode2D.Impulse); //tilfører spilleren friktionen
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
        if (!isDead) //tæller hvor lang tid spilleren har været i live
        {
            timePlayed =+ timePlayed + Time.deltaTime;
        }

        //jump
        if ((IsGrounded()||(lastGroundedTime < coyoteTime && lastJumpedTime > coyoteTime * 2)) && playerControls.Default.Jump.triggered) //sikre at spilleren kun kan hoppe når de enten er på jorden eller når tiden siden spilleren sidst forlod jorden er lavere end coyotetimen
        {
            Jump(); //kalder hop funktionen
            animator.SetBool("Jumping", true); //starter hop animationen
        }

        //healthbar
        healthBar.value = health; //opdatere healthbaren så den passer til spillerens liv
        if (health <= 0) //Tjekker om spilleren bør være død
        {
            isDead = true; //fortæller de andre scripts at spillen er død
            invincible = false; //stopper spillerens IFrames
            playerControls.Disable(); //gør så spilleren ikke kan bevæge sig
            gamecontroller.gameWon = false; //fortæller gamecontrolleren at spilleren tabte
            Invoke("LoadGameOverScene", 1f); //kalder LoadGameOverScene efter 1 sekund, så spilleren kan nå at registrer at de døde inden spillet skifter scene.
        }
        
        if (playerControls.Default.Grapple.WasReleasedThisFrame()) //tjekker om spilleren gav slip på grapple knappen og stopper spilleren fra at graple hvis det er sandt
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

        nearestGrapple = GetNearstGrapple(); //GetNearestGrapple for at finde hvilket grapplepoint der er tættest på spilleren
        foreach(GameObject x in grapplePoints) //går igennem alle grapplepunkterne og sætter deres størrelse til det normale
        {
            x.transform.localScale = new Vector3(1, 1, 1);
        }
        if(nearestGrapple != null) //sikre at der ikke sker errors
        {
            nearestGrapple.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); //gør det nærmeste grapplepunkt stort
        }
        Grapple();//kalder Grapple funktionen
        if (IsGrounded()) //Kalder IsGrounded funktionen til at tjekke om spilleren står på jorden, hvis de gør stoppes fall animationen
        {
            animator.SetBool("Falling", false);
        }
        if (IsGrounded() == false) //hvis de ikke er på jorden startes fall animationen
        {
            animator.SetBool("Falling", true);
        }
        if (invincible) //hvis spilleren er udødelig
        {
            if(iFrameBlinkCount != iFrameBlinks) //hvis spilleren ikke har blinket det antal gange de skal:
            {
                if (Time.time - iFrameTimer >= iFrameInterval) //hvis der er gået lang nok tid siden sidste blink blinker spilleren igen
                {
                    if (player.enabled) //hvis spilleren er enabled bliver han disabled
                    {
                        player.enabled = false;
                    }
                    else //hvis spilleren er disabled bliver han enabled og blink counteren tælles op
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

                BananaPickUp bananaScript = banana.GetComponent<BananaPickUp>(); //finder scriptet på bananen
                bananaScript.isPickUp = false; //ændre den til et projektil
                bananaScript.rotateSpeed = rotateSpeed * Random.Range(0.4f, 1); //giver bananen en tilfældig rotation
                banana.transform.position = transform.position; //sikre at bananen er ovenpå spilleren
                Rigidbody2D brb = banana.GetComponent<Rigidbody2D>(); //finder bananens rigidbody2d
                Vector3 mousePos = Mouse.current.position.ReadValue(); //finder musens position
                Vector3 aimDirection = Cam.ScreenToWorldPoint(mousePos); //omdanner mousepos som er et screenspace til et worldspace
                aimDirection = new Vector3(aimDirection.x - RB.position.x, aimDirection.y - RB.position.y, 0f).normalized; //finder vektoren mellem spilleren og musen
                Vector2 Force = new Vector2(aimDirection.x, aimDirection.y+0.35f) * fireMagnetuide; //finder kraften der skal tilføres til bananen
                brb.bodyType = RigidbodyType2D.Dynamic; //ændre rigidbody typen fra static til dynamic så vi kan tilfører den krafter
                brb.AddForce(new Vector2(Force.x + RB.velocity.x/10, Force.y), ForceMode2D.Impulse); // tilfører kraften samt en tiende-del af spilleren hastighed så bananen ikke er for meget langsommere end spilleren
                ScoreManager.bananaScore--; //sænker mængden af bananer spilleren har
            }
        }
        if (playerControls.Default.Throw.WasPressedThisFrame()) //Denne funktion fungere på samme måde som banan kastet bare med variablerne for kastepunkterne i stedet
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

    private Vector3 GetOrientationGrapple(Vector2 direction) //denne funktion er den der roterere spilleren når de grappeler
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
        if(canGrapple) //tjekker om der er nogen punkter i nærheden
        {
            if (playerControls.Default.Grapple.IsPressed()) //tjekker om grapple er blevet inputtet
            {

                if (isGrappling == false) //låser spilleren fast til det grapple point de grappler til
                {
                    isGrappling = true;
                    animator.SetBool("Grappeling", true);
                    gPosition = nearestGrapple.transform.position;
                    grappleP = nearestGrapple;
                }

                Vector2 direction = new Vector2(gPosition.x - gameObject.transform.position.x, gPosition.y - gameObject.transform.position.y).normalized; //finder vektoren mellem spilleren og punktet
                direction = new Vector2(direction.x * grappleMultipier, direction.y * grappleMultipier); //lægger grapple multiplieren til denne retning
                Vector3 tailPoint = transform.GetChild(0).position; // finder halepunktet på spilleren (child af spilleren)
                lr.enabled = true; //Enabler linerendereren
                lr.SetPosition(0, tailPoint); //sætter dens position til at være imellem halepunktet og grapplepunktet
                lr.SetPosition(1, grappleP.transform.position);
                Vector3 angle = GetOrientationGrapple(direction); //finder rotationen spilleren skal vende ved at kalde GetOrientationGrapple
                player.transform.eulerAngles = angle; //sætter rotationen
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
    private GameObject GetNearstGrapple() //funktionen til at finde det nærmeste grapple punkt
    {
        if (nearGPoints.Count != 0 && nearGPoints[0].GetComponent<Grapplepoint>().isThrown == false)//tjekker om der er punkter i listen og at det punkt der er tættest på ikke er i gang med at flyve
        {
            canGrapple = true;
            GameObject nearstPoint = nearGPoints[0]; //sætter det nærmeste punkt til det første punkt i listen
            for (int i = 1; i < nearGPoints.Count; i++) //for loop der går igennem hvert punkt i listen og finder det nærmeste punkt
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
        return null; //hvis ikke den overstående if-sætning er sand returneres NULL
    }


    private bool IsGrounded() //Checker om spiller er grounded
    {
        
        float extraHeightRaycast = 0.4f; //den ekstra dybde der skal tjekkes under spilleren
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightRaycast, platformLayerMask); //Raycaster en kasse under spilleren
        Color rayColor;
        if (raycastHit.collider != null) //hvis den er i kontakt med noget der er inde for den layermask defineret i kassen
        {
            rayColor = Color.green; //sætter farven af kassen til grøn (Kan ikke ses i spillet, til debugging)
            lastGroundedTime = 0; //genstarter last grounded timeren

        } else {
            rayColor = Color.red; //sætter farven af kassen til rød (Kan ikke ses i spillet, til debugging)
        }
        //Tegner de 3 linje i debuggeren der udgør hvor kassen er:
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
        if (RB.velocity.y < 0) //Hvis spilleren falder vil hoppet stoppe dem inden de tilføres en kraft
        {
            RB.velocity = new Vector2(RB.velocity.x, 0);
        }

        RB.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse); //tilføjer kraften af jumpheight til spilleren
        lastJumpedTime = 0; //genstarter last jumped timeren
    }

   
    public void dealDamage(float damage) //funktionen der bliver kaldt når spilleren tager skade
    {
        if(invincible == false && !isDead) //tjekker om spilleren er udødelig
        {
            health -= damage; //fjerner den mængde liv der bliver sendt når funktionen kaldes
            invincible = true; //gør spilleren udødelig
            iFrameTimer = Time.time; //genstarter iFrametimeren
        }
    }
    public void LoadGameOverScene() //loader gameoverscenen
    {
        //definerer variblerne i gamecontrolleren så de kan sendes over når scenen skifter og spilleren forsvinder:
        gamecontroller.timePlayed = timePlayed; 
        gamecontroller.enemiesDefeated = ScoreManager.enemiesDefeated;
        gamecontroller.bananas = ScoreManager.bananaScore;
        gamecontroller.bananasCollected = ScoreManager.totalBananas;
        gamecontroller.floorsBeaten = ScoreManager.stagesCleared;
        SceneManager.LoadScene("Gameover"); //skifter scenen
    }

    private void Awake() //bliver kaldt før den første frame hvor objectet er aktiv
    {
        playerControls = new PlayerControls(); //definere inputcontrolleren
    }
    private void OnEnable() //bliver kaldt når den bliver enablelet
    {
        playerControls.Enable(); //enabler inputcontrolleren
    }
    private void OnDisable() //bliver kaldt når den bliver disablelet
    {
        playerControls.Disable(); //disabler inputcontrolleren
    }
}
