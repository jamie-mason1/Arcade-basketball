using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnColliderToTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.isTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.isTrigger = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
