using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class LevelFinishDoor : MonoBehaviour
{
    public string nextSceneName;
    public bool finalLevel = false;
    public GameObject playtestCompleteUI;
    
    private List<Transform> players = new List<Transform>();
    public static bool levelComplete = false;

    // Camera
    [SerializeField] CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private Transform replayCameraTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float replayOrthoSize;

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
        if (!finalLevel)
        {
            playtestCompleteUI = null;
        }
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
        if (!finalLevel)
        {
            levelComplete = true;
            //SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            levelComplete = true;
            //playtestCompleteUI.SetActive(true);
            //Time.timeScale = 0;
        }
    }
}
