using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CharacterGrid : MonoBehaviour
{
    #region 主要參數
    [Foldout("參考組件")] [Header("背景")] public Image backGround = null;
    [Foldout("參考組件")] [Header("角色圖示")] public Image characterIcon = null;
    [Foldout("參考組件")] [Header("順序組件")] public CanvasGroup orderGroup = null;
    [Foldout("參考組件")] [Header("順序組件")] public Text orderText = null;

    [Foldout("參數設定")] [Header("位於場地")] public bool isOnSite = false;

    internal CharacterData characterData = null;
    internal RectTransform rectTransform = null;
    private CharacterGrid clone = null;
    private Transform parent = null;
    private bool isMouseIn = false;

    private static CharacterGrid currentGrid;
    #endregion

    #region 通用
    /// <summary> 初始化 </summary>
    public void Start()
    {
        parent = transform.parent;
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary> 開啟 / 關閉 Component </summary>
    public void SetEnabled(bool enabled)
    {
        backGround.enabled = enabled;
        characterIcon.enabled = enabled;
    }
    #endregion

    #region Events
    /// <summary> 滑鼠按下事件 </summary>
    public void MouseDownEvent()
    {
        // 如果未生成 clone 則生成 clone
        if (clone == null)
        {
            clone = Instantiate(gameObject, transform).GetComponent<CharacterGrid>();
            clone.GetComponent<RectTransform>().sizeDelta = rectTransform.sizeDelta;
            clone.transform.position = transform.position;
            clone.characterData = characterData;
        }

        // 指定值
        if (isOnSite)
            clone.orderGroup.alpha = 1;
        clone.characterData = characterData;
        clone.orderText.text = orderText.text;
        clone.backGround.color = backGround.color;
        clone.characterIcon.sprite = characterIcon.sprite;

        // 移出父物件(為迴避 Mask)
        clone.transform.SetParent(transform.root);
        // 開啟 clone Component
        clone.SetEnabled(true);
        // 指定 currentGrid
        currentGrid = clone;

        if (isOnSite) // 如果是場上欄位則背景指定白色、清除 icon、關閉順序
        {
            backGround.color = Color.white;
            characterIcon.sprite = null;
            orderGroup.alpha = 0;
        }
        else // 如果不是則關閉 Component
            SetEnabled(false);
    }

    /// <summary> 滑鼠放開事件 </summary>
    public void MouseUpEvent()
    {
        // 抓取最近欄位
        CharacterGrid grid = SiteManager.instance.FindTheCloseGrid(Input.mousePosition);
        // 檢查欄位及是否重複, 有欄位且沒重複則指定值
        if (grid != null && !SiteManager.instance.CheckRepeat(Input.mousePosition, currentGrid.characterIcon.sprite))
        {
            if (grid.orderGroup.alpha == 1)
            {
                int result = 0;
                int.TryParse(grid.orderText.text, out result);
                grid.orderText.text = SiteManager.instance.SetCharactersNumber(Input.mousePosition, clone, result).ToString();
            }
            else
                grid.orderText.text = SiteManager.instance.SetCharactersNumber(Input.mousePosition, clone).ToString();

            grid.orderGroup.alpha = 1;
            grid.characterData = characterData;
            grid.backGround.color = currentGrid.backGround.color;
            grid.characterIcon.sprite = currentGrid.characterIcon.sprite;
        }

        if (grid == null && isOnSite)
            SiteManager.instance.RemoveCharactersNumber(Input.mousePosition, clone);

        // 清除 currentGrid
        currentGrid = null;
        // 關閉 clone Component
        clone.SetEnabled(false);
        // 開啟自身 Component
        SetEnabled(true);
        // 指定 clone 座標
        clone.transform.position = transform.position;
        // 指定 clone 父物件
        clone.transform.SetParent(transform);
        // 關閉 clone 順序
        clone.orderGroup.alpha = 0;
    }

    /// <summary> 拖曳事件 </summary>
    public void DargEvent()
    {
        clone.transform.position = Input.mousePosition;
    }
    #endregion
}
