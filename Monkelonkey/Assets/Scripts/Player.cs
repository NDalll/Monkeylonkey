using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed;
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
    public float grappleMultipier;
    private bool isGrappling = false;

    public float health;
    public Slider healthBar;
    private bool isDead;


    private Vector3 gPosition;
    private GameObject grappleP;
    private GameObject[] grapplePoints;
    private LineRenderer lr;

    [SerializeField] private LayerMask platformLayerMask;
    private void Start()
    {
        healthBar.maxValue = health;
        grapplePoints = GameObject.FindGameObjectsWithTag("grapple");
        lr = gameObject.GetComponent<LineRenderer>();
    }
    private void FixedUpdate()
    {
        Vector2 input = playerControls.Default.Move.ReadValue<Vector2>();
        
        if (input.x < 0)
        {
            player.flipX = true;
        }
        if (input.x > 0)
        {
            player.flipX = false;
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
        //timer (for coyote time):
        lastGroundedTime += Time.deltaTime;
        lastJumpedTime += Time.deltaTime;

        //jump
        if ((IsGrounded()||(lastGroundedTime < coyoteTime && lastJumpedTime > coyoteTime * 2)) && playerControls.Default.Jump.triggered)
        {
            if (RB.velocity.y < 0) //Hvis spilleren falder vil hoppet stoppe dem
            {
                RB.velocity = new Vector2(RB.velocity.x, 0);
            }
            
            RB.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            lastJumpedTime = 0;
        }

        //healthbar
        healthBar.value = health;
        if (health <= 0)
        {
            isDead = true;
            playerControls.Disable();
        }

        if (playerControls.Default.Grapple.WasReleasedThisFrame())
        {
            isGrappling = false;
        }


    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(grapplePoints);
        if (collision.CompareTag("grapple"))
        {

            foreach (GameObject x in grapplePoints)
            {
                x.transform.localScale = new Vector3(1, 1, 1);
            }
            collision.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            if (playerControls.Default.Grapple.IsPressed())
            {
                if(isGrappling == false)
                {
                    RB.velocity = new Vector2(0, 0);
                    isGrappling = true;
                    gPosition = collision.bounds.center;
                    grappleP = collision.gameObject;

                }


                Vector2 direction = new Vector2(gPosition.x - gameObject.transform.position.x, gPosition.y - gameObject.transform.position.y ).normalized;
                direction = new Vector2(direction.x * grappleMultipier, direction.y * grappleMultipier);
                lr.enabled = true;
                lr.SetPosition(0, gameObject.transform.position);
                lr.SetPosition(1, grappleP.transform.position);
                
                RB.AddForce(direction*Time.deltaTime*1000);
            }
            else {
                lr.enabled = false;
            }
        }
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
