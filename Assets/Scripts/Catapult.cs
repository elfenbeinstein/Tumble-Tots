using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private Transform target;
    public  Vector3 direction;
    private Animator anim;
    private bool canCatapult;
    private List<Actor> players;
    private CommandMovement movePlayer;

    private void Start()
    {
        canCatapult = true;
        players = new List<Actor>();
        movePlayer = new MoveActor();
        anim = GetComponent<Animator>();
        direction = target.position - gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>() != null && canCatapult)
        {
            // missing (probably) distinction between whether player is alive or not
            players.Add(other.gameObject.GetComponent<Actor>());
            StartCoroutine(WaitTilUp());
            //Debug.Log("player on catapult");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>() != null)
        {
            // missing (probably) distinction between whether player is alive or not
            players.Remove(other.gameObject.GetComponent<Actor>());
        }
    }

    IEnumerator WaitTilUp()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("PlayerOnPlatform");
        canCatapult = false;
        for (int i = players.Count-1; i >= 0; i--)
        {
            // add force to players;
            movePlayer.Execute(players[i], target.position * force * Time.deltaTime);
        }
    }

    // called from animation
    public void CatapultOver()
    {
        canCatapult = true;
    }
}
