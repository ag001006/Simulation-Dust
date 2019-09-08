using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData_New", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string CharacterName = string.Empty;
    public ClassType classType = 0;
    public int grade = 0;
    public BasicAbility basicAbility = new BasicAbility();
    public Texture icon;
    public Sprite icon_Sprite;
}

/// <summary> 職業 </summary>
public enum ClassType
{
    /// <summary> 攻擊型 </summary>
    Warrior,
    /// <summary> 防禦型 </summary>
    Defender,
    /// <summary> 魔法型 </summary>
    Magician,
    /// <summary> 支援型 </summary>
    Supporter
}

[System.Serializable] public struct BasicAbility
{
    [SerializeField] public float Damage;
    [SerializeField] public float Health;
    [SerializeField] public float Reduce;
    [SerializeField] public float Critical;
    [SerializeField] public float CriticalEffect;
    [SerializeField] public float Dodge;
}

