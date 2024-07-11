using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{

    GameObject player;
    PlayableCharacter mainCharacter;
    int playerId = 0;
    public Collider attackCollider;

    [Header("Loot System")]
    public List<GameObject> lootItem = new List<GameObject>();

    private Rigidbody _rb;
    private Animator _animator;
    private Vector3? target;
    private QuestObjectController questObjectController;

    private float gravity = Physics.gravity.y;
    float randomX;
    float randomZ;
    private Vector3 destination;
    private float speed = 2f;
    private float sprint = 5f;
    private float time;

    private float distanceToChasePlayer = 20.0f;
    private Vector3 originalPosition;

    [Header("Monster UI")]
    public CanvasGroup monsterUI;
    public TextMeshProUGUI monsterNameFrame;
    public Slider hpSlider;
    private int maxHp;
    private int currentHp;
    public GameObject hpText;
    public GameObject damageText;
    private int monsterDamage;

    [Header("Sound")]
    public AudioClip MosterAttackAudioClip;
    public AudioClip HitAudioClip;
    public AudioClip DeadAudioClip;
    [Range(0, 5)] public float AudioVolume = 5f;

    bool isBattle = false;
    bool isFind = false;
    bool isAlive = true;
    ObjectData objectData;

    void Awake()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        mainCharacter = PlayerManager.Instance.GetMainCharacter();
        objectData = GetComponent<ObjectData>();
        originalPosition = transform.position;
        attackCollider.enabled = false;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        monsterUI.alpha = 0;

        SetUpMonster();
        SetDestination();
        time = Time.time;
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            mainCharacter = PlayerManager.Instance.GetMainCharacter();
        }
        if (player != null && playerId == 0) playerId = player.GetComponent<ObjectData>().GetId();

        if (currentHp == 0) monsterUI.alpha = 0;

        if (currentHp != 0 && isBattle)
        {
            if (Time.timeScale == 0) monsterUI.alpha = 0;
            else monsterUI.alpha = 1;
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            FindPlayer();
            MoveAround();
        }

        if (time + 2 < Time.time)
        {
            SetDestination();
            time = Time.time;
        }
    }

    private void SetUpMonster()
    {
        List<Dictionary<string, object>> monsterDatas = CSVReader.Read("Monsters/MonsterData");
        int monsterId = GetComponent<ObjectData>().GetId();
        Dictionary<string, object> monsterData = new Dictionary<string, object>();

        if (monsterId != 0 && monsterDatas.Count > 0)
        {
            for (int i = 0; i < monsterDatas.Count; i++)
            {
                if (monsterDatas[i]["id"].ToString() == monsterId.ToString())
                {
                    monsterData = monsterDatas[i];
                    break;
                }
            }

            if (monsterData != null)
            {
                monsterNameFrame.text = monsterData["name"].ToString();
                int.TryParse(monsterData["hp"].ToString(), out maxHp);
                int.TryParse(monsterData["attack"].ToString(), out monsterDamage);
            }

            currentHp = maxHp;
            hpText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = maxHp.ToString();
            hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentHp.ToString();
        }
    }

    private void FindPlayer()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= distanceToChasePlayer && mainCharacter.currentHP > 0)
        {
            target = player.transform.position;
            if (monsterUI.alpha == 0)
            {
                monsterUI.alpha = 1;
            }

            if (!isFind)
            {
                isFind = true;
                SoundManager.Instance.BattleSfxPlay();
                isBattle = true;
            }
        }
        else
        {
            target = null;
            if (monsterUI.alpha != 0)
            {
                monsterUI.alpha = 0;
            }
            isBattle = false;
        }
    }

    private void SetDestination()
    {
        randomX = Random.Range(-2.5f, 2.5f);
        randomZ = Random.Range(-2.5f, 2.5f);
        destination = new Vector3(transform.position.x + randomX, 0, transform.position.z + randomZ);
        destination.y = _rb.velocity.y;
    }

    private IEnumerator BackOriginalPositionCoroutine()
    {
        while (Vector3.Distance(transform.position, originalPosition) >= 5.0f)
        {
            yield return null;
            if (currentHp != maxHp)
            {
                currentHp = maxHp;
                hpSlider.value = maxHp;
                hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)maxHp).ToString();
            }

            _animator.SetBool("isMoving", true);
            _animator.SetBool("isRun", true);
            _animator.SetBool("isAttack", false);
            target = null;
            transform.LookAt(originalPosition);

            transform.position += (originalPosition - transform.position).normalized * sprint * 2 * Time.fixedDeltaTime;
            if (Vector3.Distance(transform.position, originalPosition) <= 1.0f)
            {
                yield break;
            }
        }
    }

    private void MoveAround()
    {
        if (target == null)
        {
            if (Vector3.Distance(transform.position, originalPosition) >= 30.0f) StartCoroutine(BackOriginalPositionCoroutine());
            else
            {
                transform.LookAt(destination);
                _animator.SetBool("isAttack", false);
                if (Vector3.Distance(destination, transform.position) > 1.0f)
                {
                    _animator.SetBool("isMoving", true);
                    _animator.SetBool("isRun", false);
                    transform.position += (destination - transform.position).normalized * speed * Time.fixedDeltaTime;
                }
                else
                {
                    _animator.SetBool("isMoving", false);
                    _animator.SetBool("isRun", false);
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, originalPosition) >= 30.0f) StartCoroutine(BackOriginalPositionCoroutine());
            else
            {
                transform.LookAt(player.transform.position);
                Vector3 destination = player.transform.position + new Vector3(1, 0, 1);
                float distanceToPlayer = Vector3.Distance(destination, transform.position);

                if (distanceToPlayer > 10.0f && distanceToPlayer < 30.0f)
                {
                    _animator.SetBool("isMoving", true);
                    _animator.SetBool("isRun", true);
                    _animator.SetBool("isAttack", false);
                    transform.position += (destination - transform.position).normalized * sprint * Time.fixedDeltaTime;
                }

                else if (distanceToPlayer <= 10.0f && distanceToPlayer > 2.0f)
                {
                    attackCollider.enabled = false;
                    _animator.SetBool("isMoving", true);
                    _animator.SetBool("isRun", false);
                    _animator.SetBool("isAttack", false);
                    transform.position += (destination - transform.position).normalized * speed * Time.fixedDeltaTime;

                }
                else if (distanceToPlayer <= 2.0f)
                {
                    _animator.SetBool("isMoving", false);
                    _animator.SetBool("isRun", false);
                    _animator.SetBool("isAttack", true);
                    transform.position = transform.position;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        List<string> tags = new List<string> {
            "Player_Weapon", "Player_ESkill", "Player_QSkill"
        };

        if (tags.Contains(other.tag))
        {
            Dictionary<bool, int> damageResult = new Dictionary<bool, int>();
            switch (other.tag)
            {
                case "Player_Weapon":
                    damageResult = mainCharacter.GetDamageCalc(PlayerAttackType.Weapon);

                    break;

                case "Player_ESkill":
                    damageResult = mainCharacter.GetDamageCalc(PlayerAttackType.ESkill);
                    break;

                case "Player_QSkill":
                    damageResult = mainCharacter.GetDamageCalc(PlayerAttackType.QSkill);
                    break;
            }

            StartCoroutine(DecreaseHP(other, damageResult.Keys.ToList()[0], damageResult.Values.ToList()[0]));

            if (currentHp > 0)
            {
                StartCoroutine(OnHitAnimation());
            }
        }
    }

    public IEnumerator DecreaseHP(Collider playerCollider, bool isCri, int playerDamage)
    {
        if (isAlive)
        {
            GameObject damageTextPrefeb = Instantiate(damageText, transform.position, Quaternion.identity);

            damageTextPrefeb.GetComponent<DamageTextController>().DisplayDamage(playerDamage, isCri);

            currentHp -= playerDamage;
            if (currentHp <= 0)
            {
                currentHp = 0;
                isAlive = false;
            }

            hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)currentHp).ToString();
            StartCoroutine(DecreaseHPBar());

            if (currentHp == 0)
            {
                DeadMonster();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DeadMonster()
    {
        attackCollider.enabled = false;
        _animator.SetTrigger("Dead");
        _animator.SetBool("isMoving", false);
        _animator.SetBool("isAttack", false);
        StartCoroutine(DeadMonsterCoroutine());

        // Quest
        questObjectController = GetComponent<QuestObjectController>();
        if (questObjectController != null)
        {
            QuestManager.Instance.UpdateQuest(questObjectController.questId, QuestType.DefeatMonster, objectData.GetId());
        }

        // Loot System
        if (lootItem.Count > 0)
        {
            for (int i = 0; i < lootItem.Count; i++)
            {
                float itemRandomX = Random.Range(-2.5f, 2.5f);
                float itemRandomZ = Random.Range(-2.5f, 2.5f);
                Instantiate(lootItem[i], transform.position + new Vector3(itemRandomX, 0.1f, itemRandomZ), Quaternion.identity);
            }
        }
    }

    private IEnumerator DecreaseHPBar()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            hpSlider.value = Mathf.Lerp(hpSlider.value, (float)currentHp / (float)maxHp, timer / 0.5f);
            yield return null;
        }
        yield break;
    }

    private IEnumerator DeadMonsterCoroutine()
    {
        monsterUI.alpha = 0;

        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
        yield break;
    }

    private IEnumerator OnHitAnimation()
    {
        _animator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.5f);
        _animator.ResetTrigger("Hit");
        yield break;
    }

    private void OnHitSound(AnimationEvent animationEvent)
    {
        if (HitAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("MonsterHitAudio", HitAudioClip);
        }
    }

    private void OnDeadSound(AnimationEvent animationEvent)
    {
        if (DeadAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("MonsterDead", DeadAudioClip);
        }
    }

    private void OnMonsterAttackSound(AnimationEvent animationEvent)
    {
        if (MosterAttackAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("MosterAttack", MosterAttackAudioClip);
        }
    }

    private void OnMonsterAttackStart(AnimationEvent animationEvent)
    {
        attackCollider.enabled = true;
    }

    private void OnMonsterAttackEnd(AnimationEvent animationEvent)
    {
        attackCollider.enabled = false;
    }

    public float GetMonsterDamage()
    {
        return monsterDamage;
    }

}