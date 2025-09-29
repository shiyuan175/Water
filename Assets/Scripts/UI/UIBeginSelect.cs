using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using TMPro;
using GameDefine;

namespace QFramework.Example
{
    public class UIBeginSelectData : UIPanelData
    {
    }
    public partial class UIBeginSelect : UIPanel, ICanGetUtility, ICanSendEvent, ICanRegisterEvent, ICanGetModel
    {
        [SerializeField] private Sprite[] giftSprites;
        [SerializeField] private Sprite[] itemSubIcon;
        [SerializeField] private Button[] addItemBtns;
        [SerializeField] private Sprite[] imgBgSprites;
        [SerializeField] private Button[] selectBtns;
        [SerializeField] private Button[] ItemGuideBtns;
        [SerializeField] private GameObject[] selectImgs;

        [Header("consecutive_coin")]
        [SerializeField] private Image ImgCoinWinProcess;
        [SerializeField] private TextMeshProUGUI TxtCoinWinProgress;
        [SerializeField] private Image ImgBg;
        private StageModel stageModel;

        private const int CONTINUE_WIN_NUM_ItemGift = 3;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIBeginSelectData ?? new UIBeginSelectData();

            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            TxtWinProcess.font = LevelManager.Instance.redFont;
            TextItemGuide.font = LevelManager.Instance.redFont;
            stageModel = this.GetModel<StageModel>();
            StringEventSystem.Global.Send("ClearTakeItem");
            InitUI();
            RigesterEvent();
            BindBtn();

            ImgReward.Hide();
            ContinueWinGoalNode.Hide();
        }

