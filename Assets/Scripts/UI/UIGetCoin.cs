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
        private GameGlobalModel gameGlobalModel;
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

        private readonly int[] UNLOCKLEVEL = new int[]
            { 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 121, 141, 161, 181, 201, 301, 401 };

        //17关开始，每两关一个广告，50关之后，每关一个
        private const int START_AD_LIMIT = 17;
        private const int START_AD_LIMIT2 = 31;
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

            gameGlobalModel = this.GetModel<GameGlobalModel>();
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
            // ��ҵı������������ı���ʱ���Ž�������Ķ���
            if (gameGlobalModel.GoldCoinsMultiple != 1.0f && !isHaveBox)
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
            gameGlobalModel = null;
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

            if (curLevel > START_AD_LIMIT)
            {
                if (curLevel > START_AD_LIMIT2)
                    TopOnADManager.Instance.ShowIntersAd(null, null);

                else if (curLevel % 2 == 0)
                    TopOnADManager.Instance.ShowIntersAd(null, null);
            }
        }

        private void UpdateBoxProcessNode()
        {
            //���غ���¼��ǰ�ؿ�Ϊ��һ��(��һ��ʾͨ���Ĺؿ�)
            int curLevel = saveDataUtility.GetCurrentLevel() - 1;

            TxtCoin.text = GameDefine.GameConst.WIN_COINS.ToString();
            TxtLevel.text = "Level " + curLevel.ToString();
            //6-97����ʾ(ͨ��97��֮����ʾ)
            if (curLevel >= STAR_LEVEL && curLevel < END_LEVEL)
            {
                ImgBoxProcessNode.Show();
                int _progress = (curLevel - STAR_LEVEL + 1) % REWARD_INTERVAL;
                int _displayedProgress = _progress == 0 ? REWARD_INTERVAL : _progress;
                TxtProcess.text = $"{_displayedProgress} / {REWARD_INTERVAL}";
                // ��ʼ��������
                int _startValue = _displayedProgress - 1;
                ImgProcess.fillAmount = (float)_startValue / REWARD_INTERVAL;

                ActionKit.Delay(0.1f, () =>
                {
                    float targetValue = (float)_displayedProgress / REWARD_INTERVAL;
                    mProgeressTween1 = ImgProcess.DOFillAmount(targetValue, 0.5f).SetEase(Ease.OutQuad);
                }).Start(this);

                if (_progress == 0)
                {
                    getReward = ((curLevel - STAR_LEVEL + 1) / REWARD_INTERVAL) - 1;//��һ��������
                    if (getReward >= 0 && getReward < rewardPackSO.Length)
                    {
                        var _packSO = rewardPackSO[getReward];
                        rewardGrantUtility.GrantReward(_packSO);
                        isHaveBox = true;
                        // ��ҵı������������ı���ʱ���Ž�������Ķ���
                        if (gameGlobalModel.GoldCoinsMultiple != 1.0f)
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

            // �ҵ���һ������Ŀ��
            for (int i = 0; i < UNLOCKLEVEL.Length; i++)
            {
                if (curLevel <= UNLOCKLEVEL[i])
                {
                    ImgUnlockProcessNode.Show();
                    ImgUnlock.sprite = unlockSprites[i];
                    ImgUnlock.SetNativeSize();

                    int prevUnlock = (i == 0) ? 0 : UNLOCKLEVEL[i - 1]; // ��һ��������
                    int totalNeeded = UNLOCKLEVEL[i] - prevUnlock;      // ��Ҫ��ɵĹؿ���
                    int currentProgress = curLevel - prevUnlock;        // ��ǰ����

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
            // ���л����ѽ��������ؽ���UI
            ImgUnlockProcessNode.Hide();
        }

        private void PLayGoldCoinUPAnimation()
        {
            TextTimes.Show();
            TextTimes.text = "X" + (gameGlobalModel.GoldCoinsMultiple).ToString("0.0");
            mSequence = DOTween.Sequence();
            float duration = 0.6f;
            int currentValue = (int)GameDefine.GameConst.WIN_COINS;

            mSequence.Append(DOTween.To(() => 1f, alpha => {
                TextTimes.alpha = alpha; // �ֶ�����͸����
            }, 0.2f, duration));

            mSequence.Join(DOTween.To(() => Vector3.one, scale => {
                TextTimes.transform.localScale = scale; // �ֶ���������
            }, new Vector3(0.2f, 0.2f, 0.2f), duration));
            mSequence.OnComplete(() => {
                DOTween.To(() => currentValue, x =>
                {
                    TextTimes.Hide();
                    currentValue = x;
                    TxtCoin.text = currentValue.ToString();
                }, (int)(currentValue * gameGlobalModel.GoldCoinsMultiple), duration * 1.2f);
            });
            mSequence.Play();

        }
    }
}