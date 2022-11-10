using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class gameManagerScript : MonoBehaviour
{
    [SerializeField] int roundNumber; //For choosing the level
    [SerializeField] int numberOfQualifiers; //Number of people allowed to proceed
    //[SerializeField] Transform 

    [SerializeField] string[] levelNames; //For random level pool

    
    public void ChooseRandomLevel()
    {

    }

    public void KillPlayer()
    {

    }

    public void InitializeRound()
    {
        roundNumber++;

        switch (roundNumber)
        {
            case 1:
                numberOfQualifiers = 6;
                break;

            case 2:
                numberOfQualifiers = 4;
                break;

            case 3: numberOfQualifiers = 1;
                break;

            default:
                break;
        }

    }
}
