using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnBallLocation : MonoBehaviour
{


    [SerializeField] Transform holdPoint;


    Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Compute maxPower to reach ~2x reference distance

    }





    void Update()
    {

        transform.position = holdPoint.position;
        transform.rotation = holdPoint.rotation;
    }

}
