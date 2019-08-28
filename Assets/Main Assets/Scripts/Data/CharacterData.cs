using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public string CharacterName = string.Empty;
    public ClassType classType = 0;
    public BasicAbility basicAbility = new BasicAbility();
    public Texture icon;
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

public struct BasicAbility
{
    public float Damage = 0;
    public float Health = 0;
    public float Reduce = 0;
    public float Critical = 0;
    public float CriticalEffect = 0;
    public float Dodge = 0;
}

