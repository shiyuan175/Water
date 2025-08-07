using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PropRewardPoolNode : MonoBehaviour
{
    private Image propImage;
    private TextMeshProUGUI propNumText;

    private void Awake()
    {
        propImage = GetComponent<Image>();
        propNumText = GetComponentInChildren<TextMeshProUGUI>();
        propNumText.font = LevelManager.Instance.blueFont;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pos"></param>
    /// <param name="itemNum"></param>
    public void Init(Sprite sprite, Vector2 pos, int itemNum, bool specialRewards)
    {
        //先启用调用Awake
        this.Show();
        propImage.sprite = sprite;
        propNumText.text = specialRewards ? itemNum + "min" : "X" + itemNum;
        propImage.rectTransform.anchoredPosition = pos;
    }

    public void MoveOffScreen()
    {
        RectTransform rectTransform = propImage.rectTransform;
        Vector2 offScreenPos = new Vector2(0, -Screen.height - rectTransform.rect.height * 0.5f);

        rectTransform.DOAnchorPos(offScreenPos, 0.8f)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
               RewardUIManager.Instance.RewardPool.Recycle(propImage);
            });
    }
}
