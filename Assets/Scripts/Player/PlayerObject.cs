using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerElementType
{
    Pyro, // 불
    Cryo, // 얼음
    Hydro, // 물
    Electro, // 번개
    Anemo, // 바람
    Geo, // 땅
    Dendro // 풀
}

public enum PlayerAttackType
{
    Weapon,
    ESkill,
    QSkill
}

[CreateAssetMenu(fileName = "New Player Object", menuName = "Player System/Player")]
public class PlayerObject : ScriptableObject
{
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string characterName;
    public string CharacterName => characterName;
    [SerializeField] private PlayerElementType elementType;
    public PlayerElementType ElementType => elementType;
    [SerializeField] private byte[] elementColor;
    public byte[] ElementColor => elementColor;
    [SerializeField] private int level = 1;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    [SerializeField] private float currentExp;
    public float CurrentExp
    {
        get { return currentExp; }
        set { currentExp = value; }
    }
    [SerializeField] private float maxHP = 10f;
    public float MaxHP => maxHP;
    [SerializeField] private float currentHP;
    public float CurrentHP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }
    [SerializeField] private float atk;
    public float Atk => atk;
    [SerializeField] private float def;
    public float Def => def;
    [SerializeField] private float criRate = 0.05f;
    public float CriRate => criRate;
    [SerializeField] private float criDmg = 0.5f;
    public float CriDmg => criDmg;
    [SerializeField] private ItemWeaponObject weapon;
    public ItemWeaponObject Weapon
    {
        get { return weapon; }
        set { weapon = value; }
    }
    [SerializeField] private int weaponId;
    public int WeaponId
    {
        get { return weaponId; }
        set { weaponId = value; }
    }

    [SerializeField] private float e_cooltime = 3f;
    public float E_cooltime => e_cooltime;
    [SerializeField] private float e_startTime = 0f;
    public float E_startTime
    {
        get { return e_startTime; }
        set { e_startTime = value; }
    }
    [SerializeField] private float e_remainTime = 0f;
    public float E_remainTime => e_remainTime;
    [SerializeField] private float e_damage = 3f;
    public float E_damage => e_damage;
    [SerializeField] private float e_gauge = 0f;
    public float E_gauge => e_gauge;
    [SerializeField] private float q_cooltime = 10f;
    public float Q_cooltime => q_cooltime;
    [SerializeField] private float q_startTime = 0f;
    public float Q_startTime
    {
        get { return q_startTime; }
        set { q_startTime = value; }
    }
    [SerializeField] private float q_remainTime = 0f;
    public float Q_remainTime => q_remainTime;
    [SerializeField] private float q_damage = 10f;
    public float Q_damage => q_damage;
    [SerializeField] private float q_maxGauge = 9f;
    public float Q_maxGauge => q_maxGauge;
    [SerializeField] private float q_currentGauge = 0f;
    public float Q_currentGauge
    {
        get { return q_currentGauge; }
        set { q_currentGauge = value; }
    }

    [SerializeField] private Sprite e_image;
    public Sprite E_image => e_image;
    [SerializeField] private Sprite q_image;
    public Sprite Q_image => q_image;
    [SerializeField] private Sprite profileImage;
    public Sprite ProfileImage => profileImage;
    [SerializeField] private AudioClip[] e_audios;
    public AudioClip[] E_audios => e_audios;
    [SerializeField] private AudioClip[] q_audios;
    public AudioClip[] Q_audios => q_audios;

    [TextArea(15, 20)][SerializeField] private string e_desctiption;
    public string E_desctiption => e_desctiption;
    [TextArea(15, 20)][SerializeField] private string q_desctiption;
    public string Q_desctiption => q_desctiption;

    [SerializeField] private bool isJoinParty = false;
    public bool IsJoinParty
    {
        get { return isJoinParty; }
        set { isJoinParty = value; }
    }
    [SerializeField] private int partyOrder = 0;
    public int PartyOrder
    {
        get { return partyOrder; }
        set { partyOrder = value; }
    }
}

