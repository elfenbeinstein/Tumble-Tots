using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerDead : MonoBehaviour
{
    public Actor actor;
    public Command keyQ, keyW, keyE, keyA, keyS, keyD;

    private void Start()
    {
        keyW = new MoveForward();
        keyA = new MoveLeft();
        keyS = new MoveBack();
        keyD = new MoveRight();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) { /*keyW.Execute(actor)*/ Move(); }
        if (Input.GetKeyDown(KeyCode.A)) { keyA.Execute(actor); }
        if (Input.GetKeyDown(KeyCode.S)) { keyS.Execute(actor); }
        if (Input.GetKeyDown(KeyCode.D)) { keyD.Execute(actor); }

        if (Input.GetKeyUp(KeyCode.W)) { keyW.Execute(actor, false); }
        if (Input.GetKeyUp(KeyCode.A)) { keyA.Execute(actor, false); }
        if (Input.GetKeyUp(KeyCode.S)) { keyS.Execute(actor, false); }
        if (Input.GetKeyUp(KeyCode.D)) { keyD.Execute(actor, false); }
    }

    void Move()
    {

    }
}
