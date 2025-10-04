using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardVisualWhip : MonoBehaviour
{
    [Header("Whip Settings")]
    [SerializeField] private float whipAngle = 10f;          // Max rotation angle
    [SerializeField] private float whipSpeed = 20f;          // Speed of the whip motion
    [SerializeField] private float returnSpeed = 5f;         // How quickly it returns
    [SerializeField] private float damping = 0.2f;           // Small oscillation on return
    [SerializeField] private Vector3 rotationAxis = Vector3.right; // Axis of rotation

    private Quaternion originalRotation;
    private bool isWhipping = false;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    public void TriggerWhip(float impactForce, Vector3 hitPoint)
    {
        if (!isWhipping)
            StartCoroutine(WhipBack(impactForce, hitPoint));
    }

    private IEnumerator WhipBack(float impactForce, Vector3 hitPoint)
    {
        isWhipping = true;

        // Calculate how strong the whip is
        float intensity = Mathf.Clamp(impactForce / 10f, 0.3f, 1.5f);
        float angle = whipAngle * intensity;

        // Whip backward
        float elapsed = 0f;
        while (elapsed < 1f / whipSpeed)
        {
            float t = elapsed * whipSpeed;
            float currentAngle = Mathf.Sin(t * Mathf.PI) * -angle;
            transform.localRotation = originalRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return and slightly oscillate (spring effect)
        float returnTime = 0f;
        while (returnTime < 1f)
        {
            float spring = Mathf.Exp(-returnTime * returnSpeed) * Mathf.Sin(returnTime * returnSpeed * (1f - damping)) * angle * 0.5f;
            transform.localRotation = originalRotation * Quaternion.AngleAxis(spring, rotationAxis);
            returnTime += Time.deltaTime;
            yield return null;
        }

        // Reset
        transform.localRotation = originalRotation;
        isWhipping = false;
    }
}
