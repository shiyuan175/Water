using UnityEngine;
using QFramework;
using System;
using JsonFileData;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{

    public partial class PrograssGiftPanel : ViewController,ICanGetModel
    {
        [SerializeField] private RewardSpriteMappingSO rewardSprite;
        [SerializeField] public Sprite CoinsImg;
        Sequence flashSequence;
        private void OnEnable()
        {
            rewardSprite.Initialize();
        }
        private void OnDisable()
        {
            flashSequence?.Kill();
        }


        public void Initialize(Func<bool> Buy, PGReward rewards, bool isGot, bool isLock)
        {
            InitUI(rewards.Price, isGot, isLock);
            InitItem(rewards.RewardItem);
            SetBtnClick(Buy);
        }

        public void InitItem(RewardItem[] rewards)
        {

            // 设置内容
            for (int i = 0; i < rewards.Length; i++)
            {
                Image img = control.GetChild(i).GetComponent<Image>();
                TextMeshProUGUI txt = control.GetChild(i).Find("Txt_Red").GetComponent<TextMeshProUGUI>();
                if (rewards[i].itemType == "AvatarId")
                {
                    Debug.Log("头像,待头像管理器补充");
                }
                else
                {

                    if (rewardSprite.GetRewardSprite(rewards[i].itemType) != null)
                        img.sprite = rewardSprite.GetRewardSprite(rewards[i].itemType);
                    else
                        img.sprite = CoinsImg;
                    SpecialRewardsType _rewardEnum1;

                    if (Enum.TryParse<SpecialRewardsType>(rewards[i].itemType, out _rewardEnum1))
                    {
                        txt.text = rewards[i].itemQuantity.ToString() + "m";
                    }
                    else
                    {
                        txt.text = "x" + rewards[i].itemQuantity.ToString();
                    }
                }
            }
            // 删除空
            for (int i = rewards.Length; i < control.childCount; i++)
            {
                control.GetChild(i).gameObject.SetActive(false);
            }
        }
        public void InitUI(float price, bool isGot, bool isLock)
        {
          
            if (price == 0)
                BtnBuy.transform.Find("Txt_Red").GetComponent<TextMeshProUGUI>().text = "Fress";
            else
                BtnBuy.transform.Find("Txt_Red").GetComponent<TextMeshProUGUI>().text = $"$ {price}";

            if (isLock || !isGot)
                BtnBuy.transform.Find("ImgLock").gameObject.SetActive(true);
            else
                BtnBuy.transform.Find("ImgLock").gameObject.SetActive(false);
            if (isGot)
                BtnBuy.gameObject.SetActive(value: false);
            else
                BtnBuy.gameObject.SetActive(true);

            if (isLock)
            {
                BtnBuy.interactable = false;
                ImgLock.gameObject.SetActive(true);
            }           
            else
            {
                BtnBuy.interactable = true;
                ImgLock.gameObject.SetActive(false);
            }
               
        }

        private void SetBtnClick(Func<bool> Buy)
        {
            BtnBuy.onClick.RemoveAllListeners();
            BtnBuy.onClick.AddListener(() =>
            {       
                if (Buy())
                {
                    BtnBuy.interactable = false;
                    float durationTime = 0.5f;
                    BtnBuy.transform.Find("Txt_Red").GetComponent<TextMeshProUGUI>().text = "";
                    BtnBuy.transform.DOScale(1.2f, durationTime * 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        BtnBuy.transform.DOScale(0f, durationTime * 0.7f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            BtnBuy.transform.localScale = Vector3.one;
                            BtnBuy.Hide();
                        });
                    });
                }
            });
        }
        // 改颜色，消失改回
        public Tween UnLock()
        {
            Image img = BtnBuy.transform.Find("ImgLock").GetComponent<Image>();
            Color oldColor = img.color;
            // 单个 Tween 完成所有动画
            float duration = 0.9f;
            float elapsed = 0f;

            Tween unlockTween = DOTween.To(() => elapsed, x => elapsed = x, duration, duration)                
                .OnUpdate(() =>
                {
                    float progress = elapsed / duration;

                    // 控制透明度：先保持全亮，后段淡出
                    float alpha = progress < 0.5f ? 1f : Mathf.Lerp(1f, 0f, (progress - 0.5f) * 2f);

                    // 控制缩放：先放大后缩小
                    float scale = progress < 0.3f ?
                        Mathf.Lerp(1f, 1.3f, progress / 0.3f) :
                        Mathf.Lerp(1.2f, 0.8f, (progress - 0.3f) / 0.7f);
                    img.transform.localScale = Vector3.one * scale;
                })
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(4f, () =>
                    {
                        BtnBuy.interactable = true;
                        img.gameObject.SetActive(false);
                        img.transform.localScale = Vector3.one;
                        img.color = oldColor;
                    });
                });
               

            return unlockTween;

        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
