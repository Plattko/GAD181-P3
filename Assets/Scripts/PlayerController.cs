using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform groundCheck;
    private int groundLayer = 6;
    private int p1Layer = 7;
    private int p2Layer = 8;
    private LayerMask layerMask;
    public Transform otherPlayer;
    public Transform particleManager;

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
    private float coyoteTimeCounter;
    private float gravityScale = 1f;
    private float gravityFallMultiplier = 1.9f;

    public bool doubleJumpUsed { get; private set; } = false;

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

    // Replay system
    private Recorder recorder;

    private void Awake()
    {
        // Setting frame rate cap
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 240;
        
        recorder = GetComponent<Recorder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.GetChild(1).gameObject.GetComponent<Transform>();

        layerMask = 1 << groundLayer;
        
        if (playerType == PlayerType.Player1)
        {
            layerMask |= 1 << p2Layer;
        }
        else if (playerType == PlayerType.Player2)
        {
            layerMask |= 1 << p1Layer;
        }

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
                if (!IsGrounded())
                {
                    if (doubleJumpUsed)
                    {
                        moveSpeed = p1ReducedDblJumpSpeed;
                    }
                }
                else if (IsGrounded())
                {
                    moveSpeed = regSpeed;
                }
                break;
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
        // Calculate the move direction and the desired velocity
        float targetSpeed = horizontalInput * moveSpeed;
        // Calculate the difference between the current and desired velocity
        float speedDiff = targetSpeed - rb.velocity.x;
        // Change between acceleration or deceleration depending on whether the player is providing an input
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Apply acceleration to the speed difference, then raise to a set power so acceleration increases with higher speeds
        // Multiply by sign to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        // Apply movement force to the Rigidbody on the x axis
        rb.AddForce(movement * Vector2.right);

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

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * gravityFallMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(new Vector2(groundCheck.position.x, groundCheck.position.y), new Vector2(0.8f, 0.2f), 0f, layerMask);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (coyoteTimeCounter > 0f)
            {
                // Jump particles
                particleManager.GetComponent<ParticleManager>().PlayJumpParticles(jumpParticles, groundCheck);

                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
            else if (!doubleJumpUsed)
            {
                doubleJumpUsed = true;

                // Cancel vertical velocity
                rb.velocity = new Vector2(rb.velocity.x, 0f);

                // Double jump
                switch (playerType)
                {
                    case PlayerType.Player1:

                        rb.AddForce(doubleJumpVector, ForceMode2D.Impulse);
                        break;

                    case PlayerType.Player2:

                        if (Mathf.Abs(horizontalInput) > 0.01f)
                        {
                            Vector2 directionalVector = new Vector2(doubleJumpVector.x * horizontalInput, doubleJumpVector.y);
                            rb.AddForce(directionalVector, ForceMode2D.Impulse);
                        }
                        else if (Mathf.Abs(horizontalInput) < 0.01f)
                        {
                            Vector2 verticalVector = Vector2.up * doubleJumpVector.y;
                            rb.AddForce(verticalVector, ForceMode2D.Impulse);
                        }
                        break;
                }

                // Double jump particles
                particleManager.GetComponent<ParticleManager>().PlayDoubleJumpParticles(doubleJumpParticles, transform);
            }
        }
        
        if (context.canceled && rb.velocity.y > 0f)
        {
            coyoteTimeCounter = 0f;
            
            rb.AddForce(Vector2.down * rb.velocity.y * 0.5f, ForceMode2D.Impulse);
        }
    }

    public void OnSwapPosition(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time > PlayerManager.nextSwapAllowed && canSwap)
        {
            PlayerManager.nextSwapAllowed = Time.time + swapCooldown;
            
            Vector2 thisPosition = transform.position;
            Vector2 otherPosition = otherPlayer.position;

            transform.position = otherPosition;
            otherPlayer.position = thisPosition;

            particleManager.GetComponent<ParticleManager>().PlaySwapParticles();
        }
    }
}
