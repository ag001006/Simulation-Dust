using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ButtonState : MonoBehaviour
{
    [Foldout("參數設定")] [Header("初始值")] [SerializeField] private bool initialValue = false;

    [Foldout("參數設定")] [Header("作用組件")] [SerializeField] private Image effectImage = null;
    [Foldout("參數設定")] [Header("作用顏色")] [SerializeField] private Color enableColor = Color.white;
    [Foldout("參數設定")] [Header("失效顏色")] [SerializeField] private Color disableColor = Color.white;
    
    [Foldout("參數設定")] [Header("作用組件")] [SerializeField] private Text effectText = null;
    [Foldout("參數設定")] [Header("作用文字")] [SerializeField] private string enableText = string.Empty;
    [Foldout("參數設定")] [Header("失效文字")] [SerializeField] private string disableText = string.Empty;

    private bool _currentValue;
    private bool currentValue
    {
        get
        {
            return _currentValue;
        }
        set
        {
            _currentValue = value;

            if (effectImage != null)
                effectImage.color = value ? enableColor : disableColor;

            if (effectText != null)
                effectText.text = value ? enableText : disableText;
        }
    }

    private void Start()
    {
        currentValue = initialValue;
    }

    public void Trigger()
    {
        currentValue = !currentValue;
    }

    public void SetValue(bool value)
    {
        currentValue = value;
    }
}
