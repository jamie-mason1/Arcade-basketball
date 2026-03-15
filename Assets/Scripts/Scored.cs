using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scored : MonoBehaviour
{
    AudioSource ding;
    [SerializeField] ParticleSystem confetti;
    [SerializeField] private TextMeshProUGUI scoreVal;
    int score;
    string eventPath = "event:/Score";
    public FmodHandler hand;

    HashSet<GameObject> playersThatScored = new HashSet<GameObject>();

    private void Awake()
    {
        ding = GameObject.Find("Ding").GetComponent<AudioSource>();
        score = 0;
    }
    private void Start()
    {
        hand = new FmodHandler(eventPath);
        scoreVal.text = "Score: " + score.ToString();
    }
     private void OnTriggerEnter(Collider other)
    {
        if (other.transform.position.y > transform.position.y + 0.1f)
        {
            if (other.CompareTag("Player"))
            {
                // If this player has already scored, do nothing
                if (playersThatScored.Contains(other.gameObject))
                    return;

                // Mark this player as having scored
                playersThatScored.Add(other.gameObject);

                ding.Play();

                if (hand == null)
                {
                    hand = new FmodHandler(eventPath);
                }

                hand.setSoundPlayPosition(transform.position);
                hand.StartEventSound();

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
        
    }

}
