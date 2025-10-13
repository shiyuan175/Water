using UnityEngine;
using QFramework;
using UnityEngine.Rendering;
using TMPro;
using JsonFileData;
using UnityEngine.UI;
using System;
using DG.Tweening;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class BattlePassContentPanel : ViewController, ICanGetModel
    {
        [SerializeField] public Sprite[] boxImgs;
        [SerializeField] public Sprite[] levelImgs; // 0表示还不能，1表示能
        [SerializeField] public Sprite CoinsImg;
        [SerializeField] private RewardSpriteMappingSO rewardSprite;

        private BattlePassADActivity mBattlePassADActivity;
        private BattlePassModel bPModel;


        public void Awake()
        {
            // 获取逻辑层，将奖励发放等功能转交给逻辑层处理
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
            rewardSprite.Initialize();
            bPModel = this.GetModel<BattlePassModel>();
        }
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        public void Initialize(int level)
        {
            // awake获取到了，这里为null，所以补了一个get
            bPModel = this.GetModel<BattlePassModel>();

            // 获取内容
            RewardItem[] freeData = bPModel.BPDate.Rewards[level].Free;
            RewardItem[] vipData = bPModel.BPDate.Rewards[level].Vip;

            var freeGiftPanel = ImgGiftFree.GetComponent<GiftPanel>();
            var vipGiftPanel = ImgGiftVip.GetComponent<GiftPanel>();

            // 设置进度条
            if (level < bPModel.RewardLevel)
            {
                ImgProgressBar.fillAmount = 1;
                ImgLevel.sprite = levelImgs[1];
                TextLevel.text = level.ToString();
            }
            else
            {
                ImgProgressBar.fillAmount = 0;
                ImgLevel.sprite = levelImgs[0];
                TextLevel.text = level.ToString();
            }
            // 设置领取条
            if (level == bPModel.RewardLevel - 1)
            {
                SetDividingLine(1);

            }
            #region 设置freeGift
            if (bPModel.FreeRewardGotLevel > level)
            {
                freeGiftPanel.ImgAlReceive.Show();
            }
            else
            {
                freeGiftPanel.ImgAlReceive.Hide();
                if (level < bPModel.RewardLevel)
                {
                    freeGiftPanel.BtnClaim.Show();
                    var _freeGiftPanel = freeGiftPanel;
                    freeGiftPanel.BtnClaim.onClick.RemoveAllListeners();
                    freeGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(freeData, _freeGiftPanel, false));
                }
                else
                {
                    freeGiftPanel.BtnClaim.Hide();
                }
            }

            // 创建预制体
            if (!bPModel.BPDate.Rewards[level].FreeIsBox)
            {
                GameObject _prefab = freeGiftPanel.ItemPanel.GetChild(0).gameObject;
                for (int i = 1; i < freeData.Length; i++)
                {
                    Instantiate(_prefab, freeGiftPanel.ItemPanel.transform);
                }
                SetGridLayoutCellSize(freeData.Length, freeGiftPanel.ItemPanel.GetComponent<GridLayoutGroup>());
            }

            // 设置预制体
            for (int i = 0; i < freeData.Length; i++)
            {
                var itemImg = freeGiftPanel.ItemPanel.GetChild(i).GetComponent<Image>();
                var itemNumber = freeGiftPanel.ItemPanel.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>();
                // 是宝箱
                if (bPModel.BPDate.Rewards[level].FreeIsBox)
                {
                    itemImg.sprite = boxImgs[level % boxImgs.Length % 2];
                    break;
                }
                else
                {
                    // 头像特殊处理
                    if (freeData[i].itemType == "AvatarId")
                    {
                        Debug.Log("头像,待头像管理器补充");
                    }
                    else
                    {

                        if (rewardSprite.GetRewardSprite(freeData[i].itemType) != null)
                            itemImg.sprite = rewardSprite.GetRewardSprite(freeData[i].itemType);
                        else
                            itemImg.sprite = CoinsImg;
                        itemNumber.Show();
                        SpecialRewardsType _rewardEnum1;

                        if (Enum.TryParse<SpecialRewardsType>(freeData[i].itemType, out _rewardEnum1))
                        {
                            itemNumber.text = freeData[i].itemQuantity.ToString() + "m";
                        }
                        else
                        {
                            itemNumber.text = "x" + freeData[i].itemQuantity.ToString();
                        }
                    }
                }
            }
            #endregion

            #region 设置vipGift
            if (bPModel.IsVip)
            {
                vipGiftPanel.ImgLock.Hide();
                if (bPModel.VipRewardGorLevel > level)
                {
                    vipGiftPanel.ImgAlReceive.Show();
                }
                else
                {
                    vipGiftPanel.ImgAlReceive.Hide();
                    if (level < bPModel.RewardLevel)
                    {
                        vipGiftPanel.BtnClaim.Show();
                        var _vipGiftPanle = vipGiftPanel;
                        vipGiftPanel.BtnClaim.onClick.RemoveAllListeners();
                        vipGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(vipData, _vipGiftPanle, true));
                    }
                    else
                    {
                        vipGiftPanel.BtnClaim.Hide();
                    }
                }
            }
            else
            {
                vipGiftPanel.ImgLock.Show();
                vipGiftPanel.ImgAlReceive.Hide();
                vipGiftPanel.BtnClaim.Hide();
            }



            // 创建预制体
            if (!bPModel.BPDate.Rewards[level].VipIsBox)
            {
                for (int i = 1; i < vipData.Length; i++)
                {
                    GameObject _prefab = vipGiftPanel.ItemPanel.GetChild(0).gameObject;
                    Instantiate(_prefab, vipGiftPanel.ItemPanel.transform);
                }
                SetGridLayoutCellSize(vipData.Length, vipGiftPanel.ItemPanel.GetComponent<GridLayoutGroup>());
            }

            // 设置预制体
            for (int i = 0; i < vipData.Length; i++)
            {
                var itemImg = vipGiftPanel.ItemPanel.GetChild(i).GetComponent<Image>();
                var itemNumber = vipGiftPanel.ItemPanel.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>();
                // 是宝箱
                if (bPModel.BPDate.Rewards[level].VipIsBox)
                {
                    itemImg.sprite = boxImgs[level % boxImgs.Length % 2 + 2];
                    break;
                }
                else
                {
                    // 头像特殊处理
                    if (vipData[i].itemType == "AvatarId")
                    {
                        Debug.Log("头像,待头像管理器补充");
                    }
                    else
                    {
                        if (rewardSprite.GetRewardSprite(vipData[i].itemType) != null)
                            itemImg.sprite = rewardSprite.GetRewardSprite(vipData[i].itemType);
                        else
                            itemImg.sprite = CoinsImg;
                        itemNumber.Show();
                        SpecialRewardsType _rewardEnum1;
                        if (Enum.TryParse<SpecialRewardsType>(vipData[i].itemType, out _rewardEnum1))
                        {
                            itemNumber.text = vipData[i].itemQuantity.ToString() + "m";
                        }
                        else
                        {
                            itemNumber.text = "x" + vipData[i].itemQuantity.ToString();
                        }
                    }
                }
            }
            #endregion
        }
        public void SetGridLayoutCellSize(int count, GridLayoutGroup layout)
        {
            layout.cellSize = count switch
            {
                1 => new Vector2(100, 100),
                2 => new Vector2(80, 80),
                3 => new Vector2(73, 73),
                4 => new Vector2(65, 65),
                _ => new Vector2(100, 100)
            };
        }
        public void SetBtnOnClike(RewardItem[] rewardItem, GiftPanel giftPanel, bool isVipPack)
        {
            mBattlePassADActivity.DistributeReward(rewardItem, isVipPack);
            giftPanel.BtnClaim.Hide();
            giftPanel.ImgAlReceive.Show();
        }

        public Tween UpdateUI(int level, float duration)
        { 
            Tween fillTween = ImgProgressBar.DOFillAmount(1, duration)
                    .SetEase(Ease.Linear)
                    .Pause()
                    .OnStepComplete(() =>
                    {
                        // 设置进度条图片
                        
                       
                    });
            ImgLevel.sprite = levelImgs[1];
            return fillTween;
        }
        public void UpdateUI(int level)
        {
            // 获取内容
            RewardItem[] freeData = bPModel.BPDate.Rewards[level].Free;
            RewardItem[] vipData = bPModel.BPDate.Rewards[level].Vip;
            var freeGiftPanel = ImgGiftFree.GetComponent<GiftPanel>();
            var vipGiftPanel = ImgGiftVip.GetComponent<GiftPanel>();   

            #region 设置freeGift
            freeGiftPanel.ImgAlReceive.Hide();
            if (level < bPModel.RewardLevel)
            {
                freeGiftPanel.BtnClaim.Show();
                var _freeGiftPanel = freeGiftPanel;
                freeGiftPanel.BtnClaim.onClick.RemoveAllListeners();
                freeGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(freeData, _freeGiftPanel, false));
            }
            else
            {
                freeGiftPanel.BtnClaim.Hide();
            }
            #endregion

            #region 设置vipGift
            if (bPModel.IsVip)
            {
                vipGiftPanel.ImgLock.Hide();
                if (bPModel.VipRewardGorLevel > level)
                {
                    vipGiftPanel.ImgAlReceive.Show();
                }
                else
                {
                    vipGiftPanel.ImgAlReceive.Hide();
                    if (level < bPModel.RewardLevel)
                    {
                        vipGiftPanel.BtnClaim.Show();
                        var _vipGiftPanle = vipGiftPanel;
                        vipGiftPanel.BtnClaim.onClick.RemoveAllListeners();
                        vipGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(vipData, _vipGiftPanle, true));
                    }
                    else
                    {
                        vipGiftPanel.BtnClaim.Hide();
                    }
                }
            }
            else
            {
                vipGiftPanel.ImgLock.Show();
                vipGiftPanel.ImgAlReceive.Hide();
                vipGiftPanel.BtnClaim.Hide();
            }
            #endregion
        }
        public void SetDividingLine(int endValue)
        {
            ImgTopDividingLine.DOFillAmount(endValue, 1);
            ImgButtomDividingLine.DOFillAmount(endValue, 1);
        }
    }
}
