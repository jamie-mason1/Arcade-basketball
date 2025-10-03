using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scored : MonoBehaviour
{
    AudioSource ding;
    [SerializeField] ParticleSystem confetti;
    [SerializeField] private TextMeshProUGUI scoreVal;
    [SerializeField] private TextMeshProUGUI streakVal;
    public int scoreStreak = 0;
    int score;
    private void Awake()
    {
        ding = GameObject.Find("Ding").GetComponent<AudioSource>();
        score = 0;
    }
    private void Start()
    {
        scoreVal.text = "Score: " + score.ToString();
        streakVal.text = "Current Streak: " + scoreStreak.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.position.y > transform.position.y + 0.1f) 
        {
            if (other.CompareTag("Player"))
            {
               ding.Play();
                score++;
                scoreVal.text = "Score: " + score.ToString();
                if (!confetti.isPlaying)
                {
                    confetti.Play();
                }
            }
        }

    }
    private void Update()
    {
        streakVal.text = "Current Streak: " + scoreStreak.ToString();

    }

}
