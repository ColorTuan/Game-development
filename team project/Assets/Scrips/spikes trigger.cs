using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class spikestrigger : MonoBehaviour
{
    private bool isspikesTrigger;
    private BoxCollider2D boxCollider2D;
    void Update()
    {
        if (boxCollider2D.isTrigger)
        {
            isspikesTrigger = true;
            Debug.Log(isspikesTrigger);
        }
    }
}
