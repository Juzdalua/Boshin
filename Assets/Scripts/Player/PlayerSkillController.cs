using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;
using System.Linq;

public class PlayerSkillController : MonoBehaviour
{
    int playerId;
    [SerializeField] private PlayableCharacter mainCharacter;
    [SerializeField] private GameObject skillUI;
    [SerializeField] private Transform skillPos;
    [SerializeField] private GameObject eSkillPrefeb;
    [SerializeField] private GameObject qSkillPrefeb;
    PlayerInputManager _inputManager;
    PlayerInput _input;
    Animator _animator;
    [SerializeField] private CinemachineFreeLook frontCamera;

    Transform eSkillUI;
    Transform qSkillUI;
    Coroutine eSkillCoroutine;
    Coroutine qSkillCoroutine;

    float eSkillStartTIme = 0f;
    float qSkillStartTIme = 0f;

    bool isChnnelingQ = false;

    [SerializeField] private AudioClip ESkillEffectAudioClip;
    [SerializeField] private AudioClip QSkillEffectAudioClip;
    [SerializeField] private AudioClip QSkillChargeAudioClip;
    [Range(0, 1)] [SerializeField] private float AudioVolume;


    void Start()
    {
        playerId = GetComponent<ObjectData>().GetId();
        _inputManager = GetComponent<PlayerInputManager>();
        _input = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        SetSkillUIWithCharacterSwap();
        mainCharacter = PlayerManager.Instance.GetPlayerById(GetComponent<ObjectData>().GetId());
    }

    void FixedUpdate()
    {
        if (_inputManager.eSkill)
        {
            DoESkill();
        }
        if (_inputManager.qSkill)
        {
            DoQSkill();
        }
    }

    private void DoESkill()
    {
        _inputManager.eSkill = false;
        if (Time.time > eSkillStartTIme + mainCharacter.E_cooltime || eSkillStartTIme == 0)
        {
            eSkillStartTIme = Time.time;
            mainCharacter.E_startTime = eSkillStartTIme;
            // mainCharacter.e_remainTime = eSkillStartTIme;
            _input.enabled = false;
            eSkillCoroutine = StartCoroutine(DoESkillCoroutine());

            if (playerId == 2) ChargeQGauge();
        }
    }

    private IEnumerator DoESkillCoroutine()
    {
        _animator.SetTrigger("Skill");
        _animator.SetInteger("SkillCount", 0);

        eSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 1;

        float timer = 0f;
        while (timer < mainCharacter.E_cooltime)
        {
            yield return null;
            timer += Time.deltaTime;
            eSkillUI.GetChild(1).GetComponent<Image>().fillAmount = Mathf.Lerp(0f, 1f, 1 / mainCharacter.E_cooltime * timer);
            eSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = (((float)((int)((mainCharacter.E_cooltime - timer) * 10))) / 10).ToString();
        }
        eSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
        yield break;
    }

    private IEnumerator SwapESkillCoroutine()
    {
        yield return null;

        float targetTime = mainCharacter.E_startTime + mainCharacter.E_cooltime;
        if (Time.time > targetTime)
        {
            yield break;
        }

        eSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 1;
        eSkillUI.GetChild(1).GetComponent<Image>().fillAmount = (targetTime - Time.time) / mainCharacter.E_cooltime;

        float timer = Time.time;
        while (Time.time < targetTime)
        {
            yield return null;
            timer += Time.deltaTime;
            eSkillUI.GetChild(1).GetComponent<Image>().fillAmount = Mathf.Lerp(eSkillUI.GetChild(1).GetComponent<Image>().fillAmount, 1f, 1 / (targetTime - Time.time) * Time.deltaTime);
            eSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = (((float)((int)((targetTime - Time.time) * 10))) / 10).ToString();
        }
        eSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
        yield break;
    }

