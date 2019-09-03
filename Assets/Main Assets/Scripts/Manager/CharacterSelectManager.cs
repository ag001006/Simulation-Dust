using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    [Foldout("參考組件")] [Header("Scrollbar")] [SerializeField] private Scrollbar scrollbar = null;
    [Foldout("參考組件")] [Header("Position Parent")] [SerializeField] private RectTransform positionParent = null;
    [Foldout("參考組件")] [Header("GridLayoutGroup")] [SerializeField] private GridLayoutGroup gridLayoutGroup = null;

    private float positionParent_FinalY = 0;

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        scrollbar.value = 0;
        scrollbar.size = (int)(positionParent.rect.height / gridLayoutGroup.cellSize.y) / Mathf.Ceil(positionParent.childCount / Mathf.Floor(positionParent.rect.width / gridLayoutGroup.cellSize.x));
        positionParent.localPosition = Vector3.zero;
        positionParent_FinalY = Mathf.Round(Mathf.Ceil(positionParent.childCount / Mathf.Floor(positionParent.rect.width / gridLayoutGroup.cellSize.x)) * (1 - scrollbar.size) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
    }

    public void UpdateChildPosition(float percent)
    {
        positionParent.localPosition = new Vector3(0, Mathf.Lerp(0, positionParent_FinalY, percent), 0);
    }
}
