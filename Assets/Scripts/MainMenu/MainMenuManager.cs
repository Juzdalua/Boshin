using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public Canvas background;
    public GameObject mainFloor;
    public GameObject door;
    public Button startButton;
    public Slider loadingBar;

    [Header("Audio")]
    public AudioSource bgmAudioSource;
    public AudioClip[] bgms;
    public AudioClip bgm;
    public AudioSource sfxAudioSource;
    public AudioClip doorUp;
    public AudioClip enterSound;

    [Header("Move forward variable")]
    private int flag = 0;
    private int nextFlag = 1;
    private Vector3 stopPosition;
    Coroutine updateLoadingBarCoroutine;
    Coroutine infiniteFloorCoroutine;
    public Transform infinityObjectTransform;

    private bool isMove = false;
    private bool isStop = false;
    private bool isEnter = false;
    private float stopTIme;

    private int startClickStage = 0;
    private bool isAvailableGame = false;

    void Awake()
    {
        SetBackgroundByTime();
        door.SetActive(false);
        StartButtonActive(false);
    }

    void Start()
    {
        if (bgm != null && bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }
        flag = 0;
        nextFlag = 1;
        isMove = false;

        if (updateLoadingBarCoroutine != null) StopCoroutine(updateLoadingBarCoroutine);
        updateLoadingBarCoroutine = StartCoroutine(UpdateLoadingBarCoroutine());
    }

    void Update()
    {
        if (!isMove)
        {
            Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * 2);
            ScaleupBackground();

            flag = (int)Time.timeSinceLevelLoad / 2;
            if (flag == nextFlag)
            {
                if (infiniteFloorCoroutine != null) StopCoroutine(infiniteFloorCoroutine);
                infiniteFloorCoroutine = StartCoroutine(InstantiateInfinityFloorCoroutine());
                nextFlag++;
            }
        }

        if (Input.GetMouseButtonDown(0) && isAvailableGame)
        {
            isMove = true;
            door.SetActive(true);
            if (!isStop)
            {
                StartCoroutine(SetDoorCoroutine());
                isStop = true;
                stopTIme = Time.timeSinceLevelLoad;
                // SoundManager.Instance.SFXPlay("DoorUp", doorUp);
                SfxOneShot("DoorUp", doorUp);
                StartCoroutine(WaitForSeconds(1f));
            }
        }
    }

    private IEnumerator WaitForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        yield break;
    }

    private void SetBackgroundByTime()
    {
        int hour = int.Parse(System.DateTime.Now.ToString("HH"));

        if (hour >= 5 && hour < 11)
        {
            background.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("MainMenu/Images/morning");
            bgm = Array.Find(bgms, item => item.name == "00bgm-morning");
        }
        else if (hour >= 11 && hour < 18)
        {
            background.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("MainMenu/Images/noon");
            bgm = Array.Find(bgms, item => item.name == "00bgm-noon");
        }
        else
        {
            background.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("MainMenu/Images/night");
            bgm = Array.Find(bgms, item => item.name == "00bgm-night");
        }
        bgmAudioSource.clip = bgm;
    }

    private IEnumerator InstantiateInfinityFloorCoroutine()
    {
        float startTime = Time.timeSinceLevelLoad;
        float cycleTime = 0.8f;
        GameObject newFloor = Instantiate(mainFloor, mainFloor.transform.position + (Vector3.forward * Time.timeSinceLevelLoad * 3) + Vector3.up * 1.8f, Quaternion.identity, infinityObjectTransform);

        while (Time.timeSinceLevelLoad < startTime + cycleTime)
        {
            newFloor.transform.position -= Vector3.up * Time.deltaTime * 2;
            yield return null;
        }
        stopPosition = newFloor.transform.position;
        // Destroy(newFloor, 10f);
        yield return new WaitForSeconds(1f);
        yield return null;
    }

    private void ScaleupBackground()
    {
        background.GetComponentInChildren<Image>().transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 0.01f;
    }

    private IEnumerator SetDoorCoroutine()
    {
        Vector3 doorPosition = Camera.main.transform.position + Vector3.forward * 11 - Vector3.up * 3;

        GameObject newDoor = Instantiate(door, doorPosition, Quaternion.identity);

        while (newDoor.transform.position.y < -0.5f)
        {
            newDoor.transform.position += Vector3.up * Time.deltaTime * 5;
            yield return null;
        }
        yield return null;
    }

    public void StartGame()
    {
        if (!(Time.timeSinceLevelLoad > stopTIme + 1f)) return;

        if (startClickStage == 0)
        {
            startClickStage++;
        }
        else if (startClickStage == 1)
        {
            StartCoroutine(ZoominCamera());
            ClickStartGameButton();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator ZoominCamera()
    {
        if (!isEnter)
        {
            // SoundManager.Instance.SFXPlay("StartGame", enterSound);
            SfxOneShot("StartGame", enterSound);
            isEnter = true;
        }
        float startTime = Time.timeSinceLevelLoad;
        float cycleTime = 0.8f;

        while (Time.timeSinceLevelLoad < startTime + cycleTime)
        {
            Camera.main.transform.position += Vector3.forward * 10 * Time.deltaTime;
            yield return null;
        }
    }

    // todo - Start New Game with false arg
    private void ClickStartGameButton(bool isLoad = true)
    {
        string nextSceneName = "03BoshinWind";

        if (isLoad)
        {
            if (File.Exists($"{Application.persistentDataPath}/save"))
            {
                string saveData = File.ReadAllText($"{Application.persistentDataPath}/save");

                SaveObject saveObject = new SaveObject();
                saveObject = JsonUtility.FromJson<SaveObject>(saveData);
                string sceneName = saveObject.sceneName;

                if (sceneName != null && sceneName.Trim() != "")
                    nextSceneName = sceneName;
            }
        }

        StartCoroutine(StartGameCoroutine(nextSceneName));
    }

    private IEnumerator StartGameCoroutine(string nextScene)
    {
        yield return new WaitForSeconds(0.8f);

        string[] nextScenes = { "02BoshinMainScene", nextScene };
        LoadingSceneAdditive.LoadSceneAdditive(nextScenes);
        yield return null;
    }

    private IEnumerator UpdateLoadingBarCoroutine()
    {
        float timer = 0f;
        while (timer < 2f)
        {
            yield return null;
            timer += Time.deltaTime;
            loadingBar.value = Mathf.Lerp(0, 1, timer);
        }
        if (loadingBar.value == 1)
        {
            StartButtonActive(true);
            yield break;
        }
    }

    private void StartButtonActive(bool isActive)
    {
        if (isActive)
        {
            loadingBar.GetComponent<CanvasGroup>().alpha = 0;
            startButton.enabled = true;
            startButton.GetComponent<CanvasGroup>().alpha = 1;
            isAvailableGame = true;
        }
        else
        {
            startButton.enabled = false;
            startButton.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    void SfxOneShot(string sfxName, AudioClip clip)
    {
        GameObject sound = new GameObject(sfxName + "Sound");
        AudioSource audioSource = sound.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 0.5f;
        audioSource.loop = false;
        audioSource.Play();

        GameObject.Destroy(sound, clip.length);
    }
}
