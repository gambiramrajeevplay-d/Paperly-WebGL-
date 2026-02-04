using UnityEngine;
using TMPro;

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

    [Header("Altitude Lock")]
    public float maxHeight = 5f;

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


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 🔥 Start speed at 30
        currentSpeed = Mathf.Clamp(30f, minSpeed, maxSpeed);
    }


    void Update()
    {
        if (!canControl)
            return; // ❌ no player input, no visuals update

        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = 0f;
        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S)) verticalInput = -1f;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = Mathf.Clamp(
                currentSpeed + boostAmount,
                minSpeed,
                maxSpeed
            );
        }

        if (speedText != null)
            speedText.text = currentSpeed.ToString("0");

        HandleVisuals();
        HandleBodyVisual();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isCrashed) return;

        isCrashed = true;
        canControl = false;

        // Store crash start position
        crashStartPosition = transform.position;

        // Reduce speed immediately (but not zero)
        currentSpeed = Mathf.Max(currentSpeed * 0.6f, minSpeed);

        // Add fake drag
        rb.drag = crashDrag;
        rb.useGravity = true;

        // Nose down
        climbAmount = -0.6f;

        // 🔥 UNFREEZE rotations for crash visuals
        rb.constraints = RigidbodyConstraints.None;
    }


    void FixedUpdate()
    {
        if (canControl)
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

        HandleSpeed();
        HandleMovement();
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

        if (climbAmount > 0f)
        {
            // climbing costs energy
            targetSpeed -= climbSpeedLoss * climbAmount;
        }
        else if (climbAmount < 0f)
        {
            // diving gains energy
            targetSpeed += diveSpeedGain * -climbAmount;
        }
        else
        {
            // gentle auto-glide acceleration
            targetSpeed += idleSpeedGain * Time.fixedDeltaTime;
        }

        // soft altitude cap
        if (transform.position.y >= maxHeight && climbAmount > 0f)
            targetSpeed = minSpeed;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            deceleration * Time.fixedDeltaTime
        );

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }



    // ================= MOVEMENT =================

    void HandleMovement()
    {
        Vector3 vel = rb.velocity;

        // CONSTANT forward motion
        vel.z = currentSpeed;

        // Horizontal glide (paper-like)
        float targetX = horizontalInput * sideMoveSpeed;
        vel.x = Mathf.Lerp(vel.x, targetX, Time.fixedDeltaTime * movementSmooth);

        // Vertical glide (floaty, NOT strong)
        float glideY = climbAmount * verticalMoveSpeed;
        vel.y = Mathf.Lerp(vel.y, glideY, Time.fixedDeltaTime * movementSmooth);

        rb.velocity = vel;
    }

    // ================= VISUALS =================

    void HandleVisuals()
    {
        // Pitch (climb / dive only)
        float pitch = -climbAmount * pitchAmount;

        Quaternion targetRot =
            Quaternion.Euler(-90f + pitch, 0f, 180f);

        planeVisual.localRotation = Quaternion.Slerp(
            planeVisual.localRotation,
            targetRot,
            Time.deltaTime * visualSmooth
        );
    }



    // ================= BODY VISUALS =================
    void HandleBodyVisual()
    {
        // LEFT / RIGHT input → Z rotation
        float targetRoll = -horizontalInput * bodyRollAmount;

        bodyRoll = Mathf.Lerp(
            bodyRoll,
            targetRoll,
            Time.deltaTime * bodyRollSmooth
        );

        // Apply ONLY Z rotation (keep X & Y untouched)
        Vector3 rot = transform.localEulerAngles;
        rot.z = bodyRoll;
        transform.localEulerAngles = rot;
    }

}
