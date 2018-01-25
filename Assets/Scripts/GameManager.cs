using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //List of players and characters' objects
    public List<GameObject> players = new List<GameObject>();
    public List<Object> objCharacters = new List<Object>();

    //constant number of players
    const int MAX_PLAYERS = 4;
    const int MAX_CHARACTERS = 4;

    //Inputs
    public float[] horizontal = new float[MAX_PLAYERS];
    public float[] vertical = new float[MAX_PLAYERS];
    public float[] arrowV = new float[MAX_PLAYERS];
    public float[] arrowH = new float[MAX_PLAYERS];
    public float[] triggers = new float[MAX_PLAYERS];
    public bool[] jump = new bool[MAX_PLAYERS];
    public bool[] cancel = new bool[MAX_PLAYERS];
    public bool[] fire1 = new bool[MAX_PLAYERS];
    public bool[] fire2 = new bool[MAX_PLAYERS];
    public bool[] start = new bool[MAX_PLAYERS];
    public bool[] select = new bool[MAX_PLAYERS];
    public bool[] R1 = new bool[MAX_PLAYERS];
    public bool[] L1 = new bool[MAX_PLAYERS];

    //Variables for the RoomMenu
        //Variables for the selection of the player
    public int[] playersIndex = new int[MAX_PLAYERS];
    int[] characterChoseByPlayer_ = new int[MAX_PLAYERS];
    bool[] canChangeChar_ = new bool[MAX_PLAYERS] { true, true, true, true};
    Vector3 characterPositon = new Vector3(-2.8f,0,3f);

        //Variables for the players' connection
    public int nPlayersConnected = 0;
    public int nPlayerReady = 0;
    public bool[] isReady = new bool[MAX_PLAYERS] { false, false, false, false };
    public bool[] isConnected = new bool[MAX_PLAYERS] {false, false, false, false };
    public bool[] wantToMove = new bool[MAX_PLAYERS] { true, true, true, true };

    //Variables InGame
    public int nPlayersCreated = 0;

    //GamePhase variables
    public enum GamePhase {Menu, RoomMenu, InGame, Pause, EndGame};
    public GamePhase currentPhase;

    // Use this for initialization
    void Start () {
        //Add the characters object to the resources data as objects
        for(int i = 0; i < MAX_CHARACTERS; i++)
        {
            objCharacters.Add(Resources.Load("Models/Character" + (i+1)));
        }

        //Set up the first GamePhase
        currentPhase = GamePhase.Menu;

        //Dont Destroy this gameobject when we change the level
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        //Call some functions depending on the currentPhase
        switch (currentPhase)
        {
            case GamePhase.Menu:
                break;
            case GamePhase.RoomMenu:
                ConnectPlayer();
                CharacterSelection();
                break;
            case GamePhase.InGame:
                InGame();
                RestartGame();
                break;
            case GamePhase.Pause:
                break;
            case GamePhase.EndGame:
                break;

        }

        //Set the inputs variables
        SetInputs();

        //Makes the cursor invisible
        Cursor.visible = false;
    }

    //Set the input from the controllers for all of the players
    void SetInputs()
    {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            horizontal[i] = Input.GetAxis("Horizontal" + (i + 1));
            vertical[i] = Input.GetAxis("Vertical" + (i + 1));
            arrowH[i] = Input.GetAxis("ArrowHorizontal" + (i + 1));
            arrowV[i] = Input.GetAxis("ArrowVertical" + (i + 1));
            jump[i] = Input.GetButtonDown("Jump" + (i + 1));
            cancel[i] = Input.GetButtonDown("Cancel" + (i + 1));
            fire1[i] = Input.GetButtonDown("Fire1" + (i + 1));
            fire2[i] = Input.GetButtonDown("Fire2" + (i + 1));
            start[i] = Input.GetButtonDown("Start" + (i + 1));
            R1[i] = Input.GetButtonDown("R1" + (i + 1));
            L1[i] = Input.GetButtonDown("L1" + (i + 1));
            select[i] = Input.GetButtonDown("Select" + (i + 1));
            triggers[i] = Input.GetAxis("TriggerR" + (i + 1));
        }
    }

    //Connect the player to the party room
    void ConnectPlayer()
    {
        for (int j = 0; j < MAX_PLAYERS; j++)
        {
            // Connect the player from the room
            if (!isConnected[j] && jump[j])
            {
                //Connect the player with the first character in the resource list
                isConnected[j] = true;
                characterChoseByPlayer_[j] = 0;
                //Set the playerIndex for the controller (Ex : 2nd controller got the 1rst player index if he connect himself first
                playersIndex[j] = nPlayersConnected;
                players.Add((GameObject)Instantiate(objCharacters[characterChoseByPlayer_[j]], (characterPositon + new Vector3(playersIndex[j] * 2, 0, 0)), Quaternion.Euler(Vector3.zero)));
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                //Have to change cause when we connect to the roomMenu we are not in the InGame level so we can't use the PlayerController script
                //players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                nPlayersConnected++;
            }
            // Disconnect the player from the room
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
            // if the player was ready then it put the player to the state not ready
            else if (isConnected[j] && cancel[j] && isReady[j])
            {
                isReady[j] = false;
                nPlayerReady--;

            }
            // if nobody is ready and someone press cancel the level switch for the menu
            else if(!isConnected[j] && cancel[j])
            {
                //return to the menu
                currentPhase = GamePhase.Menu;
                SceneManager.LoadScene("Menu");
            }
            else if(isConnected[j] && jump[j])
            {
                // if everybody is ready so the game start
                if(nPlayerReady == nPlayersConnected)
                {
                    //Launch the game
                    currentPhase = GamePhase.InGame;
                    SceneManager.LoadScene("InGame");
                }
                // if the player wasn't ready then it put the player to the state ready
                else if (!isReady[j])
                {
                    isReady[j] = true;
                    nPlayerReady++;
                }
            }

        }
    }

    //Manage the room before the game have been launched
    void CharacterSelection()
    {
        for (int j = 0; j < MAX_PLAYERS; j++)
        {
            // Change the character chose by the player and go to the next character in the resource list
            if (isConnected[j] && (arrowH[j] > 0.5 || horizontal[j] > 0.5) && canChangeChar_[j] && !isReady[j])
            {
                Destroy(players[playersIndex[j]].gameObject);
                if (characterChoseByPlayer_[j] == MAX_CHARACTERS - 1)
                {
                    characterChoseByPlayer_[j] = 0;
                }
                else
                {
                    characterChoseByPlayer_[j]++;
                }
                players[playersIndex[j]] = (GameObject)Instantiate(objCharacters[characterChoseByPlayer_[j]], new Vector3(-5 + (playersIndex[j] * 2), 1, 0), Quaternion.Euler(Vector3.zero));
                //Have to change cause when we connect to the roomMenu we are not in the InGame level so we can't use the PlayerController script
                //players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                canChangeChar_[j] = false;
            }
            // Change the character chose by the player and go to the previous character in the resource list
            else if (isConnected[j] && (arrowH[j] < -0.5 || horizontal[j] < -0.5) && canChangeChar_[j] && !isReady[j])
            {
                Destroy(players[playersIndex[j]].gameObject);
                if (characterChoseByPlayer_[j] == 0)
                {
                    characterChoseByPlayer_[j] = MAX_CHARACTERS - 1;
                }
                else
                {
                    characterChoseByPlayer_[j]--;
                }
                players[playersIndex[j]] = (GameObject)Instantiate(objCharacters[characterChoseByPlayer_[j]], new Vector3(-5 + (playersIndex[j] * 2), 1, 0), Quaternion.Euler(Vector3.zero));
                //Have to change cause when we connect to the roomMenu we are not in the InGame level so we can't use the PlayerController script
                //players[playersIndex[j]].gameObject.GetComponent<PlayerController>().numController = j;
                players[playersIndex[j]].gameObject.GetComponent<PlayerController>().enabled = false;
                canChangeChar_[j] = false;
            }

            // Wait the reset of the inputs to do an other switch
            if (arrowH[j] == 0 && horizontal[j] == 0)
            {
                canChangeChar_[j] = true;
            }
        }
    }

    void InGame()
    {
        for(int i = 0; i <MAX_PLAYERS; i++)
        {
            
            if(nPlayersCreated < nPlayersConnected)
            {
                players[playersIndex[i]] = (GameObject)Instantiate(objCharacters[characterChoseByPlayer_[i]], new Vector3(-5 + (playersIndex[i] * 2), 1, 0), Quaternion.Euler(Vector3.zero));
                players[playersIndex[i]].gameObject.GetComponent<PlayerController>().numController = i;
                nPlayersCreated++;
            }
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
