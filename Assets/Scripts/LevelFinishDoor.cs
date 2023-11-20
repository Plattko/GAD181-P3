using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishDoor : MonoBehaviour
{
    private List<Transform> players = new List<Transform>();
    private bool levelComplete = false;
    
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
        SceneManager.LoadScene("Tutorial_1");
    }
}
