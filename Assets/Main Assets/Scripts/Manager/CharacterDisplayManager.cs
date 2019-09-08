using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CharacterDisplayManager : MonoBehaviour
{
    #region 主要參數
    [Foldout("參考組件")] [Header("角色列表父物件")] [SerializeField] private Transform listParent = null;
    [Foldout("參考組件")] [Header("角色欄位Prefab")] [SerializeField] private GameObject gridPrefab = null;
    [Foldout("參考組件")] [Header("Scrollbar")] [SerializeField] private Scrollbar scrollbar = null;
    [Foldout("參考組件")] [Header("Position Parent")] [SerializeField] private RectTransform positionParent = null;
    [Foldout("參考組件")] [Header("GridLayoutGroup")] [SerializeField] private GridLayoutGroup gridLayoutGroup = null;

    [Foldout("攻擊型")] [Header("攻擊型背景")] [SerializeField] private Sprite warriorBackground = null;
    [Foldout("攻擊型")] [Header("攻擊型背景顏色")] [SerializeField] private Color warriorBackgroundColor = Color.red;
    [Foldout("攻擊型")] [Header("傳說角色列表")] [SerializeField] private CharacterData[] warriorCharacters_Legend = new CharacterData[0];
    [Foldout("攻擊型")] [Header("五星角色列表")] [SerializeField] private CharacterData[] warriorCharacters_Five = new CharacterData[0];
    [Foldout("攻擊型")] [Header("四星角色列表")] [SerializeField] private CharacterData[] warriorCharacters_Four = new CharacterData[0];
    [Foldout("攻擊型")] [Header("三星角色列表")] [SerializeField] private CharacterData[] warriorCharacters_Three = new CharacterData[0];
    [Foldout("防禦型")] [Header("防禦型背景")] [SerializeField] private Sprite defenderBackground = null;
    [Foldout("防禦型")] [Header("防禦型背景顏色")] [SerializeField] private Color defenderBackgroundColor = Color.blue;
    [Foldout("防禦型")] [Header("傳說角色列表")] [SerializeField] private CharacterData[] defenderCharacters_Legend = new CharacterData[0];
    [Foldout("防禦型")] [Header("五星角色列表")] [SerializeField] private CharacterData[] defenderCharacters_Five = new CharacterData[0];
    [Foldout("防禦型")] [Header("四星角色列表")] [SerializeField] private CharacterData[] defenderCharacters_Four = new CharacterData[0];
    [Foldout("防禦型")] [Header("三星角色列表")] [SerializeField] private CharacterData[] defenderCharacters_Three = new CharacterData[0];
    [Foldout("魔法型")] [Header("魔法型背景")] [SerializeField] private Sprite magicianBackground = null;
    [Foldout("魔法型")] [Header("魔法型背景顏色")] [SerializeField] private Color magicianBackgroundColor = Color.magenta;
    [Foldout("魔法型")] [Header("傳說角色列表")] [SerializeField] private CharacterData[] magicianCharacters_Legend = new CharacterData[0];
    [Foldout("魔法型")] [Header("五星角色列表")] [SerializeField] private CharacterData[] magicianCharacters_Five = new CharacterData[0];
    [Foldout("魔法型")] [Header("四星角色列表")] [SerializeField] private CharacterData[] magicianCharacters_Four = new CharacterData[0];
    [Foldout("魔法型")] [Header("三星角色列表")] [SerializeField] private CharacterData[] magicianCharacters_Three = new CharacterData[0];
    [Foldout("支援型")] [Header("支援型背景")] [SerializeField] private Sprite supporterBackground = null;
    [Foldout("支援型")] [Header("支援型背景顏色")] [SerializeField] private Color supporterBackgroundColor = Color.green;
    [Foldout("支援型")] [Header("傳說角色列表")] [SerializeField] private CharacterData[] supporterCharacters_Legend = new CharacterData[0];
    [Foldout("支援型")] [Header("五星角色列表")] [SerializeField] private CharacterData[] supporterCharacters_Five = new CharacterData[0];
    [Foldout("支援型")] [Header("四星角色列表")] [SerializeField] private CharacterData[] supporterCharacters_Four = new CharacterData[0];
    [Foldout("支援型")] [Header("三星角色列表")] [SerializeField] private CharacterData[] supporterCharacters_Three = new CharacterData[0];

    private bool displayFive = true;
    private bool displayFour = true;
    private bool displayThree = true;
    private ClassType currentType = ClassType.Warrior;

    internal List<CharacterGrid> characterGrids = new List<CharacterGrid>();
    internal int countCache;

    private Vector3 positionParent_StartPos = Vector3.zero;
    private float positionParent_FinalY = 0;

    /// <summary> 初始化 </summary>
    private void Start()
    {
        positionParent_StartPos = positionParent.localPosition;
        UpdateCharacter(currentType);
    }

    /// <summary> 角色欄捲動軸初始化 </summary>
    public void Initialization()
    {
        scrollbar.value = 0;
        scrollbar.size = (int)(positionParent.rect.height / gridLayoutGroup.cellSize.y) / Mathf.Ceil(CharacterDisplayManager.instance.countCache / Mathf.Floor(positionParent.rect.width / gridLayoutGroup.cellSize.x));
        positionParent.localPosition = positionParent_StartPos;
        positionParent_FinalY = Mathf.Round(Mathf.Ceil(CharacterDisplayManager.instance.countCache / Mathf.Floor(positionParent.rect.width / gridLayoutGroup.cellSize.x)) * (1 - scrollbar.size) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));

        scrollbar.gameObject.SetActive(scrollbar.size != 1);
    }
    #endregion

    #region Singleton
    private static CharacterDisplayManager _instance;
    public static CharacterDisplayManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CharacterDisplayManager>();

            return _instance;
        }
    }
    #endregion

    #region 捲動
    /// <summary> 捲動角色欄位 </summary>
    public void UpdateChildPosition(float percent)
    {
        positionParent.localPosition = new Vector3(0, Mathf.Lerp(0, positionParent_FinalY, percent), 0);
    }
    #endregion

    #region 顯示角色
    /// <summary> 更新角色欄位顯示 </summary>
    private void UpdateCharacter(ClassType type)
    {
        // 依照 type 顯示角色
        switch (type)
        {
            case ClassType.Warrior:
                DisplayCharacters(warriorCharacters_Legend, warriorCharacters_Five, warriorCharacters_Four, warriorCharacters_Three, warriorBackgroundColor);
                break;
            case ClassType.Defender:
                DisplayCharacters(defenderCharacters_Legend, defenderCharacters_Five, defenderCharacters_Four, defenderCharacters_Three, defenderBackgroundColor);
                break;
            case ClassType.Magician:
                DisplayCharacters(magicianCharacters_Legend, magicianCharacters_Five, magicianCharacters_Four, magicianCharacters_Three, magicianBackgroundColor);
                break;
            case ClassType.Supporter:
                DisplayCharacters(supporterCharacters_Legend, supporterCharacters_Five, supporterCharacters_Four, supporterCharacters_Three, supporterBackgroundColor);
                break;
        }

        // 角色欄捲動軸初始化
        Initialization();
        // 關閉剩餘角色欄位
        ClearList();
    }

    /// <summary> 依照是否顯示更新角色顯示 </summary>
    private void DisplayCharacters(CharacterData[] legendDatas, CharacterData[] fiveDatas, CharacterData[] fourDatas, CharacterData[] threeDatas, Color backGroundColor)
    {
        if (displayFive)
        {
            DisplayCharacters(legendDatas, backGroundColor);
            DisplayCharacters(fiveDatas, backGroundColor);
        }

        if (displayFour)
            DisplayCharacters(fourDatas, backGroundColor);

        if (displayThree)
            DisplayCharacters(threeDatas, backGroundColor);

    }
    /// <summary> 顯示傳入的角色陣列 </summary>
    private void DisplayCharacters(CharacterData[] datas, Color backGroundColor)
    {
        for (int i = datas.Length - 1; i >= 0; i--)
        {
            if (countCache >= characterGrids.Count)
            {
                CharacterGrid grid = Instantiate(gridPrefab, listParent).GetComponent<CharacterGrid>();
                characterGrids.Add(grid);
            }

            characterGrids[countCache].SetEnabled(true);
            characterGrids[countCache].characterData = datas[i];
            characterGrids[countCache].backGround.color = backGroundColor;
            characterGrids[countCache].characterIcon.sprite = datas[i].icon_Sprite;
            countCache++;
        }
    }

    /// <summary> 關閉剩餘角色欄位 </summary>
    private void ClearList()
    {
        for (int i = countCache; i < characterGrids.Count; i++)
            if (countCache < characterGrids.Count)
                characterGrids[i].SetEnabled(false);

        countCache = 0;
    }
    #endregion

    #region Button Event
    public void DisplayWarriorCharacters() { currentType = ClassType.Warrior; UpdateCharacter(currentType); }
    public void DisplayDefenderCharacters() { currentType = ClassType.Defender; UpdateCharacter(currentType); }
    public void DisplayMagicianCharacters() { currentType = ClassType.Magician; UpdateCharacter(currentType); }
    public void DisplaySupporterCharacters() { currentType = ClassType.Supporter; UpdateCharacter(currentType); }

    public void TriggerFiveEnable() { displayFive = !displayFive; UpdateCharacter(currentType); }
    public void TriggerFourEnable() { displayFour = !displayFour; UpdateCharacter(currentType); }
    public void TriggerThreeEnable() { displayThree = !displayThree; UpdateCharacter(currentType); }
    #endregion
}
