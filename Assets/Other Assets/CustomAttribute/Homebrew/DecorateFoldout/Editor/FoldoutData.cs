using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine
{
    /// <summary>
    /// 儲存單一 FoldoutAttribute 中的資訊
    /// </summary>
    [Serializable]
    public class FoldoutProperty
    {
        ///////////////////////////////////////////
        //
        //  Fields
        //
        ///////////////////////////////////////////

        /// <summary>
        /// 存放的 FoldoutAttribute 的名稱
        /// <para>用於給 Editor 辨認是否已經有此 Attribute, 因為 FoldoutAttribute 無法序列化儲存在 Unity 中, 所以額外設字串儲存</para>
        /// </summary>
        [SerializeField] public string attributeName;

        /// <summary>
        /// 存放的 FoldoutAttribute
        /// </summary>
        [SerializeField] public FoldoutAttribute attribute;

        /// <summary>
        /// 要被 Foldout 的欄位名稱
        /// </summary>
        [SerializeField] public HashSet<string> fieldNames = new HashSet<string>();

        /// <summary>
        /// 要被 Foldout 的 SerializedProperty
        /// </summary>
        [SerializeField] public List<SerializedProperty> props = new List<SerializedProperty>();

        /// <summary>
        /// 是否已展開
        /// </summary>
        [SerializeField] public bool expanded;

        ///////////////////////////////////////////
        //
        //  Methods
        //
        ///////////////////////////////////////////

        /// <summary>
        /// 丟棄下次要重置的參數
        /// </summary>
        public void Dispose()
        {
            fieldNames.Clear();
            props.Clear();
        }
    }

    /// <summary>
    /// 含有 FoldoutProperty 的資料
    /// </summary>
    [Serializable]
    public class FoldoutPropertyData
    {
        ///////////////////////////////////////////
        //
        //  Fields
        //
        ///////////////////////////////////////////

        /// <summary>
        /// 此物件（腳本）的名字
        /// </summary>
        [SerializeField] public string objectName;

        /// <summary>
        ///  Foldout 的 Property Dictionary
        /// </summary>
        [SerializeField] public List<FoldoutProperty> foldoutPropList = new List<FoldoutProperty>();
    }

    /// <summary>
    /// 儲存 Foldout 資料的 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "Foldout Data", menuName = "Foldout/Data")]
    public class FoldoutData : ScriptableObject
    {
        ///////////////////////////////////////////
        //
        //  Fields
        //
        ///////////////////////////////////////////

        /// <summary>
        ///  Foldout 的 Property Data 清單
        /// </summary>
        public List<FoldoutPropertyData> foldoutPropertyDataList = new List<FoldoutPropertyData>();
    }
}
