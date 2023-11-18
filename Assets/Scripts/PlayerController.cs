using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform groundCheck;
    private int layerIndex = 6;
    private LayerMask groundLayer;
    public Transform otherPlayer;

    private float horizontalInput;
    private float moveSpeed = 5f;
    private float jumpPower = 8f;
    private bool doubleJumpUsed = false;

    private enum PlayerType
    {
        Player1,
        Player2
    }
    [SerializeField] private PlayerType playerType;

    // Player-specific variables
    private float p1HighJumpPower = 5f;
    private float p2LungePower = 30f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.GetChild(1).gameObject.GetComponent<Transform>();
        groundLayer = 1 << layerIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        {
            doubleJumpUsed = false;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                Debug.Log("Executed first jump.");
            }
            else if (!doubleJumpUsed)
            {
                Debug.Log("Called second jump.");
                doubleJumpUsed = true;

                switch (playerType)
                {
                    case PlayerType.Player1:
                        rb.velocity = new Vector2(rb.velocity.x, p1HighJumpPower);
                        rb.AddForce(Vector2.up * p1HighJumpPower, ForceMode2D.Impulse);
                        break;

                    case PlayerType.Player2:
                        Vector2 direction = new Vector2(horizontalInput, 0);
                        //Quaternion rotation = Quaternion.identity;
                        //if (horizontalInput < 0)
                        //{
                        //    rotation = Quaternion.Euler(0, 0, -180);
                        //}
                        //else if (horizontalInput > 0)
                        //{
                        //    rotation = Quaternion.Euler(0, 0, 180);
                        //}
                        //Vector2 forceDirection = rotation * direction;
                        Debug.Log("Direction is: " + direction);
                        rb.AddForce(direction * p2LungePower, ForceMode2D.Impulse);
                        break;
                }
                Debug.Log("Executed second jump.");
            }
        }
        
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void OnSwapPosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 thisPosition = transform.position;
            Vector2 otherPosition = otherPlayer.position;

            transform.position = otherPosition;
            otherPlayer.position = thisPosition;
        }
    }
}
