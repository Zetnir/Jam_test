using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour {
    public Image imgPourcent;
    public Text textPourcent;
    public GameManager gameManager;
    public int buildIndex;

    Scene currentScene;
    AsyncOperation synchScene;
     
	// Use this for initialization
	void Start () {
        currentScene = SceneManager.GetActiveScene();
        switch (gameManager.currentPhase)
        {
            case GameManager.GamePhase.Menu:
                buildIndex = 0;
                break;
            case GameManager.GamePhase.RoomMenu:
                buildIndex = 1;
                break;
            case GameManager.GamePhase.InGame:
                buildIndex = 2;
                break;
            case GameManager.GamePhase.Pause:
                buildIndex = 3;
                break;
            case GameManager.GamePhase.EndGame:
                buildIndex = 4;
                break;
        }

        if(imgPourcent)
        {
            synchScene = SceneManager.LoadSceneAsync(buildIndex);
        }
        else
        {
            synchScene = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {

        if (textPourcent)
        {
            textPourcent.text = (synchScene.progress * 100 + 10).ToString() + "%";
        }

		if(imgPourcent)
        {
            imgPourcent.fillAmount = synchScene.progress + 0.1f;
        }
	}
}