        protected override void OnShow()
        {
            UpdateWinNum();
            UpdateItem();
            SetGoldCoinBuffUI();
            CheckGuideLevel();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnClose.onClick.RemoveAllListeners();
            BtnStart.onClick.RemoveAllListeners();
            BtnInfo.onClick.RemoveAllListeners();

            foreach (var btn in selectBtns)
            {
                btn.onClick.RemoveAllListeners();
            }
            foreach (var btn in addItemBtns)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        private void InitUI()
        {
            TxtLevelTitle.font = LevelManager.Instance.redFont;
            BtnStart.transform.Find("Text").GetComponent<TextMeshProUGUI>().font = LevelManager.Instance.redFont;
            int currentLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            if (currentLevel > GameConst.LEVEL_TYPE_LAST_DIGIT)
            {
                switch (currentLevel % GameConst.LEVEL_TYPE_LAST_DIGIT)
                {
                    case (int)GameDefine.LevelHardType.Hard:
                        ImgBg.transform.GetComponent<Image>().sprite = imgBgSprites[1];
                        break;
                    case (int)GameDefine.LevelHardType.VeryHand:
                        ImgBg.transform.GetComponent<Image>().sprite = imgBgSprites[2];
                        break;
                    default:
                        ImgBg.transform.GetComponent<Image>().sprite = imgBgSprites[0];
                        break;
                }
            }
            TxtWinProcess.font = LevelManager.Instance.redFont;
            TxtLevelTitle.text = $"Level {currentLevel}";

            // 初始化默认上锁
            for (int i = 0; i < selectBtns.Length; i++)
            {
                Transform _transform = selectBtns[i].transform;

                _transform.Find("ImgLock").Find("TextOpenTip").GetComponent<TextMeshProUGUI>().font = LevelManager.Instance.redFont;
                _transform.Find("ImgLock").Show();
                addItemBtns[i].Hide();
            }

            BtnStart.transform.Find("Text").GetComponent<TextMeshProUGUI>().font = LevelManager.Instance.redFont;

            ContinueWinGoalNode.Hide();
        }

        private void BindBtn()
        {
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            BtnStart.onClick.AddListener(() =>
            {
                if (!HealthManager.Instance.HasHp && !HealthManager.Instance.UnLimitHp)
                {
                    UIKit.OpenPanel<UIMoreLife>();
                    return;
                }
                this.SendEvent<GameStartEvent>();
                GameCtrl.Instance.InitGameCtrl();
                CloseSelf();
            });

            BtnInfo.onClick.AddListener(() =>
            {
                ImgReward.gameObject.SetActive(!ImgReward.gameObject.activeSelf);
            });

            int startID = 6; //道具起始ID
            for (int i = 0; i < addItemBtns.Length; i++)
            {
                //闭包
                int _itemId = i + startID;
                var _rewardType = (SpecialRewardsType)_itemId;
                string _sign = GameEnum.GetDescription(_rewardType);
                addItemBtns[i].onClick.AddListener(() =>
                {
                    if (CountDownTimerManager.Instance.IsTimerFinished(_sign))
                        UIKit.OpenPanel<UIBuyItem>(UILevel.Common, new UIBuyItemData() { item = _itemId });
                });
            }

            for (int i = 0; i < selectBtns.Length; i++)
            {
                int _itemId = i + startID;
                var _rewardType = (SpecialRewardsType)_itemId;
                string _sign = GameEnum.GetDescription(_rewardType);
                var _tempIndex = i;
                selectBtns[i].onClick.AddListener(() =>
                {
                    if (stageModel.ItemDic[_itemId] > 0 && CountDownTimerManager.Instance.IsTimerFinished(_sign))
                    {               
                        bool show = addItemBtns[_tempIndex].transform.GetComponent<Image>().sprite != itemSubIcon[1];
                        if (show)
                        {
                            addItemBtns[_tempIndex].transform.GetComponent<Image>().sprite = itemSubIcon[1];
                            addItemBtns[_tempIndex].interactable = false;
                            addItemBtns[_tempIndex].transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                            /*UpdateItemDisplay(stageModel.ItemDic[6 + _tempIndex], addItemBtns[_tempIndex]);*/
                            AddItemIfNotExists(_itemId);
                        }
                        else
                        {
                            UpdateItemDisplay(stageModel.ItemDic[6 + _tempIndex], addItemBtns[_tempIndex]);
                            RemoveItemIfExists(_itemId);
                        }

                    }
                });
            }
        }

        void RigesterEvent()
        {
            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                UpdateItem();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        /// 携带道具
        /// </summary>
        /// <param name="itemId"></param>
        void AddItemIfNotExists(int itemId)
        {
            //避免重复入列
            if (!LevelManager.Instance.takeItem.Contains(itemId))
                LevelManager.Instance.takeItem.Add(itemId);
        }

        /// <summary>
        /// 移除携带的道具
        /// </summary>
        /// <param name="itemId"></param>
        void RemoveItemIfExists(int itemId)
        {
            //避免取消选中仍携带
            if (LevelManager.Instance.takeItem.Contains(itemId))
                LevelManager.Instance.takeItem.Remove(itemId);
        }

        /// <summary>
        /// 更新连胜相关显示
        /// </summary>
        void UpdateWinNum()
        {
            int _curWinNum = stageModel.CountinueWinNum;

            int _winNum_Gift = _curWinNum > CONTINUE_WIN_NUM_ItemGift ? CONTINUE_WIN_NUM_ItemGift : _curWinNum;
            int _winNum_Coin = _curWinNum > GameDefine.GameConst.CONTINUE_WIN_NUM_COIN ? GameDefine.GameConst.CONTINUE_WIN_NUM_COIN : _curWinNum;

            TxtProgress.text = $"{_winNum_Gift} / {CONTINUE_WIN_NUM_ItemGift}";
            ImgProgress.fillAmount = _winNum_Gift * 1f / CONTINUE_WIN_NUM_ItemGift;

            TxtCoinWinProgress.text = $"{_winNum_Coin}/{GameDefine.GameConst.CONTINUE_WIN_NUM_COIN}";
            //0.081f * 连胜次数 + 0.095f映射值(1-10连胜映射公式)
            ImgCoinWinProcess.fillAmount = 0.081f * _winNum_Coin + 0.095f;

            //0-3胜，更新图标
            if (_winNum_Gift == 0 || _winNum_Gift == 1)
            {
                ImgBox.sprite = giftSprites[0];
                return;
            }
            ImgBox.sprite = giftSprites[_winNum_Gift - 1];
        }

        /// <summary>
        /// 更新道具显示状态
        /// </summary>
        void UpdateItem()
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_AddOneBottle)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_AddOneBottle);
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_RemoveHide)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_RemoveHide);
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_ChangeWater)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_ChangeWater);

            //更新道具数量
            UpdateItemDisplay(stageModel.ItemDic[6], addItemBtns[0]);
            UpdateItemDisplay(stageModel.ItemDic[7], addItemBtns[1]);
            UpdateItemDisplay(stageModel.ItemDic[8], addItemBtns[2]);
        }

        /// <summary>
        /// 更新道具角标状态
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="txtItem"></param>
        /// <param name="btnAdd"></param>
        void UpdateItemDisplay(int itemCount, Button btnAdd)
        {
            btnAdd.Show();
            // 有物品红底，数字，不点击
            if (itemCount > 0)
            {
                btnAdd.transform.GetComponent<Image>().sprite = itemSubIcon[0];
                btnAdd.interactable = false;
                btnAdd.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemCount.ToString();
            }
            // 没有物品，数字可点击
            else
            {
                btnAdd.transform.GetComponent<Image>().sprite = itemSubIcon[2];
                btnAdd.interactable = true;
                btnAdd.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
            }

        }

        #region 引导动画相关
        /// <summary>
        /// 设置进关道具引导动画
        /// </summary>
        private void SetEnterPropsGuideUI()
        {

            foreach(var i in addItemBtns)
            {
                i.Hide();
            }
            ItemGuidePanel.gameObject.Show();
            int startID = 6;
            SpineHandleItem.AnimationState.SetAnimation(0, "animation", true);
            SpineHandleItem.GetComponent<RectTransform>().anchoredPosition = selectBtns[0].GetComponent<RectTransform>().anchoredPosition;
            for (int i = 0; i < ItemGuideBtns.Length; i++)
            {
                int _itemId = i + startID;
                var _rewardType = (SpecialRewardsType)_itemId;
                string _sign = GameEnum.GetDescription(_rewardType);
                var _tempIndex = i;
                ItemGuideBtns[i].GetComponent<RectTransform>().anchoredPosition = selectBtns[i].GetComponent<RectTransform>().anchoredPosition;
                ItemGuideBtns[i].onClick.AddListener(() =>
                { 
                    ItemGuidePanel.Hide();
                    // 先解锁 再处理按钮的事件
                    SetEnterPropsUnLockUI();
                    if (stageModel.ItemDic[_itemId] > 0 && CountDownTimerManager.Instance.IsTimerFinished(_sign))
                    {
                        bool show = addItemBtns[_tempIndex].transform.GetComponent<Image>().sprite != itemSubIcon[1];
                        if (show)
                        {
                            addItemBtns[_tempIndex].transform.GetComponent<Image>().sprite = itemSubIcon[1];
                            addItemBtns[_tempIndex].interactable = false;
                            addItemBtns[_tempIndex].transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                            /*UpdateItemDisplay(stageModel.ItemDic[6 + _tempIndex], addItemBtns[_tempIndex]);*/
                            AddItemIfNotExists(_itemId);
                        }
                        else
                        {
                            UpdateItemDisplay(stageModel.ItemDic[6 + _tempIndex], addItemBtns[_tempIndex]);
                            RemoveItemIfExists(_itemId);
                        }

                    }
                   
                });
            }

        }

        /// <summary>
        /// 解锁进关道具状态
        /// </summary>
        private void SetEnterPropsUnLockUI()
        {
            foreach (var i in selectBtns)
            {
                i.transform.Find("ImgLock").Hide();
                i.interactable = true;
            }

            UpdateItem();
        }
        private void SetEnterPropsLockUI()
        {
            UnLimitNode.Hide();
            foreach (var i in selectBtns)
                i.interactable = false;
            foreach (var i in addItemBtns)
                i.Hide();
        }
        private void SetGoldCoinGuideUI()
        {
            GoldCoinGuidePanel.gameObject.Show();
            SetGoldCoinUnClockUI();

            BtnGoldGuide.onClick.AddListener(() =>
            {
                GoldCoinGuidePanel.gameObject.Hide();
            });

        }

        private void SetGoldCoinUnClockUI()
        {
            ContinueWinGoalNode.gameObject.Show();
        }

        private void CheckGuideLevel()
        {
            int _level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            // 引导动画开关
            if (_level == (int)GameDefine.UnLockMechanism.EnterLevelSelectProps)
            {
                SetEnterPropsGuideUI();
            }
            if (_level > (int)GameDefine.UnLockMechanism.EnterLevelSelectProps)
            {
                SetEnterPropsUnLockUI();
            }
            else
            {
                SetEnterPropsLockUI();
            }
            if (_level == (int)GameDefine.UnLockMechanism.TimesGoldCoin)
            {
                SetGoldCoinGuideUI();
            }
            if (_level > (int)GameDefine.UnLockMechanism.TimesGoldCoin)
            {
                SetGoldCoinUnClockUI();
            }

            if (_level >= (int)GameDefine.UnLockMechanism.WinStreakBeginLevel)
                Mask.Hide();
            else
                Mask.Show();
        }
        #endregion

        private void SetGoldCoinBuffUI()
        {
            float coinBuff = stageModel.GoldCoinsMultiple;
            // 金币数量设置为整数
            TextGoldCoin.text = ((int)(20 * coinBuff)).ToString();

            // buff设置为一位小数
            TextGoldCoinBuff.text = coinBuff.ToString("0.0");
        }
    }
}
