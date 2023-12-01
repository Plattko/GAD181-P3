using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReplayTrail : MonoBehaviour
{
    [SerializeField] private GameObject trail;

    [SerializeField] private float trailTimer;
    [SerializeField] private float trailInterval;

    public void Start()
    {
        trailTimer = 0;
    }

    private void Update()
    {
        trailTimer += Time.deltaTime;

        if (trailTimer >= trailInterval)
        {
            Instantiate(trail, this.transform.position, Quaternion.identity);
            trailTimer = 0;
        }
    }
}
