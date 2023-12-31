using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lava : MonoBehaviour
{
    private float riseSpeed = 0.25f;

    private Recorder recorder;

    private void Awake()
    {
        recorder = GetComponent<Recorder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, riseSpeed, 0) * Time.deltaTime);
    }

    private void LateUpdate()
    {
        ReplayData data = new MovingObjectReplayData(this.transform.position);
        recorder.RecordReplayFrame(data);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player 1") || collision.CompareTag("Player 2")) && !PlayerController.isPlayerDisabled)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
