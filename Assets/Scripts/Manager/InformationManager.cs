using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InformationButtonType
{
    Close,
    Next,
    Previous
}

public class InformationManager : Singleton<InformationManager>
{
    [SerializeField] private PlayerInputManager[] _inputManagers;
    [SerializeField] private PlayerInputManager _inputManager;
    [SerializeField] private GameObject showInfomationObject;
    private int userInfomationStage = 0;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button closeButton;
    private string[] currentTexts = null;
    private bool isShowUI = false;
    [SerializeField] private AudioClip ButtonClickAudioClip;

    void Awake()
    {
        ShowCanvas(false);
        closeButton.gameObject.SetActive(false);

        SetPlayerInputInActive();
    }

    void Update()
    {
        if (isShowUI)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInfomation();
            }
        }

        if (showInfomationObject.activeInHierarchy)
        {
            ShowButton();
        }
    }

    public void ShowCanvas(bool isShow)
    {
        showInfomationObject.SetActive(isShow);
    }

    public void OpenInfomationText(string[] informationTexts, TriggerEventController triggerEvent = null)
    {
        if (currentTexts == null || currentTexts.Length == 0)
        {
            currentTexts = informationTexts;
            userInfomationStage = 0;
            SetPlayerInputInActive(triggerEvent);
        }

        if (!isShowUI)
        {
            isShowUI = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            showInfomationObject.GetComponentInChildren<TextMeshProUGUI>().text = currentTexts[userInfomationStage];
            ShowCanvas(true);
        }
    }

    public void UpdateInfomationText()
    {
        Cursor.lockState = CursorLockMode.None;
        showInfomationObject.GetComponentInChildren<TextMeshProUGUI>().text = currentTexts[userInfomationStage];
    }

    public void CloseInfomation()
    {
        ShowCanvas(false);
        userInfomationStage = 0;
        currentTexts = null;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        isShowUI = false;
        _inputManager.canMove = true;
        OnButtonClickSound();
    }

    public void ClickNextButton()
    {
        userInfomationStage++;
        UpdateInfomationText();
        OnButtonClickSound();
    }

    public void ClickPreviousButton()
    {
        userInfomationStage--;
        UpdateInfomationText();
        OnButtonClickSound();
    }

    public void ShowButton()
    {

        if (userInfomationStage == 0 && currentTexts.Length == 1)
        {
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
        }
        else if (userInfomationStage == 0 && currentTexts.Length != 1)
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(false);
        }
        else if (userInfomationStage != 0 && currentTexts.Length != 1 && userInfomationStage != currentTexts.Length - 1)
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);
        }
        else if (userInfomationStage != 0 && currentTexts.Length != 1 && userInfomationStage == currentTexts.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
        }
    }

    private void OnButtonClickSound()
    {
        if (ButtonClickAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("InfomationButtonClickSound", ButtonClickAudioClip);
        }
    }

    private void SetPlayerInputInActive(TriggerEventController triggerEvent = null)
    {
        for (int i = 0; i < _inputManagers.Length; i++)
        {
            if (_inputManagers[i].gameObject.activeInHierarchy)
            {
                _inputManager = _inputManagers[i];
                break;
            }
        }
        if (triggerEvent != null)
        {
            // GameObject.Destroy(triggerEvent);
            EventManager.Instance.SetDoneEventById(triggerEvent.Id);
        }
    }

    public bool IsShowUI(){
        return isShowUI;
    }
}
