using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPSurvival : MonoBehaviour
{
    [SerializeField] private float endHeight;
    [SerializeField] private Vector3 direction;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool carriesPlayer;
    private bool shouldMoveUp;
    private List<Actor> players;
    private CommandMovement movePlayer;

    private bool shouldMoveDown;

    void Start()
    {
        startPosition = gameObject.transform.position;
        carriesPlayer = false;
        shouldMoveUp = false;
        shouldMoveDown = false;
        endPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + endHeight, gameObject.transform.position.z);

        players = new List<Actor>();
        movePlayer = new MoveActor();
    }

    void Update()
    {
        if (carriesPlayer && shouldMoveUp)
        {
            gameObject.transform.position += direction / 1000;
            for (int i = players.Count - 1; i >= 0; i--)
            {
                movePlayer.Execute(players[i], direction / 1000);
            }
            if (Vector3.Distance(gameObject.transform.position, endPosition) < 0.3f)
            {
                shouldMoveUp = false;
            }
        }
        else
        {
            if (shouldMoveDown)
            {
                gameObject.transform.position -= direction / 1000;
                if (Vector3.Distance(gameObject.transform.position, startPosition) <= 0.1f)
                {
                    shouldMoveDown = false;
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
            StartCoroutine(WaitForUp());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Actor>() != null)
        {
            players.Remove(other.GetComponent<Actor>());
        }
        if (players.Count == 0)
        {
            carriesPlayer = false;
            StartCoroutine(WaitForDown());
        }
    }

    IEnumerator WaitForUp()
    {
        yield return new WaitForSeconds(1);
        shouldMoveUp = true;
    }

    IEnumerator WaitForDown()
    {
        yield return new WaitForSeconds(2);
        shouldMoveDown = true;
    }
}
