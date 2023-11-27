using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    [Header("Prefab to Instantiate")]
    [SerializeField] private GameObject replayObjectPrefab;

    public Queue<ReplayData> recordingQueue { get; private set; }

    private Recording recording;

    private bool isDoingReplay = false;

    private bool hasReachedGoal = false;

    private void Awake()
    {
        hasReachedGoal = false;
        recordingQueue = new Queue<ReplayData>();
    }

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void OnGoalReached()
    {
        if (this.gameObject.transform.CompareTag("Player 1") || this.gameObject.transform.CompareTag("Player 2"))
        {
            // Disable player visual
            this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            SpriteRenderer[] spriteRenderers = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.enabled = false;
            }
        }
        StartReplay();
    }

    public void OnRestartLevel()
    {
        Reset();
    }

    private void Update()
    {
        if (LevelFinishDoor.levelComplete && !hasReachedGoal)
        {
            //Debug.Log("Finished");
            OnGoalReached();
            hasReachedGoal = true;
        }

        if (!isDoingReplay)
        {
            return;
        }

        bool hasMoreFrames = recording.PlayNextFrame();

        // Check if finished
        if (!hasMoreFrames)
        {
            RestartReplay();
        }
    }

    public void RecordReplayFrame(ReplayData data)
    {
        recordingQueue.Enqueue(data);
    }

    private void StartReplay()
    {
        isDoingReplay = true;
        // Initialise recording
        recording = new Recording(recordingQueue);
        //Reset the current recording queue for next time
        recordingQueue.Clear();
        //Instantiate the replay object in scene
        recording.InstantiateReplayObject(replayObjectPrefab);
    }

    private void RestartReplay()
    {
        isDoingReplay = true;
        // Restart queued data from beginning
        recording.RestartFromBeginning();
    }

    private void Reset()
    {
        isDoingReplay = false;
        // Reset recorder
        recordingQueue.Clear();
        recording.DestroyReplayObjectIfExists();
        recording = null;
    }
}
