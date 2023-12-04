using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{
    public GravityDirection gravityDirection;

    [SerializeField] private float gravityStrength;
    private Vector2 gravityDirectionVector;

    private Rigidbody2D p1RB;
    private Rigidbody2D p2RB;
    private ConstantForce2D p1CF = null;
    private ConstantForce2D p2CF = null;

    private void Start()
    {
        switch (gravityDirection)
        {
            case GravityDirection.Up:
                gravityDirectionVector = new Vector2(0, 1);
                break;
            case GravityDirection.Down:
                gravityDirectionVector = new Vector2(0, -1);
                break;
            case GravityDirection.Left:
                gravityDirectionVector = new Vector2(-1, 0);
                break;
            case GravityDirection.Right:
                gravityDirectionVector = new Vector2(1, 0);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (p1CF != null)
        {
            p1CF.force = gravityDirectionVector * gravityStrength;
            Accelerate(p1RB, p1CF);
        }
        else if (p2CF != null)
        {
            p2CF.force = gravityDirectionVector * gravityStrength;
            Accelerate(p2RB, p2CF);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player 1"))
        {
            Debug.Log("Player1 entered zone");
            p1RB = collision.gameObject.GetComponent<Rigidbody2D>();
            p1RB.gravityScale = 0f;
            p1CF = collision.gameObject.GetComponent<ConstantForce2D>();
            RotatePlayer(collision.gameObject);
        }
        else if (collision.CompareTag("Player 2"))
        {
            Debug.Log("Player2 entered zone");
            p2RB = collision.gameObject.GetComponent<Rigidbody2D>();
            p2RB.gravityScale = 0f;
            p2CF = collision.gameObject.GetComponent<ConstantForce2D>();
            RotatePlayer(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player 1"))
        {
            p1RB.gravityScale = 1f;
            p1CF.force = new Vector2(0, 0);
            p1CF = null;
            collision.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (collision.CompareTag("Player 2"))
        {
            p2RB.gravityScale = 1f;
            p2CF.force = new Vector2(0, 0);
            p2CF = null;
            collision.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void Accelerate(Rigidbody2D rb, ConstantForce2D constantForce2D)
    {
        switch (gravityDirection)
        {
            case GravityDirection.Up:
                if (rb.velocity.y > 0)
                {
                    constantForce2D.force = gravityDirectionVector * (gravityStrength * 1.9f);
                }
                break;
            case GravityDirection.Down:
                if (rb.velocity.y < 0)
                {
                    constantForce2D.force = gravityDirectionVector * (gravityStrength * 1.9f);
                }
                break;
            case GravityDirection.Left:
                if (rb.velocity.x < 0)
                {
                    constantForce2D.force = gravityDirectionVector * (gravityStrength * 1.9f);
                }
                break;
            case GravityDirection.Right:
                if (rb.velocity.x > 0)
                {
                    constantForce2D.force = gravityDirectionVector * (gravityStrength * 1.9f);
                }
                break;
        }
    }

    private void RotatePlayer(GameObject player)
    {
        switch (gravityDirection)
        {
            case GravityDirection.Up:
                player.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case GravityDirection.Down:
                player.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case GravityDirection.Left:
                player.transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case GravityDirection.Right:
                player.transform.eulerAngles = new Vector3(0, 0, 90);
                break;
        }
    }
}

public enum GravityDirection
{
    Up,
    Down,
    Left,
    Right
}
