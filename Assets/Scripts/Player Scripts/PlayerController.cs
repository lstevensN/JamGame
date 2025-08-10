using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A responsive 2D character controller that handles movement, jumping, and ground detection.
/// Features smooth acceleration, air control, coyote time, double jumping, and variable jump height.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
 
    [Header("Movement")]
    [SerializeField] float speed = 1f;              // Maximum movement speed
    [SerializeField] float acceleration = 10f;      // How quickly the character reaches top speed
    [SerializeField] float deceleration = 10f;      // How quickly the character stops
    [SerializeField] float airControl = 0.5f;       // Multiplier for acceleration while in air (0-1)

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;         // Maximum height of jump in units when button is held
    [SerializeField] float lowJumpMultiplier = 2f;  // Multiplier for gravity when jump button is released early (controls minimum jump height)
    [SerializeField] float fallMultiplier = 2.5f;   // Increases gravity when falling for better feel
    [SerializeField] float coyoteTime = 0.2f;       // Time in seconds character can jump after leaving ground
    [SerializeField] float doubleJumpTime = 0.5f;   // Time window in seconds to perform a double jump
    [SerializeField] float glitchDistance = 2f;     // Distance to either side that the character will glitch

    //[SerializeField] float climbingSpeed = 1.0f;

    [Header("Ground Detection")]
    [SerializeField] float castDistance = 0.1f;     // How far to check for ground below the character
    [SerializeField] ContactFilter2D groundFilter;  // Layer and collision settings for ground detection

    [Header("Components")]
    [SerializeField] Animator animator;             // Reference to character's animator
    [SerializeField] SpriteRenderer spriteRenderer; // Reference to character's sprite renderer
    [SerializeField] GameObject sprite;
    //[SerializeField] Health health;                 // Reference to character's health
    //[SerializeField] FloatDataSO healthData;

    //[SerializeField] BoolDataSO isClimbing;
    [SerializeField] BoolDataSO isClimbing;
    [SerializeField] FloatDataSO climbingSpeed;
    //[SerializeField] EventChannelSO winGameEvent;

    [SerializeField] AudioSource attackSound;
    [SerializeField] AudioSource damagedSound;

    [SerializeField] BoolDataSO playerDead;
    [SerializeField] BoolDataSO InDialogue;





    bool hasWon = false;

    Rigidbody2D rb;                                // Reference to attached Rigidbody2D component

    public const int FACE_LEFT = -1;
    public const int FACE_RIGHT = 1;

    int facing = 1;                                // Facing direction: 1 = right, -1 = left
    float currentSpeed = 0f;                       // Current horizontal speed after smoothing
    public int Facing => facing;
    public Rigidbody2D RB => rb; //Why isn't this working???

    // Ground collision tracking
    RaycastHit2D[] raycastHits = new RaycastHit2D[5]; // Buffer for ground collision results
    int groundHits = 0;                              // Number of ground collisions detected
    bool isGrounded = false;                         // Whether character is currently on ground
                                                     //public bool isClimbing = false;
    float coyoteTimer = 0f;                          // Timer for coyote time jump window
    float doubleJumpTimer = 0f;                      // Timer for double jump window

    // Jump state tracking
    bool jumpButtonReleased = true;                  // Whether jump button has been released since last press

    // Movement input
    Vector2 direction;                              // Current input direction (typically from -1 to 1)
    float speedMultiplier = 1f;                         //For things like sprinting

    //Health related
    float currentHealth;                            //Current Character Health
    float invincibilityFrames = 1;
    bool isHit = false;
    bool isDead = false;
    private float deathTimer = 0.0f;

    bool glitch = false;
    float glitchCD = 2.0f;
    float glitchTimer = 2.1f;

    [SerializeField] float attackCD = 10;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    public bool hasKey = false;



    /// <summary>
    /// Sets the movement direction based on input.
    /// Called by input system when movement input changes.
    /// </summary>
    /// <param name="v">Vector2 containing horizontal and vertical input values</param>
    //public void OnMove(Vector2 v) => direction = v;
    public void OnMove(Vector2 v)
    {
        if (isDead || hasWon) return;
        direction = v;
    }

    /// <summary>
    /// Initialize references. Called before Start.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //currentHealth = health.getHealth();
        //if (healthData != null) healthData.Value = health.getHealth();
        //winGameEvent?.AddListener(winGame);
    }

    /// <summary>
    /// Handle non-physics updates. Called once per frame.
    /// </summary>
    public void Update()
    {
        if(InDialogue.Value)
        {
            rb.linearVelocityY = 0f;
        }

        if (isDead)
        {
            rb.linearVelocityY = 0f;
            deathTimer += Time.deltaTime;
            if (deathTimer >= 1.0)
            {
                GameObject.Destroy(sprite);
            }
            return;
        }
        UpdateGroundCollision();
        UpdateFacing();
        UpdateAnimator();
        UpdateHealth();


        attackTimer -= Time.deltaTime;
        if(InDialogue.Value || isAttacking && attackTimer <= 0)
        {
            isAttacking = false;
        }

        glitchTimer += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.E) && !InDialogue.Value)
        {
            Glitch();
        }
    }

    /// <summary>
    /// Handle physics-based movement. Called at fixed time intervals.
    /// </summary>
    void FixedUpdate()
    {
        if (isDead || InDialogue.Value) return;
        // Calculate target speed based on input direction
        float targetSpeed = direction.x * speed;

        //Ladder climbing

        // Apply acceleration based on grounded state
        // Reduces control in the air by using the airControl multiplier
        float accelRate = isGrounded ? acceleration : acceleration * airControl;

        // Apply movement with smoothing
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            // Accelerating towards target speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerating to stop
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        // Apply horizontal velocity
        rb.linearVelocityX = currentSpeed * speedMultiplier;

        // Apply variable jump height physics
        if (rb.linearVelocityY < 0)
        {
            // We're falling - apply increased gravity for snappier falls
            rb.linearVelocityY += Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime * rb.gravityScale;
            //rb.linearVelocityY += Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocityY > 0 && jumpButtonReleased)
        {
            // We're rising but the jump button was released early
            // Apply extra gravity to cut the jump short (creates variable jump height)
            rb.linearVelocityY += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime * rb.gravityScale;
        }

        if (isClimbing != null && climbingSpeed != null && isClimbing.Value)
        {
            //print("Something should be happening");
            //rb.linearVelocityY = new Vector2(rb.linearVelocityX, 1.0f);
            rb.linearVelocityY = climbingSpeed.Value;
            //rb.AddForceY(rb.linearVelocityY); 
            //rb.linearVelocityY
        }


        if (isHit)
        {
            Vector2 knockback = new Vector2(facing * -100, 10);
            rb.AddForce(knockback, ForceMode2D.Impulse);
            isHit = false;
        }



        if(glitch)
        {
            glitch = false;
            Vector2 currentLocation = this.GetComponent<Transform>().position;
            Vector2 newLocation = new Vector2((currentLocation.x + (glitchDistance * facing)), currentLocation.y);
            this.GetComponent<Transform>().position = newLocation;
        }

    }

    /// <summary>
    /// Update character's facing direction based on movement input.
    /// </summary>
    void UpdateFacing()
    {
        // Only flip if we're moving and facing the opposite direction
        if (direction.x != 0 && Mathf.Sign(direction.x) != facing)
        {
            FlipDirection();
        }
    }

    /// <summary>
    /// Check for ground collision and update grounded state.
    /// Also handles landing logic and timer updates.
    /// </summary>
    void UpdateGroundCollision()
    {
        // Cast a ray downward to detect ground
        groundHits = rb.Cast(Vector2.down, groundFilter, raycastHits, castDistance);

        // Update grounded state
        bool wasGrounded = isGrounded;
        isGrounded = (groundHits > 0);

        // Handle landing (transitioning from in-air to grounded)
        if (isGrounded && !wasGrounded && rb.linearVelocityY < 0)
        {
            // Stop jump phase and vertical momentum when landing
            rb.linearVelocityY = 0;

            // Reset jump animation trigger to prevent retriggering
            animator?.ResetTrigger("Jump");
        }

        // Reset coyote timer when grounded
        if (isGrounded) coyoteTimer = coyoteTime;

        // Update timers
        coyoteTimer -= Time.deltaTime;
        doubleJumpTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Update animator parameters based on character state.
    /// </summary>
    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("InAir", !isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        animator.SetFloat("VelocityY", rb.linearVelocityY);
    }

    /// <summary>
    /// Update current health and triggers onHit or onDeath if applicalbe
    /// </summary>
    void UpdateHealth()
    {
        //if (currentHealth > health.getHealth())
        //{
        //    if (health.getHealth() > 0)
        //    {
        //        currentHealth = health.getHealth();
        //        if (healthData != null) healthData.Value = health.getHealth();
        //        OnHit();
        //    }
        //    else
        //    {
        //        currentHealth = 0;
        //        OnDeath();
        //    }
        //}
        //
        //invincibilityFrames -= Time.deltaTime;
        //
        //if (invincibilityFrames < 0)
        //{
        //    health.isInvincible = false;
        //}
    }

    /// <summary>
    /// Handle jump input. Called when jump button is pressed.
    /// </summary>
    public void OnJump()
    {
        if (isDead || hasWon || isAttacking || InDialogue.Value) return;
        //print(isClimbing.Value);
        if (isClimbing != null && isClimbing.Value) return;
        //if (coyoteTimer > 0)
        if ((coyoteTimer > 0 && isClimbing == null) || (coyoteTimer > 0 && !isClimbing))
        //if (coyoteTimer > 0 || coyoteTimer > 0 )
        {
            // First jump - using coyote time for better feel
            jumpButtonReleased = false;
            doubleJumpTimer = doubleJumpTime;  // Enable double jump
            ExecuteJump();
        }
        //else if (doubleJumpTimer > 0 )
        else if ((doubleJumpTimer > 0 && isClimbing == null) || (doubleJumpTimer > 0 && !isClimbing.Value))
        //else if (doubleJumpTimer > 0  || doubleJumpTimer > 0 )
        {
            // Double jump - only possible during double jump time window
            jumpButtonReleased = false;
            doubleJumpTimer = 0;  // Consume the double jump
            ExecuteJump();
        }
    }

    /// <summary>
    /// Handle jump button release. Called when jump button is released.
    /// Used for variable jump height control.
    /// </summary>
    public void OnJumpRelease()
    {
        jumpButtonReleased = true;  // This flag is used in FixedUpdate to apply the lowJumpMultiplier
    }

    /// <summary>
    /// Perform the actual jump physics calculation and application.
    /// </summary>
    private void ExecuteJump()
    {
        if (isDead) return;
        if (isClimbing != null && isClimbing.Value) return;
        // Calculate jump velocity using physics formula:
        // v = sqrt(2 * g * h) where g is gravity and h is desired height
        //float jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight * rb.gravityScale);
        float jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight) * rb.gravityScale;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

        // Trigger jump animation if animator exists
        animator?.SetTrigger("Jump");
    }

    /// <summary>
    /// Trigger attack animation. Called by input system.
    /// </summary>
    public void OnAttack()
    {
        if (isDead || hasWon || InDialogue.Value) return;

        isAttacking = true;
        attackTimer = attackCD;


        //animator?.SetTrigger("Attack");
        //if (!isDead && !hasWon && !isClimbing && isGrounded) attackSound?.Play();
        //if (!isDead && !hasWon && isGrounded) attackSound?.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(isAttacking)
            {
                collision.gameObject.GetComponent<Enemy>().Die();
            }
        }
    }



    /// <summary>
    /// Trigger death animation. Called when character dies.
    /// </summary>
    public void OnDeath()
    {
        if (InDialogue.Value) return;
        animator?.SetTrigger("Death");
        animator?.SetBool("IsDead", true);
        isDead = true;
        rb.linearVelocityX = 0;
        //GameObject.Destroy(this, 0.83);
    }

    /// <summary>
    /// Trigger hit/damage animation. Called when character takes damage.
    /// </summary>
    public void OnHit()
    {
        if (isDead || hasWon) return;
        //
        //health.isInvincible = true;
        animator?.SetTrigger("Hit");
        damagedSound?.Play();
        invincibilityFrames = 1;
        isHit = true;

    }

    public void OnSprintOn()
    {
        if (hasWon || InDialogue.Value) return;
        speedMultiplier = 2;
        animator?.SetBool("Sprinting", true);
    }

    public void OnSprintOff()
    {
        speedMultiplier = 1;
        animator?.SetBool("Sprinting", false);
    }



    public void Glitch()
    {
        if(isAttacking || InDialogue.Value) return;
        if (glitchTimer <= glitchCD) return;
        glitch = true;
        glitchTimer = 0;
    }



    /// <summary>
    /// Flip the character's facing direction by updating the sprite.
    /// </summary>
    private void FlipDirection()
    {
        if (isDead) return;
        facing *= -1;  // Toggle between 1 and -1
        if (spriteRenderer != null)
            spriteRenderer.flipX = (facing == 1);  // Flip sprite when facing right
    }

    private void winGame()
    {
        hasWon = true;
    }



    /// <summary>
    /// Visualize ground detection in the editor. Only visible in Scene view when selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw rays showing ground contact points and normals
        if (groundHits > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < groundHits; i++)
            {
                Gizmos.DrawRay(raycastHits[i].point, raycastHits[i].normal);
            }
        }
    }


    public void Die()
    {
        if(isAttacking || InDialogue.Value) return;
        isDead = true;
        playerDead.Value = true;
    }
}

