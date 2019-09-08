using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SiteManager : MonoBehaviour
{
    #region 主要參數
    [Foldout("參考組件")] [Header("我方角色欄位")] public CharacterGrid[] ourGrids = new CharacterGrid[18];
    [Foldout("參考組件")] [Header("敵方角色欄位")] public CharacterGrid[] enemyGrids = new CharacterGrid[18];

    internal CharacterData[] ourCharacters = new CharacterData[9];
    internal CharacterData[] enemyCharacters = new CharacterData[9];

    private bool showOrder = true;
    #endregion

    #region Singleton
    private static SiteManager _instance;
    public static SiteManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SiteManager>();

            return _instance;
        }
    }
    #endregion

    #region Function
    /// <summary> 尋找最近角色欄位 </summary>
    public CharacterGrid FindTheCloseGrid(Vector3 pos)
    {
        CharacterGrid gridCache = ourGrids[0];
        float distanceCache = Vector2.Distance(pos, ourGrids[0].rectTransform.position);

        // 檢查我方角色欄位
        for(int i = 1; i < ourGrids.Length; i++)
            if(distanceCache > Vector2.Distance(pos, ourGrids[i].rectTransform.position))
            {
                gridCache = ourGrids[i];
                distanceCache = Vector2.Distance(pos, ourGrids[i].rectTransform.position);
            }

        // 檢查敵方角色欄位
        for (int i = 0; i < enemyGrids.Length; i++)
            if (distanceCache > Vector2.Distance(pos, enemyGrids[i].rectTransform.position))
            {
                gridCache = enemyGrids[i];
                distanceCache = Vector2.Distance(pos, enemyGrids[i].rectTransform.position);
            }

        // 檢查距離
        if (distanceCache > Screen.width / 40)
            gridCache = null;

        return gridCache;
    }

    /// <summary> 檢查角色是否重複 </summary>
    public bool CheckRepeat(Vector3 pos, Sprite sprite)
    {
        // 如果沒有圖片則回傳 false
        if (sprite == null)
            return false;

        // 依照檢查座標決定檢查我方或敵方清單, 檢查到重覆則回傳 true
        if (pos.x < Screen.width * 0.4f)
        {
            for (int i = 0; i < ourGrids.Length; i++)
                if (ourGrids[i].characterIcon.sprite == sprite)
                    return true;
        }
        else
        {
            for (int i = 0; i < enemyGrids.Length; i++)
                if (enemyGrids[i].characterIcon.sprite == sprite)
                    return true;
        }

        // 都沒有則回傳false
        return false;
    }

    /// <summary> 設定角色順序 </summary>
    public int SetCharactersNumber(Vector3 pos, CharacterGrid grid,int order = 0)
    {
        // 如果沒有資料則 return
        if (grid == null || grid.characterData == null)
            return 0;

        // 依照檢查座標決定設置我方或敵方清單, 如果次序為0則找最後欄位, 不為0則設置指定欄位
        if (pos.x < Screen.width * 0.4f)
        {
            if (order == 0)
            {
                for (int i = 0; i < ourCharacters.Length; i++)
                    if (ourCharacters[i] == null)
                    {
                        ourCharacters[i] = grid.characterData;
                        return i + 1;
                    }
                    else if (ourCharacters[i] == grid.characterData)
                        return i + 1;
            }
            else
            {
                ourCharacters[order] = grid.characterData;
                return order;
            }
        }
        else
        {
            if (order == 0)
            {
                for (int i = 0; i < enemyCharacters.Length; i++)
                    if (enemyCharacters[i] == null)
                    {
                        enemyCharacters[i] = grid.characterData;
                        return i + 1;
                    }
                    else if (enemyCharacters[i] == grid.characterData)
                        return i + 1;
            }
            else
            {
                enemyCharacters[order] = grid.characterData;
                return order;
            }
        }

        return 0;
    }

    public void ChangeOrderState()
    {
        showOrder = !showOrder;
        if (showOrder)
        {
            /*for (int i = 0; i < ourGrids.Length; i++)
                for (int j = 0; j < ourCharacters.Length; j++)
                    if (ourGrids[i].characterData == ourCharacters[j])
                        ourGrids[i].orderText.text = (j + 1).ToString();*/

            for (int i = 0; i < enemyGrids.Length; i++)
                for (int j = 0; j < enemyCharacters.Length; j++)
                    if (ourGrids[i].characterData == enemyCharacters[j])
                        ourGrids[i].orderText.text = (j + 1).ToString();
        }
        else
        {
            /*for (int i = 0; i < ourGrids.Length; i++)
                ourGrids[i].orderText.text = "?";*/

            for (int i = 0; i < ourGrids.Length; i++)
                ourGrids[i].orderText.text = "?";
        }
    }
    #endregion
}
