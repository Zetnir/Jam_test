using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour {
    public Image imgPourcent;
    public Text textPourcent;

    Scene currentScene;
    AsyncOperation synchScene;
	// Use this for initialization
	void Start () {
        currentScene = SceneManager.GetActiveScene();
        synchScene = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
    }
	
	// Update is called once per frame
	void Update () {
        if(textPourcent)
        {
            textPourcent.text = (synchScene.progress * 100 + 10).ToString() + "%";
        }

		if(imgPourcent)
        {
            imgPourcent.fillAmount = synchScene.progress + 0.1f;
        }
	}
}
