using DG.Tweening;
using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        private Sequence mSequence;
        private Tween mProgeressTween1;
        private Tween mProgeressTween2;


        private int getReward;
        private bool isHaveBox = false;

        private const int STAR_LEVEL = 6;
        private const int END_LEVEL = 97;
        private const int REWARD_INTERVAL = 7;

        private readonly int[] UNLOCKLEVEL = new int[] {11, 21, 31, 41, 51, 61, 71, 81 };

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

            UnlockNewItem();
            BindClick();
            getReward = -1;
            UpdateBgUI();
            UpdateBoxProcessNode();
            UpdateUnlockProcessNode();
        }

        protected override void OnShow()
        {
            // 金币的倍数不是正常的倍数时播放金币增长的动画
            if (stageModel.GoldCoinsMultiple != 1.0f && !isHaveBox)
            {
                PLayGoldCoinUPAnimation();
            }
        }

        protected void UpdateBgUI()
        {
            int currentLevel = saveDataUtility.GetCurrentLevel() - 1;

            if (currentLevel < GameConst.LEVEL_TYPE_BEGIN_LEVEL)
                return;

            switch (currentLevel % GameConst.LEVEL_TYPE_LAST_DIGIT)
            {
                case (int)GameDefine.LevelHardType.Hard:
                    imgTipBg.sprite = imgTipBgSprites[1];
                    break;

                case (int)GameDefine.LevelHardType.VeryHand:
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
            mSequence?.Kill();
            mProgeressTween1?.Kill();
            mProgeressTween2?.Kill();
            mProgeressTween1 = null;
            mProgeressTween2 = null;
            mSequence = null;
            stageModel = null;
            saveDataUtility = null;
            rewardGrantUtility = null;
            BtnClose.onClick.RemoveAllListeners();
            BtnContinue.onClick.RemoveAllListeners();
        }

        private void BindClick()
        {
            BtnClose.onClick.AddListener(() =>
            {
                BackUIBegin();
            });

            BtnContinue.onClick.AddListener(() =>
            {
                BackUIBegin();
            });

            BtnNewItemClose.onClick.AddListener(() =>
            {
                NewItemNode.Hide();
            });
        }

        private void BackUIBegin()
        {
            UIKit.ClosePanel<UIGameNode>();
            this.SendEvent(new ReturnToMainEvent { PassLevel = true });
            CloseSelf();
        }

        private void UnlockNewItem()
        {
            int curLevel = saveDataUtility.GetCurrentLevel();
            if (UNLOCKLEVEL.Contains(curLevel))
            {
                int _idx = Array.IndexOf(UNLOCKLEVEL, curLevel);
                NewItemNode.Show();

                TxtNewItem_Red.font = LevelManager.Instance.redFont;

                TxtNewItem_Red.text = GameDefine.GameConst.GameplayTutorialInfo[curLevel].guideInfo;
                ImgNewItem.sprite = unlockSprites[_idx];
                ImgNewItem.SetNativeSize();

                BtnNewItemClose.onClick.AddListener(()=> NewItemNode.Hide());
            }
            else
                NewItemNode.Hide();
        }

        private void UpdateBoxProcessNode()
        {
            //过关后会记录当前关卡为下一关(减一表示通过的关卡)
            int curLevel = saveDataUtility.GetCurrentLevel() - 1;

            TxtCoin.text = GameDefine.GameConst.WIN_COINS.ToString();
            TxtLevel.text = "Level " + curLevel.ToString();
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
                    mProgeressTween1 = ImgProcess.DOFillAmount(targetValue, 0.5f).SetEase(Ease.OutQuad);
                }).Start(this);

                if (_progress == 0)
                {
                    getReward = ((curLevel - STAR_LEVEL + 1) / REWARD_INTERVAL) - 1;//减一计算索引
                    if (getReward >= 0 && getReward < rewardPackSO.Length)
                    {
                        var _packSO = rewardPackSO[getReward];
                        rewardGrantUtility.GrantReward(_packSO);
                        isHaveBox = true;
                        // 金币的倍数不是正常的倍数时播放金币增长的动画
                        if (stageModel.GoldCoinsMultiple != 1.0f)
                            RewardUIManager.Instance.PlayRewardAnim(_packSO.Coins,false,PLayGoldCoinUPAnimation, packSOs: _packSO);
                        else
                            RewardUIManager.Instance.PlayRewardAnim(_packSO.Coins,false,null, packSOs: _packSO);
                    }
                }
            }
            else
                ImgBoxProcessNode.Hide();
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
                    ImgUnlock.SetNativeSize();

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
                        mProgeressTween2 = ImgUnlockProcess.DOFillAmount(targetValue, 0.5f).SetEase(Ease.OutQuad);
                        //ImgUnlockProcess.fillAmount = (float)currentProgress / totalNeeded;
                    }).Start(this);
                    return;
                }
            }
            // 所有机制已解锁，隐藏解锁UI
            ImgUnlockProcessNode.Hide();
        }

        private void PLayGoldCoinUPAnimation()
        {
            TextTimes.Show();
            TextTimes.text = "X" + (stageModel.GoldCoinsMultiple).ToString("0.0");
            mSequence = DOTween.Sequence();
            float duration = 0.6f;
            int currentValue = (int)GameDefine.GameConst.WIN_COINS;

            mSequence.Append(DOTween.To(() => 1f, alpha => {
                TextTimes.alpha = alpha; // 手动控制透明度
            }, 0.2f, duration));

            mSequence.Join(DOTween.To(() => Vector3.one, scale => {
                TextTimes.transform.localScale = scale; // 手动控制缩放
            }, new Vector3(0.2f, 0.2f, 0.2f), duration));
            mSequence.OnComplete(() => {
                DOTween.To(() => currentValue, x =>
                {
                    TextTimes.Hide();
                    currentValue = x;
                    TxtCoin.text = currentValue.ToString();
                }, (int)(currentValue * stageModel.GoldCoinsMultiple), duration * 1.2f);
            });
            mSequence.Play();

        }
    }
}