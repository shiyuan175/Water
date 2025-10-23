using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum LayoutType
{
    Square = 0,
    HorizontalRectangle = 1,
    VerticalRectangle = 2,
    AutoAdapter = 3
}
/// <summary>
/// 自适应GridLayoutGroup工具 v1.2.101725 
/// 较上一个版本更新了新的参数，调整了自动计算排列的规则，使得工具更为灵活
/// 使用说明：在需要大量排序的面板上挂载该脚本，注意面板如果有动态变化的其他组件，请新建空父亲，默认枚举为正方形
/// 注意事项：子物体大要求一致。
/// </summary>
public class UILayoutAapter : BaseUIAdapter
{
    [Tooltip("设置子物体的排列形状")]
    [SerializeField] private LayoutType layoutType;
    [Tooltip("强制设置子物体的排列数量")]
    [SerializeField] private int constraintCounts;
  /*  [SerializeField] private bool isCompulsory= false;*/
    private int childCounts;
    
    [SerializeField]
    private List<RectTransform> targetRectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;
    private float parentWidth;
    private float parentHeight;
    private float totalWidth;
    private float totalHeight;


    protected override void Adapter()
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

                if (rectTransform != null && rectTransform.gameObject.activeSelf == true)
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

        Vector2 cellSize = new Vector2();
        Vector2 spaceSize = new Vector2();
        totalWidth = parentWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
        totalHeight = parentHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

        // 初始化目标物体数量为正方形的形式
        int elementsRow = Mathf.CeilToInt(Mathf.Sqrt(childCounts));
        int elementsColumn = Mathf.CeilToInt(Mathf.Sqrt(childCounts));
        // 设置排序方式和对应方式的目标物体数量
        switch (layoutType)
        {
            // 默认为Square类型 
            case LayoutType.Square:
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                /*gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;*/
                break;

            case LayoutType.HorizontalRectangle:
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                if (constraintCounts != 0)
                {
                    if (constraintCounts < childCounts)
                        elementsColumn = constraintCounts;
                    else
                        elementsColumn = childCounts;
                }
                gridLayoutGroup.constraintCount = elementsColumn;
                elementsRow = Mathf.CeilToInt((float)childCounts / elementsColumn);
                
                break;

            case LayoutType.VerticalRectangle:
                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;

                if (constraintCounts != 0 )
                {
                    if (constraintCounts < childCounts)
                        elementsRow = constraintCounts;
                    else
                        elementsRow = childCounts;
                }
                gridLayoutGroup.constraintCount = elementsRow;
                elementsColumn = Mathf.CeilToInt((float)childCounts / elementsRow);

                break;
            default:
                break;
        }
        // 计算 

        cellSize = CalculateCellSize(targetRectTransform[0], elementsRow, elementsColumn);
        int maxElement = Mathf.Max(elementsColumn, elementsRow);
        if (maxElement > 1)
            spaceSize = cellSize * 0.1f * maxElement / (maxElement - 1);
        else
            spaceSize = Vector2.zero;
        cellSize *= 0.9f;

        // 设置layouts
        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = spaceSize;
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }

    private Vector2 CalculateCellSize(RectTransform child, int targetRow, int targetColumn)
    {
        float childWidth = child.rect.width;
        float childHeight = child.rect.height;
        float OWHRatio = childWidth / childHeight; 
        // 先看横是否可以满足，不能则看竖
        float cellWidth = totalWidth / targetColumn;
        float cellHeight = totalHeight / targetRow;
       /* Debug.Log(transform.parent.parent.name + targetRow + targetColumn + "　"+totalHeight + "  " + cellWidth * 1.0f / OWHRatio * targetRow +" "+cellWidth +"　　"+OWHRatio);*/
        if (totalHeight >= cellWidth*1.0f / OWHRatio * targetRow)
        {
            return new Vector2(cellWidth, cellWidth / OWHRatio);
        }
        else
        {
            return new Vector2(cellHeight * OWHRatio, cellHeight);
        }
        /* if(!isCompulsory || layoutType == LayoutType.Square)
         {
             if (totalHeight >= cellWidth / OWHRatio * targetRow)
             {
                 return new Vector2(cellWidth, cellWidth / OWHRatio);
             }
             else
             {
                 return new Vector2(cellHeight * OWHRatio, cellHeight);
             }
         }
         else
         {
             switch (layoutType)
             {
                 case LayoutType.HorizontalRectangle:
                     if (totalHeight >= cellWidth / OWHRatio * targetRow)
                     {
                         return new Vector2(cellWidth, cellWidth / OWHRatio);
                     }
                     else
                     {
                         return new Vector2(cellHeight * OWHRatio, cellHeight);
                     }
                     break;

                 case LayoutType.VerticalRectangle:
                     if (totalHeight >= cellWidth / OWHRatio * targetRow)
                     {
                         return new Vector2(cellWidth, cellWidth / OWHRatio);
                     }
                     else
                     {
                         return new Vector2(cellHeight * OWHRatio, cellHeight);
                     }
             }

         }*/



        // 分开计算
        // 宽能放更多东西，说明要以高为基准
        /* if (NWHRatio > OWHRatio)
         {
             return new Vector2(cellHeight * OWHRatio, cellHeight);
         }
         else
         {

             return new Vector2(cellWidth, cellWidth / OWHRatio);
         }*/

    }

}
