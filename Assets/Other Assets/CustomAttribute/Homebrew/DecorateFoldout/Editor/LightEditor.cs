using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityEngine
{
    /// <summary>
    /// 覆蓋掉原本 Inspector 的自定義 Editor
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true, isFallback = true)]
	public class LightEditor : Editor
    {
        ///////////////////////////////////////////
        //
        //  Classes
        //
        ///////////////////////////////////////////

        /// <summary>
        /// 包含 3 種顏色資訊的集合
        /// </summary>
        private class Colors
        {
            public Color col0;
            public Color col1;
            public Color col2;
        }

        ///////////////////////////////////////////
        //
        //  Fields & Properties
        //
        ///////////////////////////////////////////

        // -------------------
        //  核心資料
        // ---------------------------

        /// <summary>
        /// 儲存 Foldout 設定資料的位置
        /// </summary>
        private const string DATA_StoagePath = "Assets/Other Assets/CustomAttribute/Homebrew/DecorateFoldout/Foldout Data.asset";
        
        /// <summary>
        ///  Foldout 的資料
        /// </summary>
        private FoldoutData foldoutData = null;

        /// <summary>
        ///  Foldout 的 Property 清單
        /// </summary>
        private List<FoldoutProperty> foldoutPropertyList = null;

        /// <summary>
        /// 沒有使用到 Foldout , 使用預設 UI 的 SerializedProperty
        /// </summary>
		private List<SerializedProperty> defaultProps = new List<SerializedProperty>();

        /// <summary>
        /// 用於檢查上一個 Field 是否有 FoldoutAttribute, 並且這 Attribute 有開啟 foldEverything 讓他下一個 Field 有 Foldout
        /// </summary>
        private FoldoutAttribute prevFoldAttribute;

        /// <summary>
        /// 所有目標的物件欄位 (Object Fields)
        /// </summary>
		private List<FieldInfo> ObjectFields
        {
            get
            {
                if (_objectFields == null)
                {
                    IList<Type> typeTree = target.GetType().GetTypeTree();
                    _objectFields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderByDescending(x => typeTree.IndexOf(x.DeclaringType)).ToList();
                }
                return _objectFields;
            }
        }
        private List<FieldInfo> _objectFields = null;

        // -------------------
        //  裝飾性設置
        // ---------------------------

        /// <summary>
        /// UI 的顏色設置
        /// </summary>
        private Colors UIColors
        {
            get
            {
                if (_UIColors == null)
                {
                    _UIColors = new Colors();
                    if (EditorGUIUtility.isProSkin)
                    {
                        _UIColors.col0 = new Color(0.2f, 0.2f, 0.2f, 1f);
                        _UIColors.col1 = new Color(1, 1, 1, 0.1f);
                        _UIColors.col2 = new Color(0.25f, 0.25f, 0.25f, 1f);
                    }
                    else
                    {
                        _UIColors.col0 = new Color(0.2f, 0.2f, 0.2f, 1f);
                        _UIColors.col1 = new Color(1, 1, 1, 0.55f);
                        _UIColors.col2 = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                }
                return _UIColors;
            }
        }
        private Colors _UIColors = null;

        /// <summary>
        /// UI 的外觀
        /// </summary>
        private GUIStyle UIStyle
        {
            get
            {
                if (_UIStyle == null)
                {
                    Texture2D uiTex_in = Resources.Load<Texture2D>("IN foldout focus-6510");
                    Texture2D uiTex_in_on = Resources.Load<Texture2D>("IN foldout focus on-5718");
                    
                    Color c_on = Color.white;
                    
                    _UIStyle = new GUIStyle(EditorStyles.foldout)
                    {
                        overflow = new RectOffset(-10, 0, 3, 0),
                        padding = new RectOffset(25, 0, -3, 0)   
                    };

                    _UIStyle.active.textColor = c_on;
                    _UIStyle.active.background = uiTex_in;
                    _UIStyle.onActive.textColor = c_on;
                    _UIStyle.onActive.background = uiTex_in_on;

                    _UIStyle.focused.textColor = c_on;
                    _UIStyle.focused.background = uiTex_in;
                    _UIStyle.onFocused.textColor = c_on;
                    _UIStyle.onFocused.background = uiTex_in_on;
                }
                return _UIStyle;
            }
        }
        private GUIStyle _UIStyle = null;

        ///////////////////////////////////////////
        //
        //  Methods
        //
        ///////////////////////////////////////////

        // ---------------------------------
        //  初始化 & 設置 FoldoutProperty
        // ---------------------------------------

        private void OnEnable()
        {
            // 初始化 FoldoutProperty
            // 檢查並設置: 回傳 true, 表示已經找到需要的 foldout 並且設置好 ; 回傳 false, 表示沒有找到需要 foldout 的
            if (InitFoldoutProperty())
            {
                // 檢查是否有不正常且無法使用的 FoldoutProperty, 有的話重設 foldoutPropertyList
                if (foldoutPropertyList == null) return;

                foreach (FoldoutProperty checkedfoldoutProp in foldoutPropertyList)
                {
                    if (checkedfoldoutProp == null)
                    {
                        foldoutPropertyList.Clear();
                        InitFoldoutProperty();
                        return;
                    }
                    else
                    {
                        if (checkedfoldoutProp.attribute == null)
                        {
                            foldoutPropertyList.Clear();
                            InitFoldoutProperty();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化 FoldoutProperty
        /// </summary>
        /// <returns>檢查並設置: 回傳 true, 表示已經找到需要的 foldout 並且設置好 ; 回傳 false, 表示沒有找到需要 foldout 的</returns>
        private bool InitFoldoutProperty()
        {
            // 已經找到歸屬的 FoldoutProperty - 用於找出哪些 FoldoutProperty 沒被用到, 需要刪除
            List<FoldoutProperty> searchedFoldoutPropertyList = new List<FoldoutProperty>();

            // 檢查每項欄位是否有 FoldoutAttribute
            for (int i = 0; i < ObjectFields.Count; i++)
            {
                FoldoutAttribute fold = Attribute.GetCustomAttribute(ObjectFields[i], typeof(FoldoutAttribute)) as FoldoutAttribute;
                FoldoutProperty foundFoldoutProperty = null;

                if (fold == null)
                {
                    // 如果此欄位沒有 Foldout 的 Attribute
                    // ----------------------------------------------------------------------
                    // 用 prevFoldAttribute 檢查上一個欄位是否有 FoldoutAttribute, 並且這 Attribute 有開啟 foldEverything 讓他下一個欄位有 Foldout
                    if (prevFoldAttribute != null && prevFoldAttribute.foldEverything)
                    {
                        // 檢查指定欄位的 FoldoutAttribute 有無在 LightBehaviour.FoldoutPropertyList 中
                        foundFoldoutProperty = CheckFoldoutAttribute(prevFoldAttribute, ObjectFields[i].Name);

                        // 將已找到的 FoldoutProperty 加到清單中
                        searchedFoldoutPropertyList.Add(foundFoldoutProperty);
                    }
                    // ----------------------------------------------------------------------
                }
                else
                {
                    // 如果此 Field 有 Foldout 的 Attribute
                    // ----------------------------------------------------------------------
                    // 將 prevFoldAttribute 設成找到的 FoldoutAttribute
                    prevFoldAttribute = fold;

                    // 檢查指定欄位的 FoldoutAttribute 有無在 LightBehaviour.FoldoutPropertyList 中
                    foundFoldoutProperty = CheckFoldoutAttribute(fold, ObjectFields[i].Name);

                    // 將已找到的 FoldoutProperty 加到清單中
                    searchedFoldoutPropertyList.Add(foundFoldoutProperty);
                    // ----------------------------------------------------------------------
                }
            }

            // 如果沒有任何要 foldout 的, 則不繼續執行
            // 回傳 false, 表示沒有找到需要 foldout 的
            if (foldoutPropertyList == null) return false;

            // 先將沒用到的 List 指數找出來, 再將沒用到的 FoldoutProperty 刪除
            List<int> indexToRemoveFromFoldoutPropertyList = new List<int>();
            for (int i = 0; i < foldoutPropertyList.Count; i++)
            {
                bool isInsideSearchedFoldoutPropertyList = false;
                for (int j = 0; j < searchedFoldoutPropertyList.Count; j++)
                {
                    if (foldoutPropertyList[i] == null || searchedFoldoutPropertyList[j] == null) continue;
                    if (foldoutPropertyList[i].attributeName == searchedFoldoutPropertyList[j].attributeName)
                    {
                        isInsideSearchedFoldoutPropertyList = true;
                    }
                }
                if (!isInsideSearchedFoldoutPropertyList) indexToRemoveFromFoldoutPropertyList.Add(i);
            }
            for (int i = 0; i < indexToRemoveFromFoldoutPropertyList.Count; i++)
            {
                if (foldoutPropertyList[indexToRemoveFromFoldoutPropertyList[i]] != null)
                    foldoutPropertyList.RemoveAt(indexToRemoveFromFoldoutPropertyList[i]);
            }

            // 檢查這個 SerializedProperty 是否在 FoldoutPropertyList 中, 也就是有要 Foldout
            SerializedProperty property = serializedObject.GetIterator();   // 找這腳本中的第一個 SerializedProperty
            bool next = property.NextVisible(true); // 找第一個可見的 SerializedProperty, 也就是會顯示在 Inspector 中的第一個欄位
            if (next)
            {
                // 檢查到欄位是否要 Foldout, 如果還有下一個可見欄位, 則繼續檢查到沒有為止
                do
                {
                    CheckProp_WhetherFolded(property);
                } while (property.NextVisible(false));
            }

            // 檢查且設置完成
            // 回傳 true, 表示已經找到需要的 foldout 並且設置好
            return true;
        }

        /// <summary>
        /// 檢查指定欄位的 FoldoutAttribute 有無在 FoldoutPropertyList 中
        /// <para>有 - 將找到的 FoldoutAttribute 值匯入到找到的 FoldoutProperty 中 ; 
        /// 沒有 - 在 FoldoutPropertyList 中創一個新的 FoldoutProperty</para>
        /// </summary>
        /// <param name="checkedFoldoutAttribute">要被檢查的 FoldoutAttribute</param>
        /// <param name="fieldsName">指定欄位的名稱</param>
        /// <returns>設定好的 FoldoutProperty</returns>
        private FoldoutProperty CheckFoldoutAttribute(FoldoutAttribute checkedFoldoutAttribute, string fieldsName)
        {
            // 尋找儲存資料的 Asset, 如果沒有則創個新的
            FoldoutData foldoutDataAsset = AssetDatabase.LoadAssetAtPath<FoldoutData>(DATA_StoagePath);
            if (foldoutDataAsset == null)
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FoldoutData>(), DATA_StoagePath);
                foldoutDataAsset = AssetDatabase.LoadAssetAtPath<FoldoutData>(DATA_StoagePath);
            }
            foldoutData = foldoutDataAsset;

            // 檢查儲存資料中的 foldoutPropertyDataList 裡, 有沒有已經有的腳本名稱
            Predicate<FoldoutPropertyData> predicate = x => x.objectName == target.GetType().Name;
            if (!foldoutDataAsset.foldoutPropertyDataList.Exists(predicate))
            {
                foldoutDataAsset.foldoutPropertyDataList.Add(new FoldoutPropertyData()
                {
                    objectName = target.GetType().Name,
                    foldoutPropList = new List<FoldoutProperty>()
                });
            }
            // 「foldoutPropertyList 設置完成」: 將找到的 foldoutPropertyDataList 中的 foldoutPropList 給 foldoutPropertyList
            foldoutPropertyList = foldoutDataAsset.foldoutPropertyDataList.Find(predicate).foldoutPropList;

            // 檢查 FoldoutPropertyList 中是否已經有檢查欄位的 Attribute
            FoldoutProperty foldoutProp = foldoutPropertyList.Find(
                delegate (FoldoutProperty foldout)
                {
                    return foldout.attributeName == checkedFoldoutAttribute.name;
                });

            if (foldoutProp == null)
            {
                // 如果沒有, 則在 FoldoutPropertyList 中創一個新的 FoldoutProperty
                foldoutProp = new FoldoutProperty
                {
                    attributeName = checkedFoldoutAttribute.name,
                    attribute = checkedFoldoutAttribute,
                    fieldNames = new HashSet<string> { fieldsName }
                };
                foldoutPropertyList.Add(foldoutProp);
            }
            else
            {
                // 如果有, 將找到的 FoldoutAttribute 值匯入到找到的 FoldoutProperty 中
                foldoutProp.attribute = checkedFoldoutAttribute;
                foldoutProp.fieldNames.Add(fieldsName);
            }

            return foldoutProp;
        }

        /// <summary>
        /// 檢查這個 SerializedProperty 是否在 FoldoutPropertyList 中, 也就是有要 Foldout
        /// </summary>
        /// <param name="prop">要檢查的 SerializedProperty</param>
		private void CheckProp_WhetherFolded(SerializedProperty prop)
        {
            bool shouldBeFolded = false;

            // 檢查要處理的 prop (SerializedProperty) 是否有在 FoldoutPropertyList 中, 也就是有要 Foldout
            foreach (FoldoutProperty foldoutProp in foldoutPropertyList)
            {
                if (foldoutProp.fieldNames.Contains(prop.name))
                {
                    shouldBeFolded = true;
                    foldoutProp.props.Add(prop.Copy());

                    break;
                }
            }

            // 沒有使用到 Foldout , 使用預設 UI 的 SerializedProperty
            if (shouldBeFolded == false)
            {
                defaultProps.Add(prop.Copy());
            }
        }

        // -------------------
        //  結束時處理
        // ---------------------------

        private void OnDisable()
        {
            if (foldoutPropertyList == null) return;

            foreach (FoldoutProperty foldoutProp in foldoutPropertyList)
            {
                foldoutProp.Dispose();
            }
        }

        // -------------------
        //  UI 顯示
        // ---------------------------

        public override void OnInspectorGUI()
        {
            // 如果整個 Scripts 都沒有用到 Foldout, 則畫預設的 Inspector, 並不繼續執行
			if (defaultProps.Count == 0)
			{
				DrawDefaultInspector();
				return;
            }

            EditorGUI.BeginChangeCheck();

            serializedObject.Update();
            
            // 在最上方畫出無法編輯的「腳本欄位」,也就是 Unity 腳本在 Inspector 最上方的那個欄位
            using (new EditorGUI.DisabledScope("m_Script" == defaultProps[0].propertyPath))
			{
				EditorGUILayout.PropertyField(defaultProps[0], true);
			}

			EditorGUILayout.Space();

            // 畫出有要被 Foldout 的欄位
			foreach (FoldoutProperty foldoutProp in foldoutPropertyList)
			{
                // ----------------------------
                // 上方折疊箭頭的區域
                // --------------------------------

                Rect rect = EditorGUILayout.BeginVertical();

				EditorGUILayout.Space();

				EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, rect.width + 1, rect.height + 1), UIColors.col0);
				EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, rect.width + 1, rect.height + 1), UIColors.col1);
                
                foldoutProp.expanded = EditorGUILayout.Foldout(foldoutProp.expanded, foldoutProp.attribute.name, true, UIStyle ?? EditorStyles.foldout);

                EditorGUILayout.EndVertical();

                // ----------------------------
                // 展開後下方顯示欄位的區域
                // --------------------------------

                rect = EditorGUILayout.BeginVertical();

				EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, rect.width + 1, rect.height + 1), UIColors.col2);

				if (foldoutProp.expanded)
				{
					EditorGUILayout.Space();
					{
						for (int i = 0; i < foldoutProp.props.Count; i++)
                        {
                            EditorGUI.indentLevel = 1;

                            EditorGUILayout.PropertyField(foldoutProp.props[i],
                                new GUIContent(foldoutProp.props[i].name.FirstLetterToUpperCase()), true);
                            if (i == foldoutProp.props.Count - 1)
                                EditorGUILayout.Space();

                        }
					}
				}

				EditorGUI.indentLevel = 0;
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}
            
            // 將沒被 Foldout 的預設欄位放在最底下
			for (int i = 1; i < defaultProps.Count; i++)
			{
				EditorGUILayout.PropertyField(defaultProps[i], true);
			}

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
            {
				if (target != null)
					EditorUtility.SetDirty (target);
				if (foldoutData != null)
					EditorUtility.SetDirty (foldoutData);
            }
        }
	}
    
	public static partial class FrameworkExtensions
	{
		public static string FirstLetterToUpperCase(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			char[] a = s.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}

		public static IList<Type> GetTypeTree(this Type t)
		{
			List<Type> types = new List<Type>();
			while (t.BaseType != null)
			{
				types.Add(t);
				t = t.BaseType;
			}

			return types;
		}
	}
}
