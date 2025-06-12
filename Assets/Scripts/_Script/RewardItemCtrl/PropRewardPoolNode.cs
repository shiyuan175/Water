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
    // ����ID����5��ͼ��ϴ�
    private const int INTERVAL = 5;

    private void Awake()
    {
        propImage = GetComponent<Image>();
        propNumText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pos"></param>
    /// <param name="itemID"></param>
    /// <param name="itemNum"></param>
    public void Init(Sprite sprite,Vector2 pos,int itemID ,int itemNum)
    {
        //�����õ���Awake
        this.Show();
        propImage.sprite = sprite;
        float _scale;
        if (itemID > INTERVAL)
            _scale = Random.Range(0.95f, 1f);
        else 
            _scale = Random.Range(0.95f, 1.1f);

        propNumText.text = "X" + itemNum;
        propImage.rectTransform.localScale = propImage.rectTransform.localScale * _scale;
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
                RewardItemManager.Instance.RewardPool.Recycle(propImage);
            });
    }
}
