using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

/// <summary>
/// handles all behaviour for the alive player
/// --> move, jump, double jump, dash
/// </summary>

public class InputHandlerAlive : NetworkBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject camActual;

    [SerializeField] private Actor actor;
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    public NetworkRoomPlayer owner;

    bool isAlive; // obsolete, used during testing
    CommandMovement keyMove, keyRotate;

    private float moveX, moveZ;
    private Vector3 movementVector;
    private float targetAngle, angle; // angle also obsolete
    private float turnSmoothVelocity; // obsolete

    /* when we had sprint instead of dash -- obsolete
    [SerializeField] private float sprintMultiplier = 1;
    bool isSprinting;
    */

    [Space]
    [Header("Gravity related variables:")]
    Vector3 verticalVelocity;
    private float gravity = -9.81f; // m/s²
    [SerializeField] private Transform playerFeet;
    [SerializeField] bool isGrounded;
    [SerializeField] private LayerMask groundCheckMask;
    [SerializeField] private float jumpHeight;
    private bool canDoubleJump;

    [Space]
    [Header("Dashing related variables")]
    [SerializeField] private float dashForce = 24f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash;

    public float pushBackDuration;
    public string playerID;

    public GameObject currentSpawnpoint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (!isLocalPlayer)
        {
            cam.SetActive(false);
        }
        cam = camActual;

        //isAlive = true;
        //isSprinting = false;
        canDash = true;

        // set up for command movement
        keyMove = new MoveActor();
        keyRotate = new RotateActor();

        // obsolete -- used by lava (but not anymore, cause it now goes via the server)
        EventSystem.Instance.AddEventListener(playerID, PlayerListener);
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        EventSystem.Instance.RemoveEventListener(playerID, PlayerListener);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                if (canDash) 
                { 
                    StartCoroutine(Dashing());
                    EventSystem.Instance.Fire("AUDIO", "dash");
                }

            PlayerMovement();

            isGrounded = Physics.CheckSphere(playerFeet.position, 0.3f, groundCheckMask);
            if (isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
                canDoubleJump = true;
            }
            else
                verticalVelocity.y += gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
                PlayerJump();
            keyMove.Execute(actor, verticalVelocity * Time.deltaTime);
        }
    }

    private void PlayerMovement()
    {
        if (isLocalPlayer)
        {
            // Get Player Input:
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
            movementVector = new Vector3(moveX, 0f, moveZ);
            // normalise input:
            if (movementVector.magnitude > 1) movementVector = movementVector.normalized;


            if (movementVector.magnitude >= 0.1f)
            {
                // rotate the player according to the position they're moving in while adjusting for the current position of the camera:
                targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                //angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); obsolete
                keyRotate.Execute(actor, targetAngle);

                movementVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;

                //if (isSprinting) movementVector *= sprintMultiplier;

                keyMove.Execute(actor, movementVector * movementSpeed * Time.deltaTime);
            }
        }
    }

    private void PlayerJump()
    {
        if (isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            EventSystem.Instance.Fire("AUDIO", "jump");
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                EventSystem.Instance.Fire("AUDIO", "jump");
                canDoubleJump = false;
            }
        }
    }

    // adds extra force to the player while dashing
    IEnumerator Dashing()
    {
        StartCoroutine(DashCoolDown());

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            if (movementVector.magnitude < 0.1f) movementVector = Vector3.forward;
            keyMove.Execute(actor, movementVector * dashForce * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator DashCoolDown()
    {
        canDash = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // obsolete -- used to be used by lava for testing
    private void PlayerListener(string eventName, object param)
    {
        if (eventName == "TouchedLava")
        {
            if ((Actor)param == actor)
            {
                isAlive = false;
                Debug.Log("Player touched lava");
            }
        }
    }
   

    // these two functions are used for when a player is hit by a projectile
    public void Push(Vector3 direction, float duration)
    {
        StartCoroutine(PushBack(direction, duration));
    }

    IEnumerator PushBack(Vector3 direction, float duration)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            keyMove.Execute(actor, direction);
            yield return null;
        }
    }

    // for spawning players in levels 1/2 when they leave the course
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trigger")
        {
            Debug.Log("Trigger!");
            if (currentSpawnpoint == null)
            {
                transform.position = GameObject.FindGameObjectWithTag("DefaultSpawn").transform.position;
            }
            transform.position = currentSpawnpoint.transform.position;
        }
    }
}
