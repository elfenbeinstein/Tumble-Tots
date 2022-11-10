using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private float riseTime;
    [SerializeField] private float riseAmount;
    private float timeCounter;

    private void Start()
    {
        timeCounter = 0;
        riseAmount /= 100;
    }

    void Update()
    {
        timeCounter += Time.deltaTime;

        if (timeCounter >= riseTime/100)
        {
            RaiseLava();
            timeCounter = 0;
        }
    }

    private void RaiseLava()
    {
        gameObject.transform.position += new Vector3(0, riseAmount, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null)
        {
            string ID = other.gameObject.GetComponent<InputHandlerAlive>().playerID;
            EventSystem.Instance.Fire(ID, "TouchedLava", other.gameObject.GetComponent<Actor>());
            EventSystem.Instance.Fire("AUDIO", "death");
        }
    }
}
