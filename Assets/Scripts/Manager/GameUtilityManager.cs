using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUtilityManager : MonoSingleton<GameUtilityManager>
{
    public override void OnSingletonInit()
    {
        
    }

    /// <summary>
    ///  将源 UI 对象的位置映射到目标 UI 对象所在 Canvas 的局部坐标
    /// </summary>
    ///<param name="sourceObj">源目标对象</param>
    /// <param name="targetObj">要应用的对象</param>
    public void GetLocalPositionInCanvas(RectTransform sourceObj, RectTransform targetObj , Action<Vector2> callback = null)
    {
        Canvas _sourceObjCanvas = sourceObj.GetComponentInParent<Canvas>().rootCanvas;
        Canvas _targetObjCanvas = targetObj.GetComponentInParent<Canvas>().rootCanvas;

        //延迟一帧构建Layout Group
        ActionKit.DelayFrame(1, () =>
        {
            //目标对象坐标转为世界坐标
            var _screenPoint = _sourceObjCanvas.worldCamera.WorldToScreenPoint(sourceObj.position);

            // 转化画布
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 targetObj.parent as RectTransform,
                 _screenPoint,
                 _targetObjCanvas.worldCamera,
                 out Vector2 _localPoint);
            targetObj.anchoredPosition = _localPoint;
            callback?.Invoke(_localPoint);
        }).Start(this);
    }

    /// <summary>
    /// 获取是否点击到目标UI
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsPointerOverTargetUI(Vector2 screenPosition, GameObject target)
    {
        if (target == null)
            return false;

        Canvas _targetCanvas = target.GetComponentInParent<Canvas>();

        GraphicRaycaster raycaster = _targetCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == target)
                return true;
        }

        return false;
    }


}
