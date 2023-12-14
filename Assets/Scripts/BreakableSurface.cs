using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BreakableSurface : MonoBehaviour
{
    public enum SurfaceType
    {
        Floor,
        Wall
    }
    [SerializeField] private SurfaceType surfaceType;

    private SpriteRenderer sr;
    private BoxCollider2D breakTriggerCollider;
    private Recorder recorder;

    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        breakTriggerCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        recorder = GetComponent<Recorder>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (surfaceType == SurfaceType.Floor)
        {
            breakTriggerCollider.size = new Vector2(sr.size.x - 0.1f, breakTriggerCollider.size.y);
        }
        else if (surfaceType == SurfaceType.Wall)
        {
            breakTriggerCollider.size = new Vector2(breakTriggerCollider.size.x, sr.size.y - 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ReplayData data = new BreakableWallReplayData(this.transform.position, sr.color.a, sr.size);
        recorder.RecordReplayFrame(data);
    }

    public void Break(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter detected.");
        if (collision.CompareTag("Player 1") && surfaceType == SurfaceType.Floor)
        {
            Debug.Log("Player 1 and surface type are correct.");
            if (collision.GetComponent<PlayerController>().doubleJumpUsed)
            {
                // Cancel momentum
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                Vector2 direction = collision.transform.position.y < transform.position.y ? Vector2.down : Vector2.up;
                rb.AddForce(direction * Mathf.Abs(rb.velocity.y), ForceMode2D.Impulse);
                
                //Destroy(this.gameObject);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                foreach (Collider2D col in GetComponents<Collider2D>())
                {
                    col.enabled = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                }

                virtualCamera.gameObject.GetComponent<ScreenShake>().ShakeCamera(1.2f, 0.5f);
            }
        }
        else if (collision.CompareTag("Player 2") && surfaceType == SurfaceType.Wall)
        {
            if (collision.GetComponent<PlayerController>().doubleJumpUsed)
            {
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                Vector2 direction = collision.transform.position.x < transform.position.x ? Vector2.left : Vector2.right;
                rb.AddForce(direction * Mathf.Abs(rb.velocity.x), ForceMode2D.Impulse);

                //Destroy(this.gameObject);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                foreach (Collider2D col in GetComponents<Collider2D>())
                {
                    col.enabled = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                }

                virtualCamera.gameObject.GetComponent<ScreenShake>().ShakeCamera(1.2f, 0.5f);
            }
        }
    }
}
