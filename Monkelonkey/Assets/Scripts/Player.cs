using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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

    

    private void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gamecontroller = GameObject.FindGameObjectWithTag("Gamecontroller").GetComponent<Gamecontroller>();
        tail = GameObject.FindGameObjectWithTag("Tail");
        animator = GetComponent<Animator>();
        nearGPoints = new List<GameObject>();
        healthBar.maxValue = health;
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple");
        lr = gameObject.GetComponent<LineRenderer>();
        
    }
    private void FixedUpdate()
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>();
        
        if (!isGrappling)
        {
            if (input.x < 0)
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                tail.transform.localScale = new Vector3(-1, 1, 1);
                isFlipped = true;

            }
            if (input.x > 0)
            {
                animator.SetBool("Running", true);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                tail.transform.localScale = new Vector3(1, 1, 1);
                isFlipped = false;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (input.x == 0)
        {
            animator.SetBool("Running", false);
        }


        float targetSpeed = input.x * moveSpeed; //calculate the direction we want to move in and our desired velocity
        float speedDif = targetSpeed - RB.velocity.x; //calculate difference between current velocity and desired velocity
        float accelRate;
        
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel : runDeccel;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        
        RB.AddForce(movement * Vector2.right);

        //Friction:
        if (IsGrounded() && input.x == 0 && isGrappling != true)
        {
            float amount = Mathf.Min(Mathf.Abs(RB.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(RB.velocity.x);
            RB.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
    
    private void Update()
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
        if (!isDead)
        {
            timePlayed =+ timePlayed + Time.deltaTime;
        }

        //jump
        if ((IsGrounded()||(lastGroundedTime < coyoteTime && lastJumpedTime > coyoteTime * 2)) && playerControls.Default.Jump.triggered)
        {
            Jump();
            animator.SetBool("Jumping", true);
        }

        //healthbar
        healthBar.value = health;
        if (health <= 0)
        {
            isDead = true;
            invincible = false;
            playerControls.Disable();
            gamecontroller.timePlayed = timePlayed;
            Invoke("LoadGameOverScene", 1f);
        }
        
        if (playerControls.Default.Grapple.WasReleasedThisFrame())
        {
            isGrappling = false;
            animator.SetBool("Grappeling", false);
            if (grappleP != null)
            {
                if (grappleP.GetComponent<Grapplepoint>().trowable){
                    Destroy(grappleP);
                }
            }
        }

        nearestGrapple = GetNearstGrapple();
        foreach(GameObject x in grapplePoints)
        {
            x.transform.localScale = new Vector3(1, 1, 1);
        }
        if(nearestGrapple != null)
        {
            nearestGrapple.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        Grapple();
        if (IsGrounded())
        {
            animator.SetBool("Falling", false);
        }
        if (IsGrounded() == false)
        {
            animator.SetBool("Falling", true);
        }
        if (invincible)
        {
            if(iFrameBlinkCount != iFrameBlinks)
            {
                if (Time.time - iFrameTimer >= iFrameInterval)
                {
                    if (player.enabled)
                    {
                        player.enabled = false;
                    }
                    else
                    {
                        player.enabled = true;
                        iFrameBlinkCount++;
                    }
                    iFrameTimer = Time.time;
                    
                }
            }
            else
            {
                iFrameBlinkCount = 0;
                invincible = false;
            }   
        }
        if (playerControls.Default.Attack.WasPressedThisFrame())
        {
            if (ScoreManager.bananaScore != 0)
            {
                GameObject banana = Instantiate(this.banana);

                BananaPickUp bananaScript = banana.GetComponent<BananaPickUp>();
                bananaScript.isPickUp = false;
                bananaScript.rotateSpeed = rotateSpeed * Random.Range(0.4f, 1);
                banana.transform.position = transform.position;
                Rigidbody2D brb = banana.GetComponent<Rigidbody2D>();
                Vector3 mousePos = Mouse.current.position.ReadValue();
                Vector3 aimDirection = Cam.ScreenToWorldPoint(mousePos);
                aimDirection = new Vector3(aimDirection.x - RB.position.x, aimDirection.y - RB.position.y, 0f).normalized;
                Debug.Log(aimDirection);
                Vector2 Force = new Vector2(aimDirection.x, aimDirection.y+0.35f) * fireMagnetuide;
                brb.bodyType = RigidbodyType2D.Dynamic;
                brb.AddForce(new Vector2(Force.x + RB.velocity.x/10, Force.y), ForceMode2D.Impulse);
                ScoreManager.bananaScore--;
            }
        }
        if (playerControls.Default.Throw.WasPressedThisFrame())
        {
            GameObject thrownPoint = Instantiate(throwingGrapple);
            thrownPoint.transform.position = transform.position;
            Rigidbody2D grb = thrownPoint.GetComponent<Rigidbody2D>();
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 aimDirection = Cam.ScreenToWorldPoint(mousePos);
            aimDirection = new Vector3(aimDirection.x - RB.position.x, aimDirection.y - RB.position.y, 0f).normalized;
            Vector2 Force = new Vector2(aimDirection.x, aimDirection.y) * grappleFireMagnetuide;
            grb.AddForce(new Vector2(Force.x, Force.y), ForceMode2D.Impulse);
        }
    }

    private Vector3 GetOrientationGrapple(Vector2 direction)
    {
        if (!isFlipped)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return new Vector3(0, 0, angle + 180 + grappleOffset);
        }
        else
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return new Vector3(0, 0, angle - grappleOffset);
        }
    }

    private void Grapple()
    {
        if(canGrapple)
        {
            if (playerControls.Default.Grapple.IsPressed())
            {

                if (isGrappling == false)
                {
                    isGrappling = true;
                    animator.SetBool("Grappeling", true);
                    gPosition = nearestGrapple.transform.position;
                    grappleP = nearestGrapple;
                }

                Vector2 direction = new Vector2(gPosition.x - gameObject.transform.position.x, gPosition.y - gameObject.transform.position.y).normalized;
                direction = new Vector2(direction.x * grappleMultipier, direction.y * grappleMultipier);
                Vector3 tailPoint = transform.GetChild(0).position;
                lr.enabled = true;
                lr.SetPosition(0, tailPoint);
                lr.SetPosition(1, grappleP.transform.position);
                Vector3 angle = GetOrientationGrapple(direction);
                player.transform.eulerAngles = angle;
                RB.AddForce(direction * Time.deltaTime * 1000);
            }
            else
            {
                lr.enabled = false;
            }
        }
        else
        {
            lr.enabled = false;
        }
        
    }
    private GameObject GetNearstGrapple()
    {
        if (nearGPoints.Count != 0 && nearGPoints[0].GetComponent<Grapplepoint>().isThrown == false)
        {
            canGrapple = true;
            GameObject nearstPoint = nearGPoints[0];
            for (int i = 1; i < nearGPoints.Count; i++)
            {
                if (Vector2.Distance(nearGPoints[i].transform.position, gameObject.transform.position) < Vector2.Distance(nearstPoint.transform.position, gameObject.transform.position))
                {
                    if(nearGPoints[i].GetComponent<Grapplepoint>().isThrown == false)
                    {
                        nearstPoint = nearGPoints[i];
                    }     
                }
            }
            return nearstPoint;
        }
        if (playerControls.Default.Grapple.IsPressed() == false)
        {
            canGrapple = false;
        }
        return null;
    }


    private bool IsGrounded() //Check if player is on ground
    {
        
        float extraHeightRaycast = 0.4f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightRaycast, platformLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
            lastGroundedTime = 0;

        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightRaycast), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightRaycast), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + extraHeightRaycast), Vector2.right * (playerCollider.bounds.extents.x), rayColor);
        return raycastHit.collider != null;
    }

    public void JumpFinished()
    {
        animator.SetBool("Jumping", false);
    }

    public void Jump()
    {
        if (RB.velocity.y < 0) //Hvis spilleren falder vil hoppet stoppe dem
        {
            RB.velocity = new Vector2(RB.velocity.x, 0);
        }

        RB.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        lastJumpedTime = 0;
    }

    public void LedgeJump()
    {
        RB.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    public void dealDamage(float damage)
    {
        if(invincible == false && !isDead)
        {
            health -= damage;
            invincible = true;
            iFrameTimer = Time.time;
        }
    }
    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("Gameover");
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
}
