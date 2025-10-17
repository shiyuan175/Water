using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIImageAdapter : BaseUIAdapter
{
    [SerializeField]
    [Tooltip("父亲的1080宽")]  float width;
    [SerializeField]
    [Tooltip("父亲的1920高")]  float height;
    protected override void Adapter()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentTransform = transform.parent.GetComponent<RectTransform>();
        Vector2 originalSize = rectTransform.sizeDelta;
       
        // 设置新的尺寸
        rectTransform.sizeDelta = new Vector2(
            originalSize.x * (parentTransform.sizeDelta.x /  width),
            originalSize.y * (parentTransform.sizeDelta.y / height)
        );
    }
}
