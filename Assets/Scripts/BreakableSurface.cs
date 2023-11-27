using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableSurface : MonoBehaviour
{
    public enum SurfaceType
    {
        Floor,
        Wall
    }
    [SerializeField] private SurfaceType surfaceType;

    private SpriteRenderer sr;

    private Recorder recorder;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        recorder = GetComponent<Recorder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ReplayData data = new BreakableWallReplayData(this.transform.position, sr.color.a);
        recorder.RecordReplayFrame(data);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter detected.");
        if (collision.CompareTag("Player 1") && surfaceType == SurfaceType.Floor)
        {
            Debug.Log("Player 1 and surface type are correct.");
            if (collision.GetComponent<PlayerController>().doubleJumpUsed)
            {
                //Destroy(this.gameObject);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                foreach (Collider2D col in GetComponents<Collider2D>())
                {
                    col.enabled = false;
                }
            }
        }
        else if (collision.CompareTag("Player 2") && surfaceType == SurfaceType.Wall)
        {
            if (collision.GetComponent<PlayerController>().doubleJumpUsed)
            {
                //Destroy(this.gameObject);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                foreach (Collider2D col in GetComponents<Collider2D>())
                {
                    col.enabled = false;
                }
            }
        }
    }
}
