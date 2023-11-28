using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTrigger : MonoBehaviour
{
    public BreakableSurface breakableSurface;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        breakableSurface.Break(collision);
    }
}
