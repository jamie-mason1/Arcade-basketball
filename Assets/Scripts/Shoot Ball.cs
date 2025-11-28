using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    Rigidbody rb;
    Material mat;
    TrailRenderer trail;
    private Color originalAlbedo;
    public Color targetColor = Color.red;


    bool isReadyToShoot;
    [SerializeField]Transform hoopPosition;
    [Header("Reference Settings")]
    float referenceDistance = 116.5373f; // horizontal distance to hoop for calibration
    [SerializeField] float referenceHeight = 3.05f; // hoop height - ball start height

    [SerializeField] private float squashAmount = 0f; // how much to squash
    [SerializeField] private float squashDuration = 0.2f; // how fast to squash
    [SerializeField] private float stretchDuration = 0.2f; // how fast to squash
    [SerializeField] private float stretchAmount = 0; // stretch after bounce

    public ParticleSystem smoke;
    public ParticleSystem ring;
    public ParticleSystem EnergyAura;          // Reference to the aura particle system
    [SerializeField] private float auraMaxSize = 1.5f;       // Maximum size multiplier
    [SerializeField] private float auraMaxEmission = 50f;    // Maximum emission rate at full charge
    [SerializeField] private float vibrationBoost = 0.5f;    // Extra boost during vibration
    [SerializeField] private float vibrationRampTime = 5f;   // Time to reach max vibration boost

    private float auraChargeMultiplier = 0f;
    private float auraVibrationMultiplier = 0f;
    private bool isAuraFading = false; private Vector3 originalScale;
    private bool isSquashing = false;

    float originalStartSizeSmoke;
    float originalStartSizeRing;

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

    private float timeAtFullCharge = 0f;
    private bool isFullyCharged = false;
    private bool vibrationStarted = false;
    private bool MustChargeAgain;
    private bool CanChargeAgain;
    private bool isVibrating = false;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<TrailRenderer>() != null)
            {
                trail = transform.GetChild(i).GetComponent<TrailRenderer>();
                break;
            }
        }
        trail.gameObject.SetActive(false);

        if (ChildMesh == null)
        {
            // Get the first Renderer in children
            Renderer childRenderer = GetComponentInChildren<Renderer>();
            if (childRenderer != null)
            {
                ChildMesh = childRenderer.gameObject;
            }
            else
            {
                Debug.LogError("ShootBall: No child mesh with Renderer found!");
            }
        }
        MustChargeAgain = false;
        CanChargeAgain = true;
        Renderer rend = ChildMesh.GetComponent<Renderer>();
        originalStartSizeSmoke = smoke.startSize;
        originalStartSizeRing = ring.startSize;
        // Create a unique material instance for this ball
        mat = new Material(rend.material);
        rend.material = mat;  // Assign it back to the Renderer

        originalAlbedo = mat.color;
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
                if (CanChargeAgain)
                {
                    if (!isFullyCharged)
                    {
                        chargeT += Time.deltaTime * chargeSpeed;
                        chargeT = Mathf.Clamp01(chargeT);
                        copyBall.SetPowerBarAmount(chargeT);

                        auraChargeMultiplier = chargeT; // set aura based on charge
                        UpdateEnergyAura(auraChargeMultiplier, auraVibrationMultiplier);

                        chargeT += Time.deltaTime * chargeSpeed;
                        chargeT = Mathf.Clamp01(chargeT);
                        copyBall.SetPowerBarAmount(chargeT);
                        charging.Play();
                        
                        smoke.transform.position = copyBall.ballSpawnPoint.position + Vector3.up;
                        ring.transform.position = copyBall.ballSpawnPoint.position + Vector3.up;
                        mat.color = Color.Lerp(originalAlbedo, targetColor, chargeT);
                        //smoke.startColor = mat.color;
                        if (chargeT >= 1f)
                        {
                            isFullyCharged = true;
                            timeAtFullCharge = 0f;
                        }
                    }
                    else
                    {
                        timeAtFullCharge += Time.deltaTime;

                        if (timeAtFullCharge > 5f && !vibrationStarted)
                        {
                            StartCoroutine(VibrateBall(0.8f, 0.001f, 0.01f, 40f, 20f));
                            vibrationStarted = true;
                        }
                        if (timeAtFullCharge >= 30f)
                        {
                            MustChargeAgain = true;
                            CanChargeAgain = false;
                            StartCoroutine(DissipateCharge());
                            isVibrating = false;
                            
                        }

                    }
                    smoke.startSize = originalStartSizeSmoke * chargeT;
                    ring.startSize = originalStartSizeRing * chargeT;
                }

            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                charging.Stop();
                if (MustChargeAgain == false)
                {
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
                    var smokeMain = smoke.main;
                    smokeMain.startSize = Mathf.Lerp(0.5f, 2.0f, chargeT);  // Adjust range as needed

                    var ringMain = ring.main;
                    ringMain.startSize = Mathf.Lerp(originalStartSizeRing, originalStartSizeRing * 3f, chargeT);

                    // Stop previous emissions cleanly before replaying
                    smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ring.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                    smoke.Play();
                    ring.Play();
                    trail.gameObject.SetActive(true);
                    StartCoroutine(FadeOutEnergyAura(1.5f));

                }
                mat.color = originalAlbedo;
                MustChargeAgain = false;
                CanChargeAgain = true;
                isVibrating = false;
                ring.startSize = originalStartSizeRing;
                smoke.startSize = originalStartSizeSmoke;



            }

        }
        
    }

    private void UpdateEnergyAura(float chargeMultiplier, float vibrationMultiplier = 0f)
    {
        if (EnergyAura == null) return;

        var main = EnergyAura.main;
        var emission = EnergyAura.emission;

        // Combine charge + vibration, clamp to 1.5
        float totalMultiplier = Mathf.Clamp(chargeMultiplier + vibrationMultiplier, 0f, 1f + vibrationBoost);

        // Scale size and emission
        main.startSize = auraMaxSize * totalMultiplier;
        emission.rateOverTime = auraMaxEmission * totalMultiplier;

        // Optional: fade alpha as multiplier
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, totalMultiplier));

        // Play/stop depending on totalMultiplier
        if (!EnergyAura.isPlaying && totalMultiplier > 0f)
            EnergyAura.Play();
        else if (totalMultiplier <= 0f && !isAuraFading)
            EnergyAura.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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

    private IEnumerator DissipateCharge()
    {
        float dissipateDuration = 1.5f; // how long to fade out
        float elapsed = 0f;
        float startCharge = chargeT;

        while (elapsed < dissipateDuration)
        {
            elapsed += Time.deltaTime;
            chargeT = Mathf.Lerp(startCharge, 0f, elapsed / dissipateDuration);
            copyBall.SetPowerBarAmount(chargeT);
            mat.color = Color.Lerp(targetColor, originalAlbedo, elapsed / dissipateDuration);
            yield return null;
        }

        chargeT = 0f;
        isFullyCharged = false;
        vibrationStarted = false;
        timeAtFullCharge = 0f;
        StartCoroutine(FadeOutEnergyAura(1.5f));


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
    private IEnumerator VibrateBall(
    float duration = 0.2f,
    float startIntensity = 0.05f,
    float endIntensity = 0.1f,
    float frequency = 30f,
    float mt = 5f)
    {
        Vector3 originalPos = ChildMesh.transform.localPosition;
        float vibrationElapsed = 0f;
        float nextPulseTime = 0f;

        isVibrating = true;

        while (isVibrating)
        {
            // Increase elapsed time smoothly
            vibrationElapsed += Time.deltaTime;

            // Smooth intensity increase over mt seconds
            float overallT = Mathf.Clamp01(vibrationElapsed / mt);
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, overallT);

            // Aura ramp
            auraVibrationMultiplier = Mathf.Lerp(0f, vibrationBoost, vibrationElapsed / vibrationRampTime);

            // Frequency timing
            if (Time.time >= nextPulseTime)
            {
                // Shake once
                float offsetX = Random.Range(-currentIntensity, currentIntensity);
                float offsetY = Random.Range(-currentIntensity, currentIntensity);
                float offsetZ = Random.Range(-currentIntensity, currentIntensity);

                ChildMesh.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, offsetZ);

                nextPulseTime = Time.time + (1f / frequency);
            }

            UpdateEnergyAura(auraChargeMultiplier, auraVibrationMultiplier);

            yield return null;
        }

        // Reset
        ChildMesh.transform.localPosition = originalPos;
        vibrationStarted = false;
    }


    private IEnumerator FadeOutEnergyAura(float fadeDuration = 1f)
    {
        isAuraFading = true;

        float elapsed = 0f;
        float startCharge = auraChargeMultiplier + auraVibrationMultiplier;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Lerp(startCharge, 0f, elapsed / fadeDuration);

            UpdateEnergyAura(t, 0f); // gradually reduce aura

            yield return null;
        }

        auraChargeMultiplier = 0f;
        auraVibrationMultiplier = 0f;
        UpdateEnergyAura(0f, 0f);
        isAuraFading = false;
    }

}










