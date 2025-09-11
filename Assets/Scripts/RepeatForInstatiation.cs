using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatForInstatiation : MonoBehaviour
{

    public GameObject OriginalBall;
    public bool CanStartInstatiation = false;
    float currentTime = 0f;
    float timetoinstatiation = 0.66f;
    public Transform ballSpawnPoint;
    public float orginalReferenceHeight;

    private void Awake()
    {
        orginalReferenceHeight = OriginalBall.transform.position.y;

    }

    private void Start()
    {
        Instantiate(OriginalBall);
        OriginalBall.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (CanStartInstatiation)
        {
            if(currentTime < timetoinstatiation)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                GameObject BallCopy = Instantiate(OriginalBall);
                BallCopy.transform.rotation = ballSpawnPoint.rotation;
                BallCopy.transform.position = ballSpawnPoint.position;
                BallCopy.SetActive(true);
                CanStartInstatiation = false;
                currentTime = 0f;
            }
        }
    }
}
