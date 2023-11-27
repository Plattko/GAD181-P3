using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallReplayData : ReplayData
{
    public float spriteAlpha { get; private set; }

    public BreakableWallReplayData(Vector3 position, float spriteAlpha)
    {
        this.position = position;
        this.spriteAlpha = spriteAlpha;
    }
}