    private void OnESkillSound(AnimationEvent animationEvent)
    {
        if (mainCharacter.E_audios.Length > 0)
        {
            AudioClip eSkillPlayer = mainCharacter.E_audios[Random.Range(0, mainCharacter.E_audios.Length - 1)];
            SoundManager.Instance.SFXPlay("ESkillPlayer", eSkillPlayer);
        }

        if (playerId == 1)
        {
            if (ESkillEffectAudioClip != null)
            {
                SoundManager.Instance.SFXPlay("ESkillEffect", ESkillEffectAudioClip);
            }
            GameObject instantSkill = Instantiate(eSkillPrefeb, skillPos.position, skillPos.rotation);
            Rigidbody skillRigd = instantSkill.GetComponent<Rigidbody>();
            skillRigd.velocity = skillPos.forward * 10;
        }
    }

    private void OnESkillEnd()
    {
        _input.enabled = true;
    }

    public void ChargeQGauge()
    {
        if (mainCharacter.Q_currentGauge < mainCharacter.Q_maxGauge)
        {
            mainCharacter.Q_currentGauge += mainCharacter.E_gauge;
            qSkillUI.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.Q_currentGauge / mainCharacter.Q_maxGauge;

            if (mainCharacter.Q_currentGauge >= mainCharacter.Q_maxGauge && (Time.time > qSkillStartTIme + mainCharacter.Q_cooltime || qSkillStartTIme == 0))
            {
                ReadyQSkill();
            }
        }
    }

    private void ReadyQSkill()
    {
        if (QSkillChargeAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("QSkillCharge", QSkillChargeAudioClip);
        }
        qSkillUI.GetChild(5).GetComponent<CanvasGroup>().alpha = 1;
    }

    private void DoQSkill()
    {
        _inputManager.qSkill = false;
        if (mainCharacter.Q_currentGauge < mainCharacter.Q_maxGauge)
        {
            return;
        }

        float qAnimationDealyTime = 0f;
        if (Time.time > qSkillStartTIme + mainCharacter.Q_cooltime + qAnimationDealyTime || qSkillStartTIme == 0)
        {
            qSkillStartTIme = Time.time;
            mainCharacter.Q_startTime = qSkillStartTIme;
            _input.enabled = false;
            isChnnelingQ = true;

            mainCharacter.Q_currentGauge = 0;
            qSkillUI.GetChild(1).GetComponent<Image>().fillAmount = 0f;
            qSkillUI.GetChild(5).GetComponent<CanvasGroup>().alpha = 0;

            GetComponent<PlayerInteraction>().PlayerCamera.enabled = false;

            frontCamera.enabled = true;

            if (qSkillCoroutine != null) StopCoroutine(qSkillCoroutine);
            StartCoroutine(DoQSkillCoroutine());
        }
    }

    private IEnumerator DoQSkillCoroutine()
    {
        if (mainCharacter.Q_audios.Length > 0)
        {
            AudioClip qSkillPlayer = mainCharacter.Q_audios[Random.Range(0, mainCharacter.Q_audios.Length - 1)];
            SoundManager.Instance.SFXPlay("QSkillPlayer", qSkillPlayer);
        }

        yield return new WaitForSeconds(1f);

        _animator.SetTrigger("Skill");
        _animator.SetInteger("SkillCount", 1);

        qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 1;

        float timer = 0f;
        while (timer < mainCharacter.Q_cooltime)
        {
            yield return null;
            timer += Time.deltaTime;
            qSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = (((float)((int)((mainCharacter.Q_cooltime - timer) * 10))) / 10).ToString();
            qSkillUI.GetChild(3).GetComponent<Image>().fillAmount = Mathf.Lerp(0f, 1f, 1 / mainCharacter.Q_cooltime * timer);
        }
        qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
        isChnnelingQ = false;
        _input.enabled = true;
        yield break;
    }