[System.Serializable]
public class PlayableCharacter
{
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string characterName;
    public string CharacterName => characterName;
    [SerializeField] private PlayerElementType elementType;
    public PlayerElementType ElementType => elementType;
    [SerializeField] private byte[] elementColor;
    public byte[] ElementColor => elementColor;
    [SerializeField] private int level = 1;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    [SerializeField] private float currentExp;
    public float CurrentExp
    {
        get { return currentExp; }
        set { currentExp = value; }
    }
    [SerializeField] private float maxHP = 10f;
    public float MaxHP => maxHP;
    [SerializeField] private float currentHP;
    public float CurrentHP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }
    [SerializeField] private float atk;
    public float Atk => atk;
    [SerializeField] private float def;
    public float Def => def;
    [SerializeField] private float criRate = 0.05f;
    public float CriRate => criRate;
    [SerializeField] private float criDmg = 0.5f;
    public float CriDmg => criDmg;
    [SerializeField] private ItemWeaponObject weapon;
    public ItemWeaponObject Weapon
    {
        get { return weapon; }
        set { weapon = value; }
    }
    [SerializeField] private int weaponId;
    public int WeaponId
    {
        get { return weaponId; }
        set { weaponId = value; }
    }

    [SerializeField] private float e_cooltime = 3f;
    public float E_cooltime => e_cooltime;
    [SerializeField] private float e_startTime = 0f;
    public float E_startTime
    {
        get { return e_startTime; }
        set { e_startTime = value; }
    }
    [SerializeField] private float e_remainTime = 0f;
    public float E_remainTime => e_remainTime;
    [SerializeField] private float e_damage = 3f;
    public float E_damage => e_damage;
    [SerializeField] private float e_gauge = 0f;
    public float E_gauge => e_gauge;
    [SerializeField] private float q_cooltime = 10f;
    public float Q_cooltime => q_cooltime;
    [SerializeField] private float q_startTime = 0f;
    public float Q_startTime
    {
        get { return q_startTime; }
        set { q_startTime = value; }
    }
    [SerializeField] private float q_remainTime = 0f;
    public float Q_remainTime => q_remainTime;
    [SerializeField] private float q_damage = 10f;
    public float Q_damage => q_damage;
    [SerializeField] private float q_maxGauge = 9f;
    public float Q_maxGauge => q_maxGauge;
    [SerializeField] private float q_currentGauge = 0f;
    public float Q_currentGauge
    {
        get { return q_currentGauge; }
        set { q_currentGauge = value; }
    }

    [SerializeField] private Sprite e_image;
    public Sprite E_image => e_image;
    [SerializeField] private Sprite q_image;
    public Sprite Q_image => q_image;
    [SerializeField] private Sprite profileImage;
    public Sprite ProfileImage => profileImage;
    [SerializeField] private AudioClip[] e_audios;
    public AudioClip[] E_audios => e_audios;
    [SerializeField] private AudioClip[] q_audios;
    public AudioClip[] Q_audios => q_audios;

    [TextArea(15, 20)][SerializeField] private string e_desctiption;
    public string E_desctiption => e_desctiption;
    [TextArea(15, 20)][SerializeField] private string q_desctiption;
    public string Q_desctiption => q_desctiption;

    [SerializeField] private bool isJoinParty = false;
    public bool IsJoinParty
    {
        get { return isJoinParty; }
        set { isJoinParty = value; }
    }
    [SerializeField] private int partyOrder = 0;
    public int PartyOrder
    {
        get { return partyOrder; }
        set { partyOrder = value; }
    }

    public PlayableCharacter(PlayerObject player)
    {
        id = player.Id;
        characterName = player.CharacterName;
        level = player.Level;
        currentExp = player.CurrentExp;
        elementType = player.ElementType;
        maxHP = player.MaxHP;
        currentHP = MaxHP;
        atk = player.Atk;
        def = player.Def;
        criRate = 0.05f;
        criDmg = 0.5f;
        weapon = player.Weapon;
        weaponId = player.WeaponId;

        e_cooltime = player.E_cooltime;
        e_startTime = 0f;
        e_remainTime = 0f;
        e_damage = player.E_damage;
        e_gauge = player.E_gauge;
        q_cooltime = player.Q_cooltime;
        q_startTime = 0f;
        q_remainTime = 0f;
        q_damage = player.Q_damage;
        q_maxGauge = player.Q_maxGauge;
        q_currentGauge = 0f;
        e_image = player.E_image;
        q_image = player.Q_image;
        e_audios = player.E_audios;
        q_audios = player.Q_audios;
        e_desctiption = player.E_desctiption;
        q_desctiption = player.Q_desctiption;

        profileImage = player.ProfileImage;
        isJoinParty = player.IsJoinParty;
        partyOrder = player.PartyOrder;

        switch (elementType)
        {
            case PlayerElementType.Pyro:
                elementColor = new byte[3] { 255, 172, 114 };
                break;

            case PlayerElementType.Cryo:
                elementColor = new byte[3] { 178, 243, 249 };
                break;

            case PlayerElementType.Hydro:
                elementColor = new byte[3] { 5, 224, 255 };
                break;

            case PlayerElementType.Electro:
                elementColor = new byte[3] { 222, 185, 253 };
                break;

            case PlayerElementType.Anemo:
                elementColor = new byte[3] { 157, 252, 211 };
                break;

            case PlayerElementType.Geo:
                elementColor = new byte[3] { 241, 215, 95 };
                break;

            case PlayerElementType.Dendro:
                elementColor = new byte[3] { 169, 226, 36 };
                break;
        }
    }

    public void ChargeQGauge()
    {
        q_currentGauge += e_gauge;
    }

    public Dictionary<bool, int> GetDamageCalc(PlayerAttackType attackType)
    {
        float damage = atk;
        switch (attackType)
        {
            case PlayerAttackType.Weapon:
                break;

            case PlayerAttackType.ESkill:
                damage += e_damage;
                break;

            case PlayerAttackType.QSkill:
                damage += q_damage;
                break;
        }
        if (weapon != null) damage += weapon.AtkBonus;

        float finalCriRate = criRate;
        if (weapon != null) finalCriRate += weapon.CriRateBonus;

        float finalCriDmg = criDmg;
        if (weapon != null) finalCriDmg += weapon.CriDmgBonus;

        bool isCri = false;
        int randomCriValue = Random.Range(1, 100);
        if (randomCriValue <= finalCriRate * 100f)
        {
            isCri = true;
            damage += damage * finalCriDmg;
        }

        Dictionary<bool, int> damageResult = new Dictionary<bool, int>();
        damageResult.Add(isCri, (int)damage);

        return damageResult;
    }
}