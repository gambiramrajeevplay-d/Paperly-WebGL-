using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PlaneController : MonoBehaviour
{
    [Header("Forward Speed")]
    public float minSpeed = 10f;
    public float maxSpeed = 60f;
    public float acceleration = 12f;
    public float deceleration = 16f;
    public float idleAcceleration = 4f; // 🔥 speed gain when no input

    [Header("Movement")]
    public float sideMoveSpeed = 10f;
    public float verticalMoveSpeed = 8f;
    public float movementSmooth = 4f;

    [Header("Climb Feel")]
    public float climbResponse = 2.5f;
    public float climbDrag = 8f;

    [Header("Boost")]
    public float boostAmount = 12f;

    [Header("UI")]
    public TextMeshProUGUI speedText;

    [Header("Forward Speed Control")]
    public float accelRate = 8f;   // how fast +Z speed increases
    public float decelRate = 1f;   // how slow speed decreases



    private Rigidbody rb;
    private float currentSpeed;

    private float horizontalInput;
    private float verticalInput;
    private float climbAmount; // -1 (dive) to +1 (climb)

    [Header("Speed Change Rates")]
    public float climbSpeedLoss = 6f;   // how fast speed drops when climbing
    public float diveSpeedGain = 8f;    // how fast speed increases when diving
    public float idleSpeedGain = 2f;    // how fast speed increases normally

    [Header("Visuals")]
    public Transform planeVisual;

    public float pitchAmount = 20f;   // up/down tilt
    public float rollAmount = 30f;    // left/right banking
    public float visualSmooth = 6f;

    [Header("Plane Body Visual (Root Tilt)")]
    public float bodyRollAmount = 12f;   // how much the whole plane tilts
    public float bodyRollSmooth = 6f;    // how smooth it returns
    private float bodyRoll;

    [Header("Crash Behaviour")]
    public bool canControl = true;
    private bool isCrashed = false;

    public float crashDrag = 1.5f;
    public float crashFallSpeed = 3f;
    public float crashSpeedLoss = 10f;

    [Header("Crash Stop")]
    public float crashStopDistance = 20f;

    private Vector3 crashStartPosition;
    private bool hasStopped = false;


    [Header("Stall Settings")]
    public float stallSpeed = 6f;        // speed at which plane stalls
    public float stallFallForce = 1.5f;  // how hard the plane drops
    public float stallRecoverySpeed = 12f; // speed needed to regain control

    private bool isStalling = false;


    [Header("Climb Slowdown (No Height Limit)")]
    public float climbSlowdownMultiplier = 0.6f; // how much speed is lost while climbing
    public float minClimbSpeed = 18f;            // minimum speed while climbing

    [Header("Climb Speed Control")]
    public float climbSpeedDecayRate = 1f; // speed lost per second while climbing

    [Header("Critical Speed Zone")]
    public float criticalSpeed = 18f;
    public float criticalClimbDecayMultiplier = 3f; // how brutal speed loss becomes


    [Header("Mobile Controls")]
    public Joystick_Mobile joystick; // drag your joystick here

    [Header("Altitude Speed Zone")]
    public float safeMinHeight = -20f;
    public float safeMaxHeight = 20f;

    [Header("Crash Sound")]
    public AudioClip crashSound;
    private AudioSource audioSource;

    [Header("Boost System")]
    public bool hasBoost = false;
    public bool isBoosting = false;

    public float boostSpeedAmount = 25f;
    public float boostDuration = 2f;

    [Header("Boost Visual")]
    public float boostRotationSpeed = 720f; // Z rotation speed
    public float boostFOV = 85f;
    public float normalFOV = 60f;
    public float fovSmooth = 5f;

    private Camera mainCam;
    private Coroutine boostCoroutine;

    public GameObject boostPopup;


    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        // Get camera by tag
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
        {
            mainCam = camObj.GetComponent<Camera>();
            normalFOV = mainCam.fieldOfView;
        }



        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 🔥 Start speed at 30
        currentSpeed = Mathf.Clamp(30f, minSpeed, maxSpeed);
    }


    void Update()
    {
        if (!canControl) return;

        // 🔁 Keyboard fallback (Editor)
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = 0f;
        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S)) verticalInput = -1f;

        // 📱 Mobile joystick (Editor + Mobile)
        if (joystick != null)
        {
            if (Mathf.Abs(joystick.Horizontal) > 0.05f)
                horizontalInput = joystick.Horizontal;

            if (Mathf.Abs(joystick.Vertical) > 0.05f)
                verticalInput = joystick.Vertical;
        }

        if (speedText != null)
            speedText.text = currentSpeed.ToString("0");

        // 🚀 Boost Activation
        if (hasBoost && !isBoosting && Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (boostCoroutine != null)
                StopCoroutine(boostCoroutine);

            boostCoroutine = StartCoroutine(BoostRoutine());
        }


        HandleVisuals();
        HandleBodyVisual();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (isCrashed) return;

        isCrashed = true;
        canControl = false;

        // 🔊 PLAY CRASH SOUND
        if (crashSound != null)
        {
            audioSource.PlayOneShot(crashSound);
        }

        crashStartPosition = transform.position;

        currentSpeed = Mathf.Max(currentSpeed * 0.6f, minSpeed);

        rb.drag = crashDrag;
        rb.useGravity = true;

        climbAmount = -0.6f;

        rb.constraints = RigidbodyConstraints.None;
    }



    void FixedUpdate()
    {
        // Normal input control ONLY if not stalling
        if (canControl && !isStalling)
        {
            if (verticalInput != 0f)
            {
                climbAmount = Mathf.Lerp(
                    climbAmount,
                    verticalInput,
                    Time.fixedDeltaTime * climbResponse
                );
            }
        }

        if (isCrashed)
        {
            HandleCrashMotion();
            return;
        }

        // 🛑 ENTER STALL
        if (!isStalling && climbAmount > 0f && currentSpeed <= stallSpeed)
        {
            isStalling = true;
        }

        // ⬇️ STALL BEHAVIOUR (THIS WAS MISSING)
        if (isStalling)
        {
            // Force nose down
            climbAmount = Mathf.Lerp(
                climbAmount,
                -1f,
                Time.fixedDeltaTime * stallFallForce
            );

            // Recover once speed is back
            if (currentSpeed >= stallRecoverySpeed)
            {
                isStalling = false;
            }
        }

        HandleSpeed();
        HandleMovement();
    }

    IEnumerator BoostRoutine()
    {
        isBoosting = true;
        hasBoost = false;

        float originalMaxSpeed = maxSpeed;
        maxSpeed += boostSpeedAmount;

        float timer = 0f;

        if (boostPopup != null)
            boostPopup.SetActive(false);


        while (timer < boostDuration)
        {
            timer += Time.deltaTime;

            // 🎥 Smooth FOV increase
            if (mainCam != null)
            {
                mainCam.fieldOfView = Mathf.Lerp(
                    mainCam.fieldOfView,
                    boostFOV,
                    Time.deltaTime * fovSmooth
                );
            }

            yield return null;
        }

        maxSpeed = originalMaxSpeed;
        isBoosting = false;

        // 🎥 Reset FOV smoothly
        if (mainCam != null)
        {
            StartCoroutine(ResetFOV());
        }
    }

    IEnumerator ResetFOV()
    {
        while (mainCam.fieldOfView > normalFOV + 0.1f)
        {
            mainCam.fieldOfView = Mathf.Lerp(
                mainCam.fieldOfView,
                normalFOV,
                Time.deltaTime * fovSmooth
            );

            yield return null;
        }

        mainCam.fieldOfView = normalFOV;
    }


    void HandleCrashMotion()
    {
        if (hasStopped) return;

        float travelled = Vector3.Distance(crashStartPosition, transform.position);

        if (travelled >= crashStopDistance)
        {
            // 🛑 FULL STOP
            hasStopped = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.drag = 5f;

            return;
        }

        Vector3 vel = rb.velocity;

        // Gradually lose forward speed
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            minSpeed,
            crashSpeedLoss * Time.fixedDeltaTime
        );

        vel.z = currentSpeed;

        // Paper fall
        vel.y = Mathf.Lerp(
            vel.y,
            -crashFallSpeed,
            Time.fixedDeltaTime * 1.5f
        );

        rb.velocity = vel;
    }


    // ================= SPEED =================

    void HandleSpeed()
    {
        float targetSpeed = currentSpeed;

        float height = transform.position.y;
        bool inSafeHeightZone = height >= safeMinHeight && height <= safeMaxHeight;

        // ================= CLIMB =================
        if (climbAmount > 0f)
        {
            if (!inSafeHeightZone)
            {
                // Normal climb slowdown (outside safe zone)
                float speed01 = Mathf.InverseLerp(
                    criticalSpeed,
                    maxSpeed,
                    currentSpeed
                );

                float climbLoss = Mathf.Lerp(
                    climbSpeedDecayRate,
                    climbSpeedDecayRate * criticalClimbDecayMultiplier,
                    speed01
                );

                targetSpeed -= climbLoss * climbAmount;
            }
            else
            {
                // 🔥 SAFE HEIGHT: maintain speed
                targetSpeed += idleSpeedGain * 0.5f;
            }
        }

        // ================= DIVE =================
        else if (climbAmount < 0f)
        {
            targetSpeed += diveSpeedGain * -climbAmount;
        }

        // ================= LEVEL FLIGHT =================
        else
        {
            targetSpeed += idleSpeedGain;
        }

        targetSpeed = Mathf.Clamp(targetSpeed, minSpeed, maxSpeed);

        float rate;
        if (climbAmount > 0f && targetSpeed < currentSpeed)
            rate = decelRate * 6f;
        else
            rate = targetSpeed > currentSpeed ? accelRate : decelRate;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        if (inSafeHeightZone)
        {
            targetSpeed = Mathf.Max(targetSpeed, maxSpeed * 0.7f);
        }

        // ================= STALL CHECK =================
        if (!inSafeHeightZone && climbAmount > 0f && currentSpeed <= stallSpeed)
        {
            isStalling = true;
        }
    }





    // ================= MOVEMENT =================

    void HandleMovement()
    {
        Vector3 vel = rb.velocity;

        // 🔥 Forward speed
        vel.z = currentSpeed;

        // ================= TURN CONTROL =================
        // Turning strength depends on speed
        float turnFactor = Mathf.InverseLerp(
            stallSpeed,   // barely controllable
            maxSpeed,     // full authority
            currentSpeed
        );

        turnFactor = Mathf.Clamp01(turnFactor);

        float targetX = horizontalInput * sideMoveSpeed * turnFactor;
        vel.x = Mathf.Lerp(
            vel.x,
            targetX,
            Time.fixedDeltaTime * movementSmooth
        );

        // ================= CLIMB / FALL =================
        float speedFactor = Mathf.Lerp(
            0.6f,   // weak lift at low speed
            1.2f,   // strong lift at high speed
            Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed)
        );

        float climbBoost = Mathf.Lerp(1f, 1.6f, Mathf.Abs(climbAmount));
        float glideY = climbAmount * verticalMoveSpeed * speedFactor * climbBoost;


        // Extra fall while stalling
        if (isStalling)
        {
            glideY -= stallFallForce;
        }

        vel.y = Mathf.Lerp(
            vel.y,
            glideY,
            Time.fixedDeltaTime * movementSmooth
        );

        rb.velocity = vel;
    }



    // ================= VISUALS =================

    void HandleVisuals()
    {
        // Speed-based responsiveness
        float speed01 = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

        float dynamicPitch = pitchAmount * Mathf.Lerp(0.4f, 1.1f, speed01);
        float dynamicSmooth = Mathf.Lerp(3f, visualSmooth, speed01);

        float pitch = -climbAmount * dynamicPitch;

        Quaternion targetRot =
            Quaternion.Euler(-90f + pitch, 0f, 180f);

        planeVisual.localRotation = Quaternion.Slerp(
            planeVisual.localRotation,
            targetRot,
            Time.deltaTime * dynamicSmooth
        );

        //  Rotate whole plane on Z while boosting
        //if (isBoosting)
        //{
        //    transform.Rotate(Vector3.forward * boostRotationSpeed * Time.deltaTime);
        //}

    }




    // ================= BODY VISUALS =================
    private float boostZRotation = 0f;

    void HandleBodyVisual()
    {
        // Base banking roll
        float targetRoll = -horizontalInput * bodyRollAmount;

        bodyRoll = Mathf.Lerp(
            bodyRoll,
            targetRoll,
            Time.deltaTime * bodyRollSmooth
        );

        // 🔥 Add boost rotation using your existing boostRotationSpeed
        if (isBoosting)
        {
            boostZRotation += boostRotationSpeed * Time.deltaTime;
        }
        else
        {
            boostZRotation = Mathf.Lerp(boostZRotation, 0f, Time.deltaTime * 3f);
        }

        float finalZ = bodyRoll + boostZRotation;

        Vector3 rot = transform.localEulerAngles;
        rot.z = finalZ;
        transform.localEulerAngles = rot;
    }

}
