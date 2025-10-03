using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBall : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;
    ShootBall shoot;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            shoot = other.GetComponent<ShootBall>();
            GameObject lastBall = shoot.copyBall.ballsShotOrder[shoot.copyBall.ballsShotOrder.Count - 1];

            // Only check scoring for the last ball
            if (other.gameObject == lastBall)
            {
                if (shoot.hasScored)
                {
                    if (shoot.hasBeenCounted == false) { 
                        shoot.scored.scoreStreak++;
                        shoot.hasBeenCounted = true;
                    }
                }
                else
                {
                    shoot.scored.scoreStreak = 0;
                }
            }
            StartCoroutine(DestroyAfterDelay(other.gameObject, 20f));

        }
    }
    private System.Collections.IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null) // check in case it was destroyed earlier
        {
            explosion.transform.position = obj.transform.position;
            if (!explosion.isPlaying)
            {
                explosion.Play();
            }
            for (int i = shoot.copyBall.ballsShotOrder.Count - 1; i >= 0; i--)
            {
                if (shoot.copyBall.ballsShotOrder[i] == obj)
                {
                    shoot.copyBall.ballsShotOrder.RemoveAt(i);
                }
            }

            Destroy(obj);
        }
    }
}
