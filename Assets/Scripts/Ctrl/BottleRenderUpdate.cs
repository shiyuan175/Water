
using System;
using System.Collections.Generic;
using QFramework;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BottleRenderUpdate : MonoBehaviour
{
    public WaterRenderUpdate[] waterRenders;
    public SkeletonGraphic[] Brooms;
    public Transform WaterSpine;
    public Transform WaterSpinePos;
    public Image BotteImage;
    public Image MaskImage;

    public int bottleIndex = 1;
    public GameObject[] waterTopSurfaces;
    public Transform fillWaterTransform;
    public Transform fillWaterPosition; // 固定倒水的位置
    public Transform[] corners;

    private Material _material;
    private Vector3 _waterScale;
    private BottleCtrl _bottleCtrl;
    
    private BottleCtrl _otherBottle = null;

    // 倒水控制
    private bool _isPouring = false;
    private float _targetScaleX = 1f;
    private float _targetScaleY = 1f;
    private float _scaleSpeed = 2f; // 缩放速度，可调节

    // 原始水面缩放
    private Vector3 _originalScale;
    private float _originalWaterWidth;

    public void Start()
    {
        _bottleCtrl = transform.parent.GetComponent<BottleCtrl>();

        _material = new Material(MaskImage.material);
        _material.SetFloat("_StencilRef", bottleIndex);
        MaskImage.material = _material;

        BotteImage.material = new Material(BotteImage.material);

        //int i = 0;
        foreach (var waterRenderUpdater in waterRenders)
        {
            waterRenderUpdater.Stencil = bottleIndex;
            //waterRenderUpdater.RenderQueue = 2901 + i++;
        }

        foreach (var skeletonGraphic in Brooms)
        {
            var material = new Material(skeletonGraphic.material);
            material.SetFloat("_Stencil", bottleIndex);
            material.SetFloat("_StencilOp", ((int)UnityEngine.Rendering.StencilOp.Keep) * 1.0f);
            material.SetFloat("_StencilComp", ((int)UnityEngine.Rendering.CompareFunction.Disabled) * 1.0f);
            material.renderQueue = 3000 + 1;
            skeletonGraphic.material = material;
            //skeletonGraphic.TrimRenderers();
            var skeletonSubmeshGraphics = skeletonGraphic.gameObject.GetComponentsInChildren<SkeletonSubmeshGraphic>();
            foreach (var skeletonSubmeshGraphic in skeletonSubmeshGraphics)
            {
                skeletonSubmeshGraphic.material = material;
            }
        }

        _waterScale = WaterSpine.localScale;
        var waterSpineMaterial = new Material(WaterSpine.GetComponent<SkeletonGraphic>().material);
        waterSpineMaterial.SetFloat("_StencilWater", bottleIndex);
        //waterSpineMaterial.SetFloat("_StencilCompWater", (int)UnityEngine.Rendering.CompareFunction.Equal);
        waterSpineMaterial.SetFloat("_StencilCompWater", (int)UnityEngine.Rendering.CompareFunction.Equal);
        waterSpineMaterial.SetFloat("_StencilReadMaskWater", 255);
        WaterSpine.GetComponent<SkeletonGraphic>().material = waterSpineMaterial;


        if (WaterSpine != null)
        {
            _originalScale = WaterSpine.localScale;
            _targetScaleX = _originalScale.x;
            _targetScaleY = _originalScale.y;
        }

        var rectTransform = WaterSpine.GetComponent<RectTransform>();
        _originalWaterWidth = rectTransform.rect.width * _waterScale.x; // 原始本地宽度

        SetWaterSortingOrder(2);
    }

    /* public void LateUpdate()
     {
         // 设置最高水位线
         Assert.IsTrue(waterTopSurfaces != null && waterTopSurfaces.Length == 2);
         var waterHeightClip = Mathf.Min(waterTopSurfaces[0].transform.position.y, waterTopSurfaces[1].transform.position.y);
         foreach (var waterRenderUpdater in waterRenders)
         {
             waterRenderUpdater.FillHeightClip = waterHeightClip;
         }

         // 设置fill water效果的位置
         if (fillWaterTransform != null && _otherBottle != null)
         {
             //fillWaterTransform.localRotation = Quaternion.Inverse(transform.rotation);
             //fillWaterTransform.position = new Vector3(_otherBottle.transform.position.x, fillWaterPosition.position.y, fillWaterPosition.position.z);

             bool isPourLeft = _otherBottle.transform.position.x < transform.position.x;
             fillWaterTransform.localRotation = Quaternion.Inverse(transform.rotation);
             Vector3 localPos = transform.InverseTransformPoint(fillWaterPosition.position);
             if (isPourLeft)
                 localPos.x = -localPos.x;
             fillWaterTransform.position = transform.TransformPoint(localPos);
             Vector3 localScale = fillWaterTransform.localScale;
             localScale.x = Mathf.Abs(localScale.x) * (isPourLeft ? -1 : 1);
             fillWaterTransform.localScale = localScale;
         }

         // 计算顶部spine的位置和缩放
         WaterSpine.rotation = Quaternion.identity;
         var position = WaterSpinePos.position;
         var waterSpineHeight = Mathf.Min(position.y, waterHeightClip);
         Vector2 point1 = new Vector2(-1, waterSpineHeight);
         Vector2 point2 = new Vector2(1, waterSpineHeight);

         List<Vector2> intesectionPoints = new List<Vector2>();
         for (int i = 0; i < 4; i++)
         {
             bool bIntersect = LineIntersection.GetLineSegmentIntersection(point1, point2, new Vector2(corners[i].position.x, corners[i].position.y),
                 new Vector2(corners[(i + 1) % 4].position.x, corners[(i + 1) % 4].position.y), out Vector2 intersectionPoint);
             if (bIntersect)
             {
                 intesectionPoints.Add(intersectionPoint);
             }
         }

         Assert.IsTrue(intesectionPoints.Count == 2);

         position = new Vector3((intesectionPoints[0].x + intesectionPoints[1].x) * 0.5f, waterSpineHeight, position.z);
         WaterSpine.localScale = new Vector3(Mathf.Abs(intesectionPoints[1].x - intesectionPoints[0].x) / 1.688f, 1, 1);
         WaterSpine.position = position;

         //Vector3 p0 = WaterSpine.parent.InverseTransformPoint(intesectionPoints[0]);
         //Vector3 p1 = WaterSpine.parent.InverseTransformPoint(intesectionPoints[1]);
         //
         //float currentWidth = Mathf.Abs(p1.x - p0.x);
         //float scaleX = currentWidth / _originalWaterWidth;
         //
         //WaterSpine.localScale = new Vector3(scaleX * _waterScale.x, _waterScale.y, _waterScale.z);
         //WaterSpine.position = new Vector3((intesectionPoints[0].x + intesectionPoints[1].x) * 0.5f,
         //                                  waterSpineHeight,
         //                                  WaterSpine.position.z); 
     }*/

    public void LateUpdate()
    {
        // 1. 设置最高水位线
        Assert.IsTrue(waterTopSurfaces != null && waterTopSurfaces.Length == 2);
        float waterHeightClip = Mathf.Min(
            waterTopSurfaces[0].transform.position.y,
            waterTopSurfaces[1].transform.position.y
        );
        foreach (var waterRenderUpdater in waterRenders)
        {
            waterRenderUpdater.FillHeightClip = waterHeightClip;
        }

        // 2. 设置倒水 fill 水特效位置
        if (fillWaterTransform != null && _otherBottle != null)
        {
            bool isPourLeft = _otherBottle.transform.position.x < transform.position.x;
            fillWaterTransform.localRotation = Quaternion.Inverse(transform.rotation);
            Vector3 localPos = transform.InverseTransformPoint(fillWaterPosition.position);
            if (isPourLeft) localPos.x = -localPos.x;
            fillWaterTransform.position = transform.TransformPoint(localPos);

            Vector3 localScale = fillWaterTransform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (isPourLeft ? -1 : 1);
            fillWaterTransform.localScale = localScale;
        }

        // 3. 计算水面顶部 Spine 的交点
        WaterSpine.rotation = Quaternion.identity;
        Vector3 position = WaterSpinePos.position;
        float waterSpineHeight = Mathf.Min(position.y, waterHeightClip);

        Vector2 point1 = new Vector2(-1, waterSpineHeight);
        Vector2 point2 = new Vector2(1, waterSpineHeight);

        List<Vector2> intersectionPoints = new List<Vector2>();
        for (int i = 0; i < 4; i++)
        {
            bool bIntersect = LineIntersection.GetLineSegmentIntersection(
                point1, point2,
                new Vector2(corners[i].position.x, corners[i].position.y),
                new Vector2(corners[(i + 1) % 4].position.x, corners[(i + 1) % 4].position.y),
                out Vector2 intersectionPoint
            );
            if (bIntersect) intersectionPoints.Add(intersectionPoint);
        }

        Assert.IsTrue(intersectionPoints.Count == 2);

        // 4. 计算目标位置和缩放
        position = new Vector3(
            (intersectionPoints[0].x + intersectionPoints[1].x) * 0.5f,
            waterSpineHeight,
            position.z
        );

        float targetScaleX = Mathf.Abs(intersectionPoints[1].x - intersectionPoints[0].x) / 1.688f;
        float targetScaleY = 1f;

        if (_otherBottle != null) // 倒水状态
        {
            targetScaleX *= 1.4f; // X 放大
            targetScaleY *= 0.7f; // Y 缩小
        }
        else // 静止状态：保持原始 Spine 缩放 + 水平居中 UI 适配
        {
            position.x = WaterSpinePos.position.x;        // 水面水平居中
            targetScaleX = _originalScale.x;              // 原始宽度
            targetScaleY = _originalScale.y;              // 原始高度
        }

        // 5. 平滑过渡
        Vector3 currentScale = WaterSpine.localScale;
        float smoothX = Mathf.Lerp(currentScale.x, targetScaleX, Time.deltaTime * 6f);
        float smoothY = Mathf.Lerp(currentScale.y, targetScaleY, Time.deltaTime * 6f);
        WaterSpine.localScale = new Vector3(smoothX, smoothY, currentScale.z);

        // 6. 设置最终位置
        WaterSpine.position = position;
    }

    public float GetBottleBottomY()
    {
        // 获取瓶子底部的Y坐标
        //Debug.LogError(corners[3].position.y);
        return corners[3].position.y;
    }
    
    public void SetWaterSortingOrder(int order)
    {
        foreach (var waterRenderUpdater in waterRenders)
        {
            waterRenderUpdater.SortingOrder = order;
        }
    }

    // 移动的瓶子，最后渲染（改变）
    public void SetMoveBottleRenderState(bool isMove, BottleCtrl otherBottle = null)
    {
        _otherBottle = otherBottle;
        if (isMove)
        {
            //_bottleCtrl.GetComponent<Canvas>().sortingOrder = 1;
            //SetWaterSortingOrder(1);
            if (_otherBottle != null)
            {
                //_otherBottle.GetComponent<Canvas>().sortingOrder = 2;

                var bottleRender = _otherBottle.GetComponentInChildren<BottleRenderUpdate>();
                //bottleRender.SetWaterSortingOrder(2);
                _bottleCtrl.ImgWaterDown.material.SetFloat("_ClipHeight", bottleRender.GetBottleBottomY());
            }
        }
        else
        {
            //_bottleCtrl.GetComponent<Canvas>().sortingOrder = 0;
            //SetWaterSortingOrder(0);
            if (_otherBottle != null)
            {
                //_otherBottle.GetComponent<Canvas>().sortingOrder = 0;

                var bottleRender = _otherBottle.GetComponentInChildren<BottleRenderUpdate>();
                //bottleRender.SetWaterSortingOrder(0);
                _bottleCtrl.ImgWaterDown.material.SetFloat("_ClipHeight", -1000.0f);
            }
        }

    }
}
