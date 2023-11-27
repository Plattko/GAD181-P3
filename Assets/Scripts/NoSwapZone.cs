using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSwapZone : MonoBehaviour
{
    [SerializeField] private PlayerController p1Controller;
    [SerializeField] private PlayerController p2Controller;
    [SerializeField] private UIManager uiManager;

    private List<Transform> players = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count >= 1)
        {
            p1Controller.canSwap = false;
            p2Controller.canSwap = false;
            uiManager.EnableNoSwapUI();
        }
        else if (players.Count < 1)
        {
            p1Controller.canSwap = true;
            p2Controller.canSwap = true;
            uiManager.DisableNoSwapUI();
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
}
