using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, duration, force, pushDuration;

    private void Start()
    {
        //Destroy projectile after a set duration
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if other object is a player
        if (collision.gameObject.GetComponent<InputHandlerAlive>() != null)
        {
            //Push Player back
            Vector3 pushDirection = collision.transform.position - transform.position; // Vector for the direction the player gets pushed
            pushDirection.Normalize();

            collision.gameObject.GetComponent<InputHandlerAlive>().Push(pushDirection * force * Time.deltaTime, pushDuration); // Calls function in the InputHandlerAlive script that pushes player back

            EventSystem.Instance.Fire("AUDIO", "ouch"); // Plays audio
        }
        
        //Destroy object when it collides with another object
        if (collision.gameObject.GetComponent<InputHandlerDead>() == null)
            Destroy(gameObject);
    }
}
