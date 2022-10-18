using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerDead : MonoBehaviour
{
    [SerializeField] private Actor actor;
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    bool isAlive;
    CommandMovement keyMove, keyRotate;

    private float moveX, moveZ;
    private Vector3 movementVector;
    private float targetAngle, angle;
    private float turnSmoothVelocity;

    void Start()
    {
        keyMove = new MoveActor();
        keyRotate = new RotateActor();
    }

    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        movementVector = new Vector3(moveX, 0f, moveZ);
        if (movementVector.magnitude > 1) movementVector = movementVector.normalized;

        if (movementVector.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            keyRotate.Execute(actor, targetAngle);

            movementVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;;

            keyMove.Execute(actor, movementVector * movementSpeed * Time.deltaTime);
        }
    }
}
