using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFinishDoor : MonoBehaviour
{
    public string nextSceneName;
    public bool finalLevel = false;
    public GameObject playtestCompleteUI;
    
    private List<Transform> players = new List<Transform>();
    private bool levelComplete = false;
    
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
        foreach (Transform player in players)
        {
            GameObject playerGO = player.gameObject;
            playerGO.GetComponent<Recorder>().OnGoalReached();
        }
        
        if (!finalLevel)
        {
            levelComplete = true;
            //SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            //playtestCompleteUI.SetActive(true);
            //Time.timeScale = 0;
        }
    }
}
