using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplayManager : MonoBehaviour
{
    [Foldout("背景圖片")] [Header("攻擊型背景")] [SerializeField] private Sprite warriorBackground;
    [Foldout("背景圖片")] [Header("防禦型背景")] [SerializeField] private Sprite defenderBackground;
    [Foldout("背景圖片")] [Header("魔法型背景")] [SerializeField] private Sprite magicianBackground;
    [Foldout("背景圖片")] [Header("支援型背景")] [SerializeField] private Sprite supporterBackground;
}
