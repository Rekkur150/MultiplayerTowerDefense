using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChangeManager : MonoBehaviour
{

    public Animator loadingPanel;
    public static SceneChangeManager singleton;

    // Start is called before the first frame update
    void Start()
    {

        if (singleton == null)
        {
            singleton = this;
        } else if (singleton != this)
        {
            Destroy(gameObject);
        }

        if (loadingPanel == null)
            Debug.LogError("Not loading panel on scene manager", this);

        loadingPanel.SetBool("IsFading", true);

        SceneManager.activeSceneChanged += ChangedActiveScene;
        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(this);
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        loadingPanel.SetBool("IsFading", false);
        Debug.Log("Scene Changed!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadingPanel.SetBool("IsFading", true);
        Debug.Log("Scene Loaded!");
    }
}
