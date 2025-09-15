using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using GameDefine;
using TMPro;

namespace QFramework.Example
{
	public class UIGetCoinData : UIPanelData
	{
	}
	public partial class UIGetCoin : UIPanel, ICanSendEvent, ICanGetUtility, ICanGetModel
    {
        [SerializeField] private GiftPackSO[] rewardPackSO;
        [SerializeField] private Sprite[] unlockSprites;
        [SerializeField] private Sprite[] imgTipBgSprites;
        [SerializeField] private Image imgTipBg;
        private StageModel stageModel;
        private RewardGrantUtility rewardGrantUtility;
        private SaveDataUtility saveDataUtility;
        private int getReward;

        private const int STAR_LEVEL = 6;
        private const int END_LEVEL = 97;
        private const int REWARD_INTERVAL = 7;

        private readonly int[] UNLOCKLEVEL = new int[] { 7, 11, 21, 31, 41, 51, 61, 71, 81 };

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGetCoinData ?? new UIGetCoinData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtContinue.font = LevelManager.Instance.greenFont;
            TxtLevel.font = LevelManager.Instance.blueFont;
            TxtProcess.font = LevelManager.Instance.blueFont;
            TxtUnlockProcess.font = LevelManager.Instance.blueFont;

            stageModel = this.GetModel<StageModel>();
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            saveDataUtility = this.GetUtility<SaveDataUtility>();
        }

        protected override void OnShow()
        {
            BindClick();
            getReward = -1;
            UpdateBgUI();
            UpdateBoxProcessNode();
            UpdateUnlockProcessNode();
            
        }
		protected void UpdateBgUI()
        {
         
            
            int currentLevel = saveDataUtility.GetCurrentLevel()-1;
            
            if (currentLevel < 10)
                return ;
        
            switch(currentLevel%10)
            {
                case 4:
                    imgTipBg.sprite = imgTipBgSprites[1];
                    break;
                case 9:
                    imgTipBg.sprite = imgTipBgSprites[2];
                    break;
                default:
                    imgTipBg.sprite = imgTipBgSprites[0];
                    break;
            }
        }
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            stageModel = null;
            saveDataUtility = null;
            rewardGrantUtility = null;
            BtnClose.onClick.RemoveAllListeners();
            BtnContinue.onClick.RemoveAllListeners();
        }

		void BindClick()
		{
            BtnClose.onClick.AddListener(() =>
            {
                BackUIBegin();
            });

            BtnContinue.onClick.AddListener(() =>
            {
                BackUIBegin();
            });
        }

        void BackUIBegin()
        {
            UIKit.ClosePanel<UIGameNode>();
            this.SendEvent<LevelClearEvent>(new LevelClearEvent());
            CloseSelf();
        }

        private void UpdateBoxProcessNode()
        {
            //过关后会记录当前关卡为下一关(减一表示通过的关卡)
            int curLevel = saveDataUtility.GetCurrentLevel() - 1;
            //6-97关显示(通过97关之后不显示)
            if (curLevel >= STAR_LEVEL && curLevel < END_LEVEL)
            {
                ImgBoxProcessNode.Show();
                int _progress = (curLevel - STAR_LEVEL + 1) % REWARD_INTERVAL;
                int _displayedProgress = _progress == 0 ? REWARD_INTERVAL : _progress;
                TxtProcess.text = $"{_displayedProgress} / {REWARD_INTERVAL}";
                // 初始化进度条
                int _startValue = _displayedProgress - 1;
                ImgProcess.fillAmount = (float)_startValue / REWARD_INTERVAL;

                ActionKit.Delay(0.1f, () =>
                {
                    float targetValue = (float)_displayedProgress / REWARD_INTERVAL;
                    ImgProcess.DOFillAmount(targetValue, 0.5f).SetEase(Ease.OutQuad);
                }).Start(this);

                if (_progress == 0)
                {
                    getReward = ((curLevel - STAR_LEVEL + 1) / REWARD_INTERVAL) - 1;//减一计算索引
                    if (getReward >= 0 && getReward < rewardPackSO.Length)
                    {
                        var _packSO = rewardPackSO[getReward];
                        rewardGrantUtility.GrantReward(_packSO);
                        RewardUIManager.Instance.PlayRewardAnim(_packSO.Coins, packSOs: _packSO);
                    }
                }
            }
            else
                ImgBoxProcessNode.Hide();

            TxtCoin.text = ((int)(GameDefine.GameConst.WIN_COINS * stageModel.GoldCoinsMultiple)).ToString();
            TxtLevel.text = "Level " + curLevel.ToString();
        }

        private void UpdateUnlockProcessNode()
        {
            int curLevel = saveDataUtility.GetCurrentLevel();

            // 找到下一个解锁目标
            for (int i = 0; i < UNLOCKLEVEL.Length; i++)
            {
                if (curLevel <= UNLOCKLEVEL[i])
                {
                    ImgUnlockProcessNode.Show();
                    ImgUnlock.sprite = unlockSprites[i];

                    int prevUnlock = (i == 0) ? 0 : UNLOCKLEVEL[i - 1]; // 上一个解锁点
                    int totalNeeded = UNLOCKLEVEL[i] - prevUnlock;      // 需要完成的关卡数
                    int currentProgress = curLevel - prevUnlock;        // 当前进度

                    if (currentProgress > totalNeeded)
                        currentProgress = totalNeeded;

                    TxtUnlockProcess.text = $"{currentProgress} / {totalNeeded}";

                    int startValue = currentProgress - 1;
                    ImgUnlockProcess.fillAmount = (float)startValue / totalNeeded; ;

                    ActionKit.Delay(0.1f, () =>
                    {
                        float targetValue = (float)currentProgress / totalNeeded;
                        ImgUnlockProcess.DOFillAmount(targetValue, 0.5f).SetEase(Ease.OutQuad);
                        //ImgUnlockProcess.fillAmount = (float)currentProgress / totalNeeded;
                    }).Start(this);
                   

                    return;
                }
            }

            // 所有机制已解锁，隐藏解锁UI
            ImgUnlockProcessNode.Hide();
        }
    }
}
