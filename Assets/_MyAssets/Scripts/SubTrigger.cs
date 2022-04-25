using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTrigger : MonoBehaviour
{
    public delegate void Triggered2D(Collider2D other, bool entered);
    public event Triggered2D onTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Sub Trigger Entered");
        onTrigger?.Invoke(collision, true); 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        print("Sub Trigger Exited");
        onTrigger?.Invoke(collision, false);
    }
}
