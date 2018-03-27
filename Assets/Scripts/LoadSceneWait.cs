using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneWait : MonoBehaviour {
    
    public string nextSceneName;
    public UISprite loadingBar;
    public TweenAlpha tpTransicion;
    AsyncOperation asyncLoad;
    bool transitionEndBegin = false;
    public bool quickLoad = true;

    void Start()
    {
        print("variables " + VariablesGlobales.nextScene);
        print("scene " + SceneManager.GetActiveScene().name);
        if (VariablesGlobales.nextScene == "")
        {
            VariablesGlobales.nextScene = "Splash";
            nextScene();
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "LoadScene")
                tpTransicion.PlayForward();
            else
                nextScene();
        }

    }

    public void loadLoadScene()
    {
        print("load load");
        if(!quickLoad)
            SceneManager.LoadScene("LoadScene");
        else
            SceneManager.LoadScene(VariablesGlobales.nextScene);
    }

    public void nextScene()
    {
        print("next scene");
        nextSceneName = VariablesGlobales.nextScene;
        StartCoroutine(loadNextScene());
    }

    IEnumerator loadNextScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;
        print(asyncLoad.progress);
        //loadingBar.width = 0;
        while (!asyncLoad.isDone)
        {
            //print(asyncLoad.progress);
            //int currentLoad = (loadingBar.width > Mathf.CeilToInt(1220 * asyncLoad.progress))? loadingBar.width: Mathf.CeilToInt(1220 * asyncLoad.progress);
            //if (loadingBar != null)
            //    loadingBar.width = currentLoad;
            print(asyncLoad.progress);
            if (asyncLoad.progress >= 0.9f && !transitionEndBegin)
            {
                transitionEndBegin = true;
            //    loadingBar.width = 1220;
                if (tpTransicion != null)
                    tpTransicion.PlayForward();
            }
            yield return null;
        }
        //tpTransicion.PlayForward();
    }

    public void transitionEnded()
    {
        asyncLoad.allowSceneActivation = true;
        print(asyncLoad.progress);
    }
}


