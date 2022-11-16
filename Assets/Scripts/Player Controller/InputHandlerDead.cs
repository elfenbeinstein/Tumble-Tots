using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class InputHandlerDead : NetworkBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private Actor actor;
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    bool canShoot;

    CommandMovement keyMove, keyRotate;

    private float moveX, moveZ;
    private Vector3 movementVector;
    private float targetAngle, angle;
    private float turnSmoothVelocity;
    private float flyDirection;
    public float cooldown;

    public GameObject projectile;

    void Start()
    {
        if (isLocalPlayer)
        {
            cam.SetActive(true);
        }
        else
            cam = GameObject.FindObjectOfType<Camera>().gameObject;

        keyMove = new MoveActor();
        keyRotate = new RotateActor();

        Cursor.visible = false;

        canShoot = true; // Player can shoot at the beginning
    }

    void Update()
    {
        PlayerMovement();
        Shooting();
    }

    private void PlayerMovement()
    {
        if (isLocalPlayer)
        {
            // get player input:
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
            movementVector = new Vector3(moveX, 0f, moveZ);

            if (Input.GetKey(KeyCode.E)) flyDirection = 1;
            else if (Input.GetKey(KeyCode.Q)) flyDirection = -1;
            else flyDirection = 0;

            if (movementVector.magnitude >= 0.1f)
            {
                // rotate the player according to the position they're moving in while adjusting for the current position of the camera:
                targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                keyRotate.Execute(actor, angle);

                movementVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized; ;

                keyMove.Execute(actor, movementVector * movementSpeed * Time.deltaTime);
            }

            if (flyDirection != 0)
            {
                keyMove.Execute(actor, Vector3.up * flyDirection * movementSpeed * Time.deltaTime);
            }
        }
    }

    //Calls Command function that spawns a bullet (projectile)
    public void Shooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canShoot == true && isLocalPlayer)
        {
            Shoot();
            EventSystem.Instance.Fire("AUDIO", "shoot"); //Play audio
            canShoot = false; // Player cannot shoot now
            StartCoroutine(Cooldown());
        }
    }

    [Command]
    void Shoot()
    {
        Debug.Log("Called;");
        GameObject bullet = GameObject.Instantiate(projectile, actor.shootingPoint.transform.position, actor.shootingPoint.transform.rotation); //Create bullet
        NetworkServer.Spawn(bullet); // Spawn bullet
    }

    // Cooldown before player can shoot again
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}
