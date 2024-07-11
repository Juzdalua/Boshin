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
    public int id;
    public string characterName;
    public PlayerElementType elementType;
    public byte[] elementColor;
    public int level = 1;
    public float currentExp;
    public float maxHP = 10f;
    public float currentHP;
    public float atk;
    public float def;
    public float criRate = 0.05f;
    public float criDmg = 0.5f;
    public ItemWeaponObject weapon;
    public int weaponId;

    public float e_cooltime = 3f;
    public float e_startTime = 0f;
    public float e_remainTime = 0f;
    public float e_damage = 3f;
    public float e_gauge = 0f;
    public float q_cooltime = 10f;
    public float q_startTime = 0f;
    public float q_remainTime = 0f;
    public float q_damage = 10f;
    public float q_maxGauge = 9f;
    public float q_currentGauge = 0f;

    public Sprite e_image;
    public Sprite q_image;
    public Sprite profileImage;
    public AudioClip[] e_audios;
    public AudioClip[] q_audios;

    [TextArea(15, 20)] public string e_desctiption;
    [TextArea(15, 20)] public string q_desctiption;

    public bool isJoinParty = false;
    public int partyOrder = 0;
}

[System.Serializable]
public class PlayableCharacter
{
    public int id;
    public string characterName;
    public PlayerElementType elementType;
    public byte[] elementColor;
    public int level = 1;
    public float currentExp;
    public float maxHP = 10f;
    public float currentHP;
    public float atk;
    public float def;
    public float criRate = 0.05f;
    public float criDmg = 0.5f;
    public ItemWeaponObject weapon;
    public int weaponId;

    public float e_cooltime = 3f;
    public float e_startTime = 0f;
    public float e_remainTime = 0f;
    public float e_damage = 3f;
    public float e_gauge = 3f;
    public float e_currentGauge = 0f;
    public float q_cooltime = 10f;
    public float q_startTime = 0f;
    public float q_remainTime = 0f;
    public float q_damage = 10f;
    public float q_maxGauge = 9f;
    public float q_currentGauge = 0f;

    public Sprite e_image;
    public Sprite q_image;
    public Sprite profileImage;
    public AudioClip[] e_audios;
    public AudioClip[] q_audios;

    [TextArea(15, 20)] public string e_desctiption;
    [TextArea(15, 20)] public string q_desctiption;

    public bool isJoinParty = false;
    public int partyOrder = 0;

    public PlayableCharacter(PlayerObject player)
    {
        id = player.id;
        characterName = player.characterName;
        level = player.level;
        currentExp = player.currentExp;
        elementType = player.elementType;
        maxHP = player.maxHP;
        currentHP = maxHP;
        atk = player.atk;
        def = player.def;
        criRate = 0.05f;
        criDmg = 0.5f;
        weapon = player.weapon;
        weaponId = player.weaponId;

        e_cooltime = player.e_cooltime;
        e_startTime = 0f;
        e_remainTime = 0f;
        e_damage = player.e_damage;
        e_gauge = player.e_gauge;
        e_currentGauge = 0f;
        q_cooltime = player.q_cooltime;
        q_startTime = 0f;
        q_remainTime = 0f;
        q_damage = player.q_damage;
        q_maxGauge = player.q_maxGauge;
        q_currentGauge = 0f;
        e_image = player.e_image;
        q_image = player.q_image;
        e_audios = player.e_audios;
        q_audios = player.q_audios;
        e_desctiption = player.e_desctiption;
        q_desctiption = player.q_desctiption;

        profileImage = player.profileImage;
        isJoinParty = player.isJoinParty;
        partyOrder = player.partyOrder;

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
        if (weapon != null) damage += weapon.atkBonus;

        float finalCriRate = criRate;
        if (weapon != null) finalCriRate += weapon.criRateBonus;

        float finalCriDmg = criDmg;
        if (weapon != null) finalCriDmg += weapon.criDmgBonus;

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