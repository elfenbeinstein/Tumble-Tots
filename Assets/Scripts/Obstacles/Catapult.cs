using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catapult Obstacle in third level - survival (called Tabea_test in Unity)
/// catapults player if they step into trigger zone
/// </summary>

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
            // this whole throwCount was a workaround that I never got around to changing because it worked and there were bigger fish to fry
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
            // Add player to list of players to be catapulted
            players.Add(other.gameObject.GetComponent<Actor>());
            if (canCatapult)
                StartCoroutine(WaitTilUp());
            else
            {
                // if players enter while it's going down, it will catapult again once it's down
                playerOnCWhileGoingDown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>() != null)
        {
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
    }

    // called from animation
    public void CatapultOver()
    {
        canCatapult = true;
        if (playerOnCWhileGoingDown)
            StartCoroutine(WaitTilUp());
    }
}
