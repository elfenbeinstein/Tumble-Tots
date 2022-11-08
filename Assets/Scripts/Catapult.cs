using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    [SerializeField] private int throwDuration;
    private int throwCount;
    [SerializeField] private float force;
    [SerializeField] private Transform target;
    public  Vector3 direction;
    private Animator anim;
    private bool canCatapult;
    private List<Actor> players;
    private CommandMovement movePlayer;
    private bool catapultsPlayers;
    private bool playerOnCWhileGoingDown;

    private void Start()
    {
        throwCount = 0;
        canCatapult = true;
        catapultsPlayers = false;
        playerOnCWhileGoingDown = false;
        players = new List<Actor>();
        movePlayer = new MoveActor();
        anim = GetComponent<Animator>();
        direction = target.position - gameObject.transform.position;
    }

    private void Update()
    {
        if (catapultsPlayers)
        {
            throwCount += 1;
            if (throwCount < throwDuration)
            {
                for (int i = players.Count - 1; i >= 0; i--)
                {
                    movePlayer.Execute(players[i], direction * force * Time.deltaTime);
                }
            }
            else
            {
                catapultsPlayers = false;
                throwCount = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>() != null)
        {
            // missing (probably) distinction between whether player is alive or not
            players.Add(other.gameObject.GetComponent<Actor>());
            if (canCatapult)
                StartCoroutine(WaitTilUp());
            else
            {
                playerOnCWhileGoingDown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>() != null)
        {
            // missing (probably) distinction between whether player is alive or not
            players.Remove(other.gameObject.GetComponent<Actor>());

            if (players.Count == 0)
            {
                playerOnCWhileGoingDown = false;
            }
        }
    }

    IEnumerator WaitTilUp()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("PlayerOnPlatform");
        canCatapult = false;
        yield return new WaitForSeconds(0.2f);
        catapultsPlayers = true;
        playerOnCWhileGoingDown = false;
        /* -- moved to the Update method for testing
        for (int i = players.Count-1; i >= 0; i--)
        {
            //Debug.Log("catapult " + players[i].gameObject);
            // add force to players;
            movePlayer.Execute(players[i], direction * force * 10 * Time.deltaTime);
            //players[i].cc.Move(direction * force * 10 * Time.deltaTime);
        }
        */
    }

    // called from animation
    public void CatapultOver()
    {
        canCatapult = true;
        if (playerOnCWhileGoingDown)
            StartCoroutine(WaitTilUp());
    }
}
