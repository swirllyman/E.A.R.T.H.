using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTrigger : MonoBehaviour
{
    public delegate void Triggered2D(Collider2D other, bool entered);
    public event Triggered2D onTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTrigger?.Invoke(collision, true); 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onTrigger?.Invoke(collision, false);
    }
}
