using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneAdditive : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI tip;
    public string[] tips;
    string selectedTip;
    private static List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    private static string[] sceneNames;

    public static void LoadSceneAdditive(string[] scenes)
    {
        SceneManager.LoadScene("00BoshinLoading");
        sceneNames = scenes;
    }

    void Start()
    {
        StartCoroutine(LoadingScreen());
        SetTips();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetTips();
        }
    }

    void SetTips()
    {
        selectedTip = tips[Random.Range(0, tips.Length - 1)];
        tip.text = selectedTip;
    }

    public IEnumerator LoadingScreen()
    {
        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneNames[0]));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneNames[1], LoadSceneMode.Additive));

        float totalProgress = 0f;

        for (int i = 0; i < scenesToLoad.Count; i++)
        // for (int i = scenesToLoad.Count - 1; i <= 0; i--)
        {
            while (!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                progressBar.value = 1f - totalProgress / (float)scenesToLoad.Count;
                yield return null;
            }
        }
    }
}
