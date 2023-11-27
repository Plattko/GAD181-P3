using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallReplayData : ReplayData
{
    public float spriteAlpha { get; private set; }
    public Vector2 size { get; private set; }

    public BreakableWallReplayData(Vector3 position, float spriteAlpha, Vector2 size)
    {
        this.position = position;
        this.spriteAlpha = spriteAlpha;
        this.size = size;
    }
}
