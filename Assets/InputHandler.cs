using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    bool isAlive;
    CommandMovement keySpace, keyShift;
    void Start()
    {
        isAlive = true;

        keySpace = new Jumping();
        keyShift = new Dashing();
    }

    void Update()
    {
        if (isAlive) { HandleInput(); }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
            keySpace.Execute();
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            keyShift.Execute();

    }
}
