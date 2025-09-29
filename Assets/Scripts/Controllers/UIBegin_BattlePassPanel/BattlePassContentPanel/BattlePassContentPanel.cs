using UnityEngine;
using QFramework;
using UnityEngine.Rendering;
using TMPro;
using JsonFileData;
using UnityEngine.UI;
using System;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class BattlePassContentPanel : ViewController,ICanGetModel
	{  
        [SerializeField] public Sprite[] boxImgs;
        [SerializeField] public Sprite[] levelImgs; // 0表示还不能，1表示能

        [SerializeField] private RewardSpriteMappingSO rewardSprite;
		private BattlePassADActivity mBattlePassADActivity;
        private BattlePassModel bPModel;

      
        public void Awake()
        {
            // 获取逻辑层，将奖励发放等功能转交给逻辑层处理
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();

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
            RewardItem[] freeData=bPModel.BPDate.Rewards[level].Free;
            RewardItem[] vipData = bPModel.BPDate.Rewards[level].Vip;

            var freeGiftPanel = ImgGiftFree.GetComponent<GiftPanel>();
            var vipGiftPanel = ImgGiftVip.GetComponent<GiftPanel>();

            // 设置进度条
            if (level <= bPModel.RewardLevel)
            {  
                ImgProgressBar.fillAmount = 1;
                ImgLevel.sprite = levelImgs[1];
            }
            else
            {
                ImgProgressBar.fillAmount = 0;
                ImgLevel.sprite = levelImgs[0];
            }

            #region 设置freeGift
            if (bPModel.FreeRewardGotLevel>=level)
            {
                freeGiftPanel.ImgAlReceive.Show();          
            }
            else
            {
                freeGiftPanel.ImgAlReceive.Hide();
                if (level<=bPModel.RewardLevel)
                {
                    freeGiftPanel.BtnClaim.Show();
                    freeGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(freeData));
                }
                else
                {
                    freeGiftPanel.BtnClaim.Hide();
                }              
            }

            // 创建预制体
            if (!bPModel.BPDate.Rewards[level].FreeIsBox)
            {
                for (int i = 1; i < freeData.Length; i++)
                {
                    GameObject _prefab = freeGiftPanel.ItemPanel.GetChild(0).gameObject;
                    Instantiate(_prefab, transform);
                }
            }

            // 设置预制体
            for(int i =0;i<freeData.Length;i++)
            {
                var itemImg = freeGiftPanel.ItemPanel.GetChild(i).GetComponent<Image>();
                var itemNumber = freeGiftPanel.ItemPanel.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>();
                // 是宝箱
                if (bPModel.BPDate.Rewards[level].FreeIsBox)
                {
                    itemImg.sprite = boxImgs[level%boxImgs.Length];
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
                        itemImg.sprite = rewardSprite.GetRewardSprite(freeData[i].itemType);
                        SpecialRewardsType _rewardEnum1;
                        if (Enum.TryParse<SpecialRewardsType>(freeData[i].itemType, out _rewardEnum1))
                        {
                            itemNumber.text = freeData[i].itemQuantity.ToString()+"m";
                        }
                        else
                        {
                            itemNumber.text = "x"+freeData[i].itemQuantity.ToString();
                        }
                    }              
                }
            }
            #endregion

            #region 设置vipGift
            if (bPModel.FreeRewardGotLevel >= level)
            {
                vipGiftPanel.ImgAlReceive.Show();
            }
            else
            {
                vipGiftPanel.ImgAlReceive.Hide();
                if (level <= bPModel.RewardLevel)
                {
                    vipGiftPanel.BtnClaim.Show();
                    vipGiftPanel.BtnClaim.onClick.AddListener(() => SetBtnOnClike(vipData));
                }
                else
                {
                    vipGiftPanel.BtnClaim.Hide();
                }
            }
            vipGiftPanel.BtnClaim.Hide();
            
            
            // 创建预制体
            if (!bPModel.BPDate.Rewards[level].FreeIsBox)
            {
                for (int i = 1; i < freeData.Length; i++)
                {
                    GameObject _prefab = freeGiftPanel.ItemPanel.GetChild(0).gameObject;
                    Instantiate(_prefab, transform);
                }
            }

            // 设置预制体
            for (int i = 0; i < freeData.Length; i++)
            {
                var itemImg = freeGiftPanel.ItemPanel.GetChild(i).GetComponent<Image>();
                var itemNumber = freeGiftPanel.ItemPanel.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>();
                // 是宝箱
                if (bPModel.BPDate.Rewards[level].FreeIsBox)
                {
                    itemImg.sprite = boxImgs[level % boxImgs.Length];
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
                        itemImg.sprite = rewardSprite.GetRewardSprite(freeData[i].itemType);
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
        }
        /// <summary>
        /// 设置按钮的点击事件
        /// </summary>
        /// <param name="freePack"></param>
        /// <param name="vipPack"></param>
        public void SetBtnOnClike(RewardItem[] freeReward)
        {
          /*  BtnFreeClaim.onClick.AddListener(() =>
            {
                mBattlePassADActivity.DistributeReward(freeReward, false);

            });
            BtnRechargeClaim.onClick.AddListener(() =>
            {
                mBattlePassADActivity.DistributeReward(vipReward, true);
            });*/
        }

        public void SetCanChangeUI(int lever, bool isAnmi =false)
		{
             
		}
        
    }
}
