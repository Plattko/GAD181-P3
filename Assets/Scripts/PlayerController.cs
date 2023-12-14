using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform groundCheck;
    public Transform otherPlayer;
    public Transform particleManager;

    // Ground check variables
    private int groundLayer = 6;
    private int p1Layer = 7;
    private int p2Layer = 8;
    private LayerMask groundLayerMask;
    private Collider2D lastGroundCollider;

    // Wall check variables
    private int wallLayer = 11;
    private LayerMask wallLayerMask;
    private Collider2D wallCollider;

    // Move variables
    private float horizontalInput;
    private float moveSpeed = 8f;
    private float acceleration = 7f;
    private float deceleration = 7f;
    private float velPower = 0.9f;

    private float frictionAmount = 0.25f;

    // Jump variables
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private List<ParticleSystem> doubleJumpParticles = new List<ParticleSystem>();
    
    [SerializeField] private float jumpPower = 8f;
    private float coyoteTime = 0.15f;
    public float coyoteTimeCounter;
    private float gravityScale = 1f;
    private float gravityFallMultiplier = 1.9f;

    public bool doubleJumpUsed { get; private set; } = false;

    // Wall-slide and wall-jump variables
    [SerializeField] private ParticleSystem wallJumpLeftParticles;
    [SerializeField] private ParticleSystem wallJumpRightParticles;

    private float wallSlideSpeed = Mathf.PI;
    private bool isWallSliding = false;
    
    private float wallJumpDirection;
    private float wallJumpCoyoteTime = 0.15f;
    private float wallJumpCoyoteTimeCounter;
    [SerializeField] private Vector2 wallJumpVector;

    private bool isWallJumping = false;
    private float wallJumpTime = 0.75f;
    private float wallJumpEndTime;
    private float wallJumpMoveLerp = 0.2f;
    
    // Swap variables
    private float swapCooldown = 1f;
    public bool canSwap = true;

    // Player-specific variables
    private enum PlayerType
    {
        Player1,
        Player2
    }
    [SerializeField] private PlayerType playerType;
    [SerializeField] private Vector2 doubleJumpVector;

    private float p1ReducedDblJumpSpeed = 4f;
    private float regSpeed;

    // Replay system variables
    private Recorder recorder;

    public AnimationCurve stretchCurve;
    public AnimationCurve squashCurve;
    private bool isGroundedSquash = true; // Used for calling squash coroutine
    private float stretchTime = 0;

    public static bool isPlayerDisabled = false;

    private void Awake()
    {
        // Setting frame rate cap
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 240;

        isPlayerDisabled = false;

        recorder = GetComponent<Recorder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.GetChild(1).gameObject.GetComponent<Transform>();

        groundLayerMask = 1 << groundLayer;

        if (playerType == PlayerType.Player1)
        {
            groundLayerMask |= 1 << p2Layer;
        }
        else if (playerType == PlayerType.Player2)
        {
            groundLayerMask |= 1 << p1Layer;
        }

        wallLayerMask = 1 << wallLayer;

        regSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        {
            doubleJumpUsed = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        switch (playerType)
        {
            case PlayerType.Player1:
                
                if (IsGrounded() || isWallSliding)
                {
                    moveSpeed = regSpeed;
                }
                break;
        }

        if (isWallSliding)
        {
            wallJumpCoyoteTimeCounter = wallJumpCoyoteTime;

            // Set the wall-jump direction
            if (wallCollider.transform.position.x < transform.position.x)
            {
                wallJumpDirection = 1f;
            }
            else if (wallCollider.transform.position.x > transform.position.x)
            {
                wallJumpDirection = -1f;
            }
        }
        else
        {
            wallJumpCoyoteTimeCounter -= Time.deltaTime;
        }

        // Conditions that cancel wall-jumping
        if ((isWallJumping && Time.time > wallJumpEndTime) || IsGrounded() || doubleJumpUsed || (isWallSliding && rb.velocity.x == 0))
        {
            isWallJumping = false;
        }
    }

    private void LateUpdate()
    {
        // Record replay data for this frame
        ReplayData data = new PlayerReplayData(this.transform.position);
        recorder.RecordReplayFrame(data);
    }

    private void FixedUpdate()
    {
        if (isWallJumping)
        {
            Move(wallJumpMoveLerp);
        }
        else
        {
            Move(1);
        }

        // Friction
        if (IsGrounded() && Mathf.Abs(horizontalInput) < 0.01f)
        {
            // Choose the smallest value between the player's current velocity and the friction amount
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            // Set movement direction
            amount *= Mathf.Sign(rb.velocity.x);
            // Apply friction force opposite to the player's movement direction
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        // Fast fall
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * gravityFallMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        WallSlide();
    }

    // ---------------------------------
    // COLLISION CHECKS
    // ---------------------------------
    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapBox(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(0.8f, 0.2f), 0f, groundLayerMask);
        if (Mathf.Abs(rb.velocity.y) <= 0.01f && !PlayerManager.isSwapping && collider != null)
        {
            lastGroundCollider = collider;

            if (!isGroundedSquash)
            {
                StartCoroutine(Squash());
            }
            isGroundedSquash = true;
            return true;
        }
        return false;
    }

    private bool IsWalled()
    {
        wallCollider = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(1.0f, 0.8f), 0f, wallLayerMask);
        if (wallCollider != null)
        {
            return wallCollider;
        }
        return false;
    }

    // ---------------------------------
    // MOVEMENT METHODS
    // ---------------------------------
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded())
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Jump()
    {
        // Jump particles
        if (lastGroundCollider.gameObject.layer == groundLayer)
        {
            particleManager.GetComponent<ParticleManager>().PlayJumpParticles(jumpParticles, groundCheck.position);
        }

        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isGroundedSquash = false;

        // Stretch
        StartCoroutine(Stretch(false, 0));
    }

    private void WallJump()
    {
        isWallJumping = true;
        doubleJumpUsed = false;
        wallJumpEndTime = Time.time + wallJumpTime;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(wallJumpVector.x * wallJumpDirection, wallJumpVector.y), ForceMode2D.Impulse);

        if (wallJumpDirection > 0)
        {
            particleManager.GetComponent<ParticleManager>().PlayJumpParticles(wallJumpLeftParticles, new Vector2(groundCheck.position.x - 0.45f, groundCheck.position.y));
        }
        else if (wallJumpDirection < 0)
        {
            particleManager.GetComponent<ParticleManager>().PlayJumpParticles(wallJumpRightParticles, new Vector2(groundCheck.position.x + 0.45f, groundCheck.position.y));
        }
    }

    private void DoubleJump()
    {
        // Prevent multiple uses of double jump
        doubleJumpUsed = true;

        // Cancel vertical velocity
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        // Double jump
        switch (playerType)
        {
            case PlayerType.Player1:

                moveSpeed = p1ReducedDblJumpSpeed;
                rb.AddForce(doubleJumpVector, ForceMode2D.Impulse);
                StartCoroutine(Stretch(false, stretchTime));
                break;

            case PlayerType.Player2:

                if (Mathf.Abs(horizontalInput) > 0.01f)
                {
                    Vector2 directionalVector = new Vector2(doubleJumpVector.x * horizontalInput, doubleJumpVector.y);
                    if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(directionalVector.x))
                    {
                        directionalVector.x -= rb.velocity.x;
                    }

                    rb.AddForce(directionalVector, ForceMode2D.Impulse);
                    StartCoroutine(Stretch(true, 0));
                }
                else if (Mathf.Abs(horizontalInput) < 0.01f)
                {
                    Vector2 verticalVector = Vector2.up * doubleJumpVector.y;
                    rb.AddForce(verticalVector, ForceMode2D.Impulse);
                    StartCoroutine(Stretch(false, stretchTime));
                }
                break;
        }

        // Double jump particles
        particleManager.GetComponent<ParticleManager>().PlayDoubleJumpParticles(doubleJumpParticles, transform);
    }

    private void Swap()
    {
        PlayerManager.isSwapping = true;

        PlayerManager.nextSwapAllowed = Time.time + swapCooldown;

        Vector2 thisPosition = transform.position;
        Vector2 otherPosition = otherPlayer.position;

        transform.position = otherPosition;
        otherPlayer.position = thisPosition;

        coyoteTimeCounter = 0f;
        otherPlayer.GetComponent<PlayerController>().coyoteTimeCounter = 0f;

        particleManager.GetComponent<ParticleManager>().PlaySwapParticles();
        particleManager.GetComponent<ParticleManager>().PlaySwapTrailParticles();

        StartCoroutine(NoLongerSwapping());
    }

    private void Move(float lerpAmount)
    {
        // Calculate the move direction and the desired velocity
        float targetSpeed = horizontalInput * moveSpeed;
        // Reduce player horizontal control during wall jump
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        // Calculate the difference between the current and desired velocity
        float speedDiff = targetSpeed - rb.velocity.x;
        // Change between acceleration or deceleration depending on whether the player is providing an input
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Apply acceleration to the speed difference, then raise to a set power so acceleration increases with higher speeds
        // Multiply by sign to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        // Apply movement force to the Rigidbody on the x axis
        rb.AddForce(movement * Vector2.right);
    }

    // ---------------------------------
    // INPUT CHECKS
    // ---------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (coyoteTimeCounter > 0f) // Regular Jump
            {
                Jump();
            }
            else if (wallJumpCoyoteTimeCounter > 0f) // Wall jump
            {
                WallJump();
            }
            else if (!doubleJumpUsed) // Double jump
            {
                DoubleJump();
            }
        }
        
        if (context.canceled && rb.velocity.y > 0f)
        {
            coyoteTimeCounter = 0f;
            wallJumpCoyoteTimeCounter = 0f;

            rb.AddForce(Vector2.down * rb.velocity.y * 0.5f, ForceMode2D.Impulse);
        }
    }

    public void OnSwapPosition(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time > PlayerManager.nextSwapAllowed && canSwap)
        {
            Swap();
        }
    }

    public IEnumerator NoLongerSwapping()
    {
        yield return new WaitForFixedUpdate();
        PlayerManager.isSwapping = false;
        Debug.Log("Swapping is false");
    }

    private IEnumerator Stretch(bool sideways, float startingTime)
    {
        float duration = 0.5f;
        stretchTime = startingTime;

        while (stretchTime < duration)
        {
            float stretchScale = stretchCurve.Evaluate(stretchTime / duration);

            if (!sideways)
            {
                transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x, 1.25f + stretchScale, 1f);
            }
            else
            {
                transform.GetChild(0).localScale = new Vector3(0.9f + stretchScale, transform.GetChild(0).localScale.y, 1f);
            }

            stretchTime += Time.deltaTime;
            yield return null;
        }

        transform.GetChild(0).localScale = new Vector3(0.9f, 1.25f, 1f);
        stretchTime = 0f;
    }

    private IEnumerator Squash()
    {
        float duration = 0.5f;
        float time = 0;

        while (time < duration)
        {
            float squashScale = squashCurve.Evaluate(time / duration);
            transform.GetChild(0).localScale = new Vector3(0.9f, 1.25f - squashScale, 1f);

            time += Time.deltaTime;
            yield return null;
        }
        transform.GetChild(0).localScale = new Vector3(0.9f, 1.25f, 1f);
    }
}
