using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, duration, force;
    private CommandMovement movePlayer;
    public Actor actor;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Debug.Log("Player hit");
            //Push Player back
            actor = collision.gameObject.GetComponent<Actor>();
            Vector3 pushDirection = collision.transform.position - transform.position;
            pushDirection.Normalize();
            collision.gameObject.GetComponent<InputHandlerAlive>().Push(pushDirection * force * Time.deltaTime);
        }
        Destroy(gameObject);
    }
}
