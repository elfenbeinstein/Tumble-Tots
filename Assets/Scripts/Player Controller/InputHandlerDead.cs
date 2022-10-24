using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerDead : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Actor actor;
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    bool isAlive;
    CommandMovement keyMove, keyRotate;
    CommandDeadMovement keyDeadMove;

    private float moveX, moveZ;
    private Vector3 movementVector;
    private float targetAngle, angle;
    private float turnSmoothVelocity;
    private float flyDirection;

    void Start()
    {
        keyDeadMove = new MoveDead();
        keyMove = new MoveActor();
        keyRotate = new RotateActor();
    }

    void Update()
    {
        Debug.Log(movementVector.y + ";" +  flyDirection);
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        movementVector = new Vector3(moveX, 0f, moveZ);

        if (Input.GetKey(KeyCode.Q)) flyDirection = 1;
        else if (Input.GetKey(KeyCode.E)) flyDirection = -1;
        else flyDirection = 0;

        //if (movementVector.magnitude > 1) movementVector = movementVector.normalized; 

        if (movementVector.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            keyRotate.Execute(actor, targetAngle);

            movementVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;;

            keyMove.Execute(actor, movementVector * movementSpeed * Time.deltaTime);
            //keyMove.Execute(actor, Vector3.right * moveX * movementSpeed * Time.deltaTime);
            //keyMove.Execute(actor, Vector3.forward * moveZ * movementSpeed * Time.deltaTime);
        }
        
        if (flyDirection != 0)
        {
            keyMove.Execute(actor, Vector3.up * flyDirection * movementSpeed * Time.deltaTime);
        }
    }
}
