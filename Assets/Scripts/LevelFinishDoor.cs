using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class LevelFinishDoor : MonoBehaviour
{
    public UIManager uiManager;
    
    private List<Transform> players = new List<Transform>();
    public static bool levelComplete;

    // Camera
    [SerializeField] CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private Transform replayCameraTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float replayOrthoSize;

    [SerializeField] private GameObject swapIcon;

    private void Awake()
    {
        levelComplete = false;

        cinemachineCamera.m_Lens.OrthographicSize = 5f;
        cinemachineCamera.Follow = playerCameraTransform;
        cinemachineCamera.LookAt = playerCameraTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 2 && !levelComplete)
        {
            LevelComplete();
        }

        if (levelComplete)
        {
            cinemachineCamera.m_Lens.OrthographicSize = replayOrthoSize;
            cinemachineCamera.Follow = replayCameraTransform;
            cinemachineCamera.LookAt = replayCameraTransform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player 1") || collision.CompareTag("Player 2"))
        {
            players.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player 1") || collision.CompareTag("Player 2"))
        {
            players.Remove(collision.transform);
        }
    }

    private void LevelComplete()
    {
        levelComplete = true;
        LevelTimer.isTimerRunning = false;
        PlayerController.isPlayerDisabled = true;
        if (swapIcon != null)
        {
        swapIcon.SetActive(false);
        }
        uiManager.EnableReplayUI();
    }
}
