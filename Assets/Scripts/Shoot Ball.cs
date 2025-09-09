using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    Rigidbody rb;
    bool isReadyToShoot;
    [SerializeField]Transform hoopPosition;
    [Header("Reference Settings")]
    float referenceDistance = 116.5373f; // horizontal distance to hoop for calibration
    [SerializeField] float referenceHeight = 3.05f; // hoop height - ball start height

    [Header("Ball Hold Settings")]
    [SerializeField] Transform holdPoint;
    [SerializeField] float maxChargeOffset = 5f;

    [Header("Shooting Settings")]
    [SerializeField] float arcAngleDegrees = 45f;
    [SerializeField] float maxPower; // calculated in Start

    private float chargeT = 0f;
    public float chargeSpeed = 1f;
    float gravity = Mathf.Abs(Physics.gravity.y);

    Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isReadyToShoot = true;
        cam = Camera.main;
        referenceHeight = hoopPosition.position.y - transform.position.y;
        maxPower = CalculateMaxPower(referenceDistance * 2f, referenceHeight, arcAngleDegrees);
        // Compute maxPower to reach ~2x reference distance

    }

    float CalculateMaxPower(float distance, float height, float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        // Projectile formula: v = sqrt(g * x^2 / (2 * (x * tanθ - y) * cos^2θ))
        float numerator = gravity * distance * distance;
        float denominator = 2f * (distance * Mathf.Tan(angleRad) - height) * cos * cos;

        if (denominator <= 0f) denominator = 0.01f; // prevent divide by zero

        float speed = Mathf.Sqrt(numerator / denominator);
        return speed;
    }

    public Vector3 CalculateLaunchVelocity()
    {
        Vector3 dir = cam.transform.forward.normalized;
        Quaternion arcRotation = Quaternion.AngleAxis(arcAngleDegrees, -cam.transform.right);

        Vector3 launchDir = arcRotation * dir;
        Debug.Log(launchDir);
        float speed = chargeT * maxPower;

        return launchDir * speed;
    }

    void Update()
    {
        if (isReadyToShoot)
        {
            Vector3 targetPosition = holdPoint.position - Vector3.up * (chargeT * maxChargeOffset);
            rb.transform.position = targetPosition;
            rb.transform.rotation = holdPoint.rotation;
            maxPower = CalculateMaxPower(referenceDistance * 2f, referenceHeight, arcAngleDegrees);

            if (Input.GetKey(KeyCode.Space))
            {
                chargeT += Time.deltaTime * chargeSpeed;
                chargeT = Mathf.Clamp01(chargeT);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                rb.isKinematic = false;
                Vector3 launchVelocity = CalculateLaunchVelocity();
                rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);

                isReadyToShoot = false;
                chargeT = 0f;
            }
        }
        
    }
}
