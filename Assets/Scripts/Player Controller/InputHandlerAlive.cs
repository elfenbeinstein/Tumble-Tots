using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class InputHandlerAlive : NetworkBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private GameObject cam;

    [SerializeField] private Actor actor;
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    public NetworkRoomPlayer owner;

    bool isAlive;
    CommandMovement keyMove, keyRotate;

    private float moveX, moveZ;
    private Vector3 movementVector;
    private float targetAngle, angle;
    private float turnSmoothVelocity;

    [SerializeField] private float sprintMultiplier = 1;
    bool isSprinting;

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

    public Vector3 defaultSpawn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (!isLocalPlayer)
        {
            cam.SetActive(false);
        }
        else
            cam = GameObject.FindObjectOfType<Camera>().gameObject;

        isAlive = true;
        isSprinting = false;
        canDash = true;

        keyMove = new MoveActor();
        keyRotate = new RotateActor();

        EventSystem.Instance.AddEventListener(playerID, PlayerListener);
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        EventSystem.Instance.RemoveEventListener(playerID, PlayerListener);
    }

    void Update()
    {
        if (isAlive) { HandleInput(); }
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
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
            movementVector = new Vector3(moveX, 0f, moveZ);
            if (movementVector.magnitude > 1) movementVector = movementVector.normalized;

            if (movementVector.magnitude >= 0.1f)
            {
                //targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg;
                targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                keyRotate.Execute(actor, targetAngle);

                movementVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;

                if (isSprinting) movementVector *= sprintMultiplier;

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

    IEnumerator Dashing()
    {
        StartCoroutine(DashCoolDown());

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trigger")
        {
            Debug.Log("Trigger!");
            //transform.TransformVector(new Vector3(0, 1.6f, 1.46f));
            transform.position = GameObject.FindGameObjectWithTag("DefaultSpawn").transform.position;
        }
    }
}
