using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameManager gameManager;
    public List<GameObject> Players = new List<GameObject>();
    public Vector3 playersPosition = Vector3.zero;
    public float cameraDelay = 2f;
    public float distancePlayers;
    Vector3 camPos;
    public int nPlayers;

    // Use this for initialization
    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        switch(gameManager.currentPhase)
        {
            case GameManager.GamePhase.Menu:
                break;
            case GameManager.GamePhase.RoomMenu:
                RoomMenu();
                break;
            case GameManager.GamePhase.InGame:
                InGame();
                break;
            case GameManager.GamePhase.Pause:
                break;
            case GameManager.GamePhase.EndGame:
                break;
        }




    }

    void RoomMenu()
    {
            nPlayers = gameManager.nPlayersConnected;
            Players = gameManager.players;
    }

    void InGame()
    {
            playersPosition = Vector3.zero;
            distancePlayers = 0f;
            for (int i = 0; i < nPlayers; i++)
            {
                playersPosition += Players[i].transform.position;
                if (nPlayers > 1)
                    distancePlayers += Players[i].transform.position.magnitude;
                if (Players[i].GetComponent<PlayerController>().isDead)
                {
                    Players.Remove(Players[i]);
                    nPlayers--;
                }

            }
            playersPosition /= nPlayers;
            distancePlayers /= nPlayers;
            if (nPlayers > 0)
            {
                camPos = new Vector3(0, 4 + (0.5f * distancePlayers), -5f - (0.5f * distancePlayers));
                transform.position = Vector3.Lerp(transform.position, playersPosition + camPos, cameraDelay * Time.deltaTime);
            }
            else
            {
                camPos = new Vector3(0, 5, -5f);
                transform.position = Vector3.Lerp(transform.position, camPos, cameraDelay * Time.deltaTime);

            }
    }
}
