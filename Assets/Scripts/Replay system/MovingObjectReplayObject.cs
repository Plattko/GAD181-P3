using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectReplayObject : ReplayObject
{
    public override void SetDataForFrame(ReplayData data)
    {
        MovingObjectReplayData movingObjectData = (MovingObjectReplayData) data;

        this.transform.position = movingObjectData.position;
    }
}