    private IEnumerator SwapQSkillCoroutine()
    {
        yield return null;

        float targetTime = mainCharacter.Q_startTime + mainCharacter.Q_cooltime;
        if (Time.time > targetTime)
        {
            yield break;
        }

        qSkillUI.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.Q_currentGauge / mainCharacter.Q_maxGauge;
        qSkillUI.GetChild(1).GetComponent<Image>().color = new Color32(mainCharacter.ElementColor[0], mainCharacter.ElementColor[1], mainCharacter.ElementColor[2], 255);
        qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 1;
        qSkillUI.GetChild(3).GetComponent<Image>().fillAmount = (targetTime - Time.time) / mainCharacter.Q_cooltime;

        // if (mainCharacter.q_currentGauge >= mainCharacter.q_maxGauge && (Time.time > qSkillStartTIme + mainCharacter.q_cooltime || qSkillStartTIme == 0))
        // {
        //     ReadyQSkill();
        // }

        float timer = Time.time;
        while (Time.time < targetTime)
        {
            yield return null;
            timer += Time.deltaTime;
            qSkillUI.GetChild(3).GetComponent<Image>().fillAmount = Mathf.Lerp(qSkillUI.GetChild(3).GetComponent<Image>().fillAmount, 1f, 1 / (targetTime - Time.time) * Time.deltaTime);
            qSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = (((float)((int)((targetTime - Time.time) * 10))) / 10).ToString();
        }
        qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
        yield break;
    }

    private void OnQSkillStart(AnimationEvent animationEvent)
    {
        if (QSkillEffectAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("QSkillEffect", QSkillEffectAudioClip);
        }

        frontCamera.enabled = false;
        GetComponent<PlayerInteraction>().PlayerCamera.enabled = true;
        _input.enabled = true;
    }

    private void OnQSkillEnd(AnimationEvent animationEvent)
    {
        Instantiate(qSkillPrefeb, skillPos.position + Vector3.forward * 3, skillPos.rotation);
    }

    public bool GetIsChannelingQ()
    {
        return isChnnelingQ;
    }

    public void SetSkillUIWithCharacterSwap()
    {
        mainCharacter = PlayerManager.Instance.GetPlayerById(playerId);
        if (mainCharacter != null)
        {
            // E Skill UI
            eSkillUI = skillUI.transform.GetChild(0);
            if (mainCharacter.E_startTime == 0 || mainCharacter.E_startTime + mainCharacter.E_cooltime < Time.time)
            {
                // E Cooltime image
                eSkillUI.GetChild(1).GetComponent<Image>().fillAmount = 1f;

                // E Cooltime sec
                eSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = mainCharacter.E_cooltime.ToString();
                eSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                if (eSkillCoroutine != null) StopCoroutine(eSkillCoroutine);
                StartCoroutine(SwapESkillCoroutine());
            }

            // Q Skill UI
            qSkillUI = skillUI.transform.GetChild(1);

            if (mainCharacter.Q_startTime == 0 || mainCharacter.Q_startTime + mainCharacter.Q_cooltime < Time.time)
            {
                // Q Cooltime image
                qSkillUI.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.Q_currentGauge / mainCharacter.Q_maxGauge;
                qSkillUI.GetChild(1).GetComponent<Image>().color = new Color32(mainCharacter.ElementColor[0], mainCharacter.ElementColor[1], mainCharacter.ElementColor[2], 255);

                // Q Cooltime sec
                qSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = mainCharacter.E_cooltime.ToString();
                qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
                qSkillUI.GetChild(5).GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                if (qSkillCoroutine != null) StopCoroutine(qSkillCoroutine);
                StartCoroutine(SwapQSkillCoroutine());
            }

            // mainCharacter.q_currentGauge = 0;
            // qSkillUI.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.q_currentGauge;
            // qSkillUI.GetChild(1).GetComponent<Image>().color = new Color32(mainCharacter.elementColor[0], mainCharacter.elementColor[1], mainCharacter.elementColor[2], 255);
            // qSkillUI.GetChild(2).GetComponent<TextMeshProUGUI>().text = mainCharacter.e_cooltime.ToString();
            // qSkillUI.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;
            // qSkillUI.GetChild(5).GetComponent<CanvasGroup>().alpha = 0;

            eSkillUI.GetChild(3).GetComponent<Image>().sprite = mainCharacter.E_image;
            qSkillUI.GetChild(4).GetComponent<Image>().sprite = mainCharacter.Q_image;
        }

    }
}
