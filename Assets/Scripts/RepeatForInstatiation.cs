using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RepeatForInstatiation : MonoBehaviour
{

    public GameObject OriginalBall;
    public GameObject OriginalBallMesh;
    public bool CanStartInstatiation = false;
    float currentTime = 0f;
    float timetoinstatiation = 0.66f;
    public Transform ballSpawnPoint;
    public float orginalReferenceHeight;
    [SerializeField] Image powerBar;
    public int shots;
    [SerializeField] private TextMeshProUGUI numShots;
    [SerializeField] float maxTime = 5f;
    float cur = 0;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] float x = 1;
    [SerializeField] float y = 4;
    public float startFillAmount;
    public List<GameObject> ballsShotOrder = new List<GameObject>();
    bool gameover;

    private void Awake()
    {
        orginalReferenceHeight = OriginalBall.transform.position.y;
    }

    private void Start()
    {
        powerBar.fillAmount = 0f;
        Instantiate(OriginalBall);
        OriginalBall.SetActive(false);
        shots = 0;
        int minutes = Mathf.FloorToInt(maxTime / 60);
        int seconds = Mathf.FloorToInt(maxTime % 60);
        cur = maxTime;
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        numShots.text = "Shots: " + shots.ToString();
        timer.text = "Time : " + time;
        gameover = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (cur > 0)
        {
            if (CanStartInstatiation)
            {
                numShots.text = "Shots: " + shots.ToString();
                if (currentTime < timetoinstatiation)
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
            cur = cur - (Time.deltaTime*x)/y;
            int minutes = Mathf.FloorToInt(cur / 60);
            int seconds = Mathf.FloorToInt(cur % 60);
            string time = string.Format("{0:00}:{1:00}", minutes, seconds);
            timer.text = "Time : " + time;


        }
        else
        {
            gameover = false;
        }
    }
    public void SetPowerBarAmount(float charge)
    {
        charge = Mathf.Clamp01(charge);
        powerBar.fillAmount = charge;
    }
}
