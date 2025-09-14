using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RepeatForInstatiation : MonoBehaviour
{

    public GameObject OriginalBall;
    public bool CanStartInstatiation = false;
    float currentTime = 0f;
    float timetoinstatiation = 0.66f;
    public Transform ballSpawnPoint;
    public float orginalReferenceHeight;
    [SerializeField] Image powerBar;


    public float startFillAmount; 

    private void Awake()
    {
        orginalReferenceHeight = OriginalBall.transform.position.y;

    }

    private void Start()
    {
        powerBar.fillAmount = 0f;
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
                float t = currentTime / timetoinstatiation; // goes 0 → 1
                powerBar.fillAmount = Mathf.Lerp(startFillAmount, 0f, t);

            }
            else
            {
                GameObject BallCopy = Instantiate(OriginalBall);
                BallCopy.transform.rotation = ballSpawnPoint.rotation;
                BallCopy.transform.position = ballSpawnPoint.position;
                BallCopy.SetActive(true);
                CanStartInstatiation = false;
                currentTime = 0f;
                powerBar.fillAmount = 0f;

            }
        }
    }
    public void SetPowerBarAmount(float charge)
    {
        charge = Mathf.Clamp01(charge);
        powerBar.fillAmount = charge;
    }
}
