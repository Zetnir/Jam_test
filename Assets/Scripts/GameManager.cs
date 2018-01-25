using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public List<GameObject> players = new List<GameObject>();
    public List<Object> objCharacters = new List<Object>();

    const int MAX_PLAYERS = 4;
    const int MAX_CHARACTERS = 4;

    public float[] horizontal = new float[MAX_PLAYERS];
    public float[] vertical = new float[MAX_PLAYERS];
    public float[] arrowH = new float[MAX_PLAYERS];
    public float[] triggerR = new float[MAX_PLAYERS];
    //float[] arrowV = new float[MAX_PLAYERS];
    public bool[] jump = new bool[MAX_PLAYERS];
    public bool[] cancel = new bool[MAX_PLAYERS];
    public bool[] fire1 = new bool[MAX_PLAYERS];
    public bool[] fire2 = new bool[MAX_PLAYERS];
    public bool[] start = new bool[MAX_PLAYERS];
    public bool[] special = new bool[MAX_PLAYERS];

    public int[] playersIndex = new int[MAX_PLAYERS];
    int[] characterChoseByPlayer = new int[MAX_PLAYERS];
    bool[] canChangeChar = new bool[MAX_PLAYERS] { true, true, true, true};

    public int nPlayersConnected = 0;
    public int nPlayerReady = 0;
    public bool[] isReady = new bool[MAX_PLAYERS] { false, false, false, false };
    public bool[] isConnected = new bool[MAX_PLAYERS] {false, false, false, false };
    public bool[] wantToMove = new bool[MAX_PLAYERS] { true, true, true, true };

    public enum GamePhase {Menu, RoomMenu, InGame};
    public GamePhase currentPhase;
    // Use this for initialization
    void Start () {
        for(int i = 0; i < MAX_CHARACTERS; i++)
        {
            objCharacters.Add(Resources.Load("Models/Character" + (i+1)));
        }
        currentPhase = GamePhase.RoomMenu;
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        switch (currentPhase)
        {
            case GamePhase.Menu:
                break;
            case GamePhase.RoomMenu:
                ConnectionPlayer();
                RoomMenu();
                break;
            case GamePhase.InGame:
                InGameManager();
                RestartGame();
                break;
        }
        InputPlayers();
        Cursor.visible = false;
    }

    //Set the input from the controllers for all of the players
    void InputPlayers()
    {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            horizontal[i] = Input.GetAxis("Horizontal" + (i + 1));
            vertical[i] = Input.GetAxis("Vertical" + (i + 1));
            arrowH[i] = Input.GetAxis("ArrowHorizontal" + (i + 1));
            jump[i] = Input.GetButtonDown("Jump" + (i + 1));
            cancel[i] = Input.GetButtonDown("Cancel" + (i + 1));
            fire1[i] = Input.GetButtonDown("Fire1" + (i + 1));
            fire2[i] = Input.GetButtonDown("Fire2" + (i + 1));
            start[i] = Input.GetButtonDown("Start" + (i + 1));
            special[i] = Input.GetButtonDown("Special" + (i + 1));
            triggerR[i] = Input.GetAxis("TriggerR" + (i + 1));

        }
    }

    //Connect the player to the party room
    void ConnectionPlayer()
    {
        for (int j = 0; j < MAX_PLAYERS; j++)
        {
            // Connect or disconnect the player to the room
            if (!isConnected[j] && jump[j])
            {
                isConnected[j] = true;
                characterChoseByPlayer[j] = 0;
                playersIndex[j] = nPlayersConnected;
                players.Add((GameObject)Instantiate(objCharacters[characterChoseByPlayer[j]], new Vector3(-5 + (playersIndex[j] * 1.6f), 1, 0), Quaternion.Euler(Vector3.zero)));
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                nPlayersConnected++;


            }
            else if (isConnected[j] && cancel[j] && !isReady[j])
            {
                isConnected[j] = false;
                Destroy(players[playersIndex[j]].gameObject);
                players.Remove(players[playersIndex[j]]);
                for(int k = j+1; k < MAX_PLAYERS; k++)
                {
                    if(playersIndex[k] > 0)
                        playersIndex[k]--;
                }
                nPlayersConnected--;
            }
            else if (isConnected[j] && cancel[j] && isReady[j])
            {
                isReady[j] = false;
                nPlayerReady--;

            }
            else if(!isConnected[j] && cancel[j])
            {
                currentPhase = GamePhase.Menu;
                //return to the menu
            }
            else if(isConnected[j] && jump[j])
            {
                if(nPlayerReady == nPlayersConnected)
                {
                    currentPhase = GamePhase.InGame;
                }
                else if(!isReady[j])
                {
                    isReady[j] = true;
                    nPlayerReady++;
                }
            }

        }
    }

    //Manage the room before the game have been launched
    void RoomMenu()
    {
        for (int j = 0; j < MAX_PLAYERS; j++)
        {
            // Enable the player to chose his character
            if (isConnected[j] && (arrowH[j] > 0.5 || horizontal[j] > 0.5) && canChangeChar[j] && !isReady[j])
            {
                Destroy(players[playersIndex[j]].gameObject);
                if (characterChoseByPlayer[j] == MAX_CHARACTERS - 1)
                {
                    characterChoseByPlayer[j] = 0;
                }
                else
                {
                    characterChoseByPlayer[j]++;
                }
                players[playersIndex[j]] = (GameObject)Instantiate(objCharacters[characterChoseByPlayer[j]], new Vector3(-5 + (playersIndex[j] * 1.6f), 1, 0), Quaternion.Euler(Vector3.zero));
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                canChangeChar[j] = false;
            }
            else if (isConnected[j] && (arrowH[j] < -0.5 || horizontal[j] < -0.5) && canChangeChar[j] && !isReady[j])
            {
                Destroy(players[playersIndex[j]].gameObject);
                if (characterChoseByPlayer[j] == 0)
                {
                    characterChoseByPlayer[j] = MAX_CHARACTERS - 1;
                }
                else
                {
                    characterChoseByPlayer[j]--;
                }
                players[playersIndex[j]] = (GameObject)Instantiate(objCharacters[characterChoseByPlayer[j]], new Vector3(-5 + (playersIndex[j] * 1.6f), 1, 0), Quaternion.Euler(Vector3.zero));
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                canChangeChar[j] = false;
            }

            if (arrowH[j] == 0 && horizontal[j] == 0)
            {
                canChangeChar[j] = true;
            }
        }
    }

    void InGameManager()
    {
        for(int i = 0; i <MAX_PLAYERS; i++)
        {
            if(wantToMove[i])
            {
                players[playersIndex[i]].gameObject.GetComponent<PlayerController>().enabled = true;
                wantToMove[i] = false;
            }
        }

    }

    void RestartGame()
    {
        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            if (start[i])
            {
                SceneManager.LoadScene("Scene_Ludo");
                Destroy(gameObject);
            }

        }
    }

}
