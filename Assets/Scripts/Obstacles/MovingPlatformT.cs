using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformT : MonoBehaviour
{
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Vector3 direction;
    bool up;

    // stuff to move player:
    bool carriesPlayer;
    List<Actor> players;
    private CommandMovement movePlayer;

    void Start()
    {
        up = true;
        carriesPlayer = false;
        players = new List<Actor>();
        movePlayer = new MoveActor();
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

            if (carriesPlayer)
            {
                for (int i = players.Count - 1; i >= 0; i--)
                {
                    movePlayer.Execute(players[i], (direction / 1000));
                }
            }
        }
        else
        {
            gameObject.transform.position -= direction / 1000;
            if (Vector3.Distance(gameObject.transform.position, pos1.position) <= 0.3f)
            {
                up = true;
            }

            if (carriesPlayer)
            {
                for (int i = players.Count - 1; i >= 0; i--)
                {
                    movePlayer.Execute(players[i], (-1 * direction / 1000));
                }
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Actor>() != null)
        {
            players.Add(other.GetComponent<Actor>());
            carriesPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Actor>() != null)
        {
            players.Remove(other.GetComponent<Actor>());
        }
        if (players.Count == 0) carriesPlayer = false;
    }
}
