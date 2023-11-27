using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallReplayObject : ReplayObject
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void SetDataForFrame(ReplayData data)
    {
        BreakableWallReplayData wallData = (BreakableWallReplayData) data;

        this.transform.position = wallData.position;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, wallData.spriteAlpha);
    }
}
