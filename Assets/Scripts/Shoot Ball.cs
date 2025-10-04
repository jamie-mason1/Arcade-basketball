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

    [SerializeField] private float squashAmount = 0f; // how much to squash
    [SerializeField] private float squashDuration = 0.2f; // how fast to squash
    [SerializeField] private float stretchDuration = 0.2f; // how fast to squash
    [SerializeField] private float stretchAmount = 0; // stretch after bounce

    private Vector3 originalScale;
    private bool isSquashing = false;


    [Header("Ball Hold Settings")]
    [SerializeField] Transform holdPoint;
    [SerializeField] float maxChargeOffset = 5f;

    [Header("Shooting Settings")]
    [SerializeField] float arcAngleDegrees = 45f;
    [SerializeField] float maxPower; // calculated in Start


    [SerializeField] GameObject ChildMesh;
    [SerializeField] private float shotVelolcityMultiplier;
    private float chargeT = 0f;
    public float chargeSpeed = 1f;
    float gravity = Mathf.Abs(Physics.gravity.y);
    [SerializeField] private AudioSource charging;
    [SerializeField] private AudioSource shot;
    SphereCollider col;
    Camera cam;
    public bool hasScored;
    public bool hasBeenCounted;

    public RepeatForInstatiation copyBall;
    float originalVel;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        isReadyToShoot = true;
        cam = Camera.main;
        referenceHeight = hoopPosition.position.y - copyBall.orginalReferenceHeight;
        maxPower = CalculateMaxPower(referenceDistance * 2f, referenceHeight, arcAngleDegrees);
        // Compute maxPower to reach ~2x reference distance
        originalVel = 0;
        col = GetComponent<SphereCollider>();
        col.enabled = false;
        originalScale = ChildMesh.transform.localScale;
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
                copyBall.SetPowerBarAmount(chargeT);
                charging.Play();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                charging.Stop();
                copyBall.ballsShotOrder.Add(gameObject);
                rb.isKinematic = false;
                Vector3 launchVelocity = CalculateLaunchVelocity();
                rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);
                StartCoroutine(EnableCollider(col, 2f));
                copyBall.startFillAmount = chargeT;
                shot.Play();
                isReadyToShoot = false;
                chargeT = 0f;
                if (copyBall.CanStartInstatiation == false)
                {
                    copyBall.CanStartInstatiation = true;
                    copyBall.shots++;
                }
            }
            
        }
        
    }
    private System.Collections.IEnumerator EnableCollider(SphereCollider col, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (col != null) // check in case it was destroyed earlier
        {
            col.enabled = true;
        }
    }
    

    
    void OnCollisionEnter(Collision collision)
    {
        if (!isSquashing)
        {
            // Run squash/stretch animation
            StartCoroutine(SquashEffect(collision.relativeVelocity.magnitude));
        }
    }


    private IEnumerator SquashEffect(float impactForce)
    {
        isSquashing = true;

        // Optional: use impact force to scale intensity
        float squashIntensity = Mathf.Clamp(impactForce / 10f, 0.8f, 1.5f);

        // Step 1: Squash
        Vector3 squashedScale = new Vector3(
            originalScale.x * (1 + squashAmount * squashIntensity),
            originalScale.y * (1 - squashAmount * squashIntensity),
            originalScale.z * (1 + squashAmount * squashIntensity)
        );

        float elapsed = 0f;
        while (elapsed < squashDuration)
        {
            ChildMesh.transform.localScale = Vector3.Lerp(originalScale, originalScale, elapsed / squashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Step 2: Stretch (after bounce)
        Vector3 stretchedScale = new Vector3(
            originalScale.x * (1 - squashAmount * 0.5f),
            originalScale.y * (1 + stretchAmount * squashIntensity),
            originalScale.z * (1 - squashAmount * 0.5f)
        );

        elapsed = 0f;
        while (elapsed < squashDuration)
        {
            ChildMesh.transform.localScale = Vector3.Lerp(originalScale, squashedScale, elapsed / squashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Step 3: Return to normal
        elapsed = 0f;
        while (elapsed < stretchDuration)
        {
            ChildMesh.transform.localScale = Vector3.Lerp(squashedScale, stretchedScale, elapsed / squashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ChildMesh.transform.localScale = originalScale;
        
        isSquashing = false;
    }
}










