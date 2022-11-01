using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformT : MonoBehaviour
{
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Vector3 direction;
    bool up;

    void Start()
    {
        up = true;
    }

    void Update()
    {
        if (up)
        {
            gameObject.transform.position += direction / 1000;
            if (Vector3.Distance(gameObject.transform.position, pos2.position) <= 0.3f)
            {
                up = false;
            }
        }
        else
        {
            gameObject.transform.position -= direction / 1000;
            if (Vector3.Distance(gameObject.transform.position, pos1.position) <= 0.3f)
            {
                up = true;
            }
        }
    }
}
