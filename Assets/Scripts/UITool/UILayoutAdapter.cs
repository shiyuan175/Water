using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum LayoutType
{
    Square = 0,
    HorizontalRectangle = 1,
    VerticalRectangle = 2,
    None
}
/// <summary>
/// 自适应GridLayoutGroup工具 v1.1.101525 
/// 较上一个版本更新了枚举情况的处理，能够更加自由的处理子物体的排列，同时解决了子物体一定要是正方形的问题
/// 使用说明：在需要大量排序的面板上挂载该脚本，注意面板如果有动态变化的其他组件，请新建空父亲，默认枚举为正方形
/// 注意事项：子物体大要求一致。
/// </summary>
public class UILayoutAapter : MonoBehaviour
{
    [Tooltip("设置子物体的排列形状")]
    [SerializeField] private LayoutType layoutType;
    [Tooltip("强制设置子物体的排列数量")]
    [SerializeField] private int constraintCount;
    private int childCounts;

    private List<RectTransform> targetRectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private float childLenght;
    private RectTransform rectTransform;
    private float parentWidth;
    private float parentHeight;
    private float totalWidth;
    private float totalHeight;

    #region 动态调整的触发与初始化
    void OnEnable()
    {
        // 延后执行，保证不受到动态ui的影响
        ExecuteAtEndOfFrame(Adapter);
    }
    public void ExecuteAtEndOfFrame(System.Action action)
    {
        StartCoroutine(ExecuteAtEndOfFrameCoroutine(action));
    }
    private IEnumerator ExecuteAtEndOfFrameCoroutine(System.Action action)
    {
        yield return new WaitForEndOfFrame();
        action?.Invoke();
    }
    #endregion

    /// <summary>
    /// 动态调整
    /// </summary>
    void Adapter()
    {
        // 初始化
        targetRectTransform = new List<RectTransform>();
        childCounts = targetRectTransform.Count;
        if (childCounts == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                RectTransform rectTransform = child.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    targetRectTransform.Add(rectTransform);
                }
            }
        }
        childCounts = targetRectTransform.Count;

        //RectTransform 
        rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;

        parentHeight = rectTransform.rect.height;
        parentWidth = rectTransform.rect.width;
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
            gridLayoutGroup = gameObject.AddComponent<GridLayoutGroup>();
        // 计算
        Vector2 cellSize = new Vector2();
        Vector2 spaceSize = new Vector2();
        totalWidth = parentWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
        totalHeight = parentHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

        // 初始化目标物体数量为正方形的形式
        int elementsRow = (int)Math.Sqrt(childCounts);

        // 设置排序方式和对应方式的目标物体数量
        switch (layoutType)
        {
            // 默认为Square类型 
            case LayoutType.None:
            case LayoutType.Square:
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                break;

            case LayoutType.HorizontalRectangle:
                elementsRow += 1;
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                break;

            case LayoutType.VerticalRectangle:
                elementsRow += 1;
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                break;
        }

        // 将目标物体数设置为用户需求
        if (constraintCount != 0)
        {
            elementsRow = constraintCount;
        }
        gridLayoutGroup.constraintCount = elementsRow;



       // 计算 
       Debug.Log(childCounts);
        Debug.Log(elementsRow);
        cellSize = CalculateCellSize(targetRectTransform[0], elementsRow);
        spaceSize = cellSize * 0.1f*elementsRow/(elementsRow-1);
        cellSize *= 0.9f;

        // 设置layouts
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spaceSize;
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }

    private Vector2 CalculateCellSize(RectTransform child, int targetRow)
    {
        float childWidth = child.rect.width;
        float childHeight = child.rect.height;
        float OWHRatio = childWidth / childHeight;
        float cellWidth = totalWidth / targetRow;
        float cellHeight = totalHeight / targetRow;
        float NWHRatio = cellWidth / cellHeight;

        // 宽能放更多东西，说明要以高为基准
        if(NWHRatio > OWHRatio)
        {

            return new Vector2(cellHeight * OWHRatio, cellHeight);
        }
        else
        {

            return new Vector2(cellWidth, cellWidth / OWHRatio);
        }
        
    }
}
