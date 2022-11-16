using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseEnabler : MonoBehaviour
{
    private void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Win_Screen") { Cursor.visible = true; }
    }
}
