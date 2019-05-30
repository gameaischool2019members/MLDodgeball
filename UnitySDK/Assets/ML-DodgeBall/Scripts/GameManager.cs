using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public DodgeBall dodgeBall;
    public AgentDodgeBallPlayer playerRed, playerBlue;

    public void ResetGame()
    {
        Debug.Log("Reset Game");

        playerRed?.Done();
        playerBlue?.Done();

        dodgeBall.transform.localPosition = new Vector3(0, 0.75f, 0);
        dodgeBall.rb.velocity = Vector3.zero;
    }
}

