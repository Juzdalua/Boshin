using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnterController : MonoBehaviour
{
    public AudioClip bgmClip;
    public string villigeName;
    public GameObject villigeUI;

    private CanvasGroup canvasGroup;
    private string currentVilligeName;
    private Coroutine SceneEnterCanvasCoroutine;
    public float villigeNameRemainTime = 2f;
    public string sceneName;

    void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        canvasGroup.alpha = 0f;
        villigeUI.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = villigeName;
        villigeUI.SetActive(false);
    }

    void Update()
    {
        if (canvasGroup.alpha >= 1)
        {
            StartCoroutine(HideVilligeNameCoroutine());
        }

        if (!PlayerManager.Instance.GetActivePlayer().GetComponent<PlayerInputManager>().canMove)
        {
            villigeUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DataManager.Instance.sceneName = sceneName;
            if (bgmClip != null)
            {
                SoundManager.Instance.BgmSoundPlay(bgmClip.name, bgmClip);
            }
            currentVilligeName = GameManager.Instance.GetCurrentVilligeName();
            if (currentVilligeName != villigeName)
            {
                villigeUI.SetActive(true);
                GameManager.Instance.SetCurrentVilligeName(villigeName);
                SceneEnterCanvasCoroutine = StartCoroutine(ShowVilligeNameCoroutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneEnterCanvasCoroutine != null)
            {
                StopCoroutine(SceneEnterCanvasCoroutine);
                canvasGroup.alpha = 0;
                villigeUI.SetActive(false);
            }
        }
    }

    private IEnumerator ShowVilligeNameCoroutine()
    {
        yield return null;

        float timer = 0f;
        while (timer < villigeNameRemainTime)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, timer / villigeNameRemainTime);
            if (canvasGroup.alpha >= 1f)
            {
                yield break;
            }
        }
    }

    private IEnumerator HideVilligeNameCoroutine()
    {
        yield return new WaitForSeconds(2f);

        float timer = 0f;
        while (timer < 5f)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, timer / 10f);
            if (canvasGroup.alpha <= 0f)
            {
                yield break;
            }
        }
        yield return null;
    }
}

