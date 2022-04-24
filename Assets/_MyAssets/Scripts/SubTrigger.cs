using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTrigger : MonoBehaviour
{
    public delegate void OnTriggerEnter(Collider other);
    public delegate void OnTriggerExit(Collider other);
    public event OnTriggerEnter onTriggerEnter;
    public event OnTriggerExit onTriggerExit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
