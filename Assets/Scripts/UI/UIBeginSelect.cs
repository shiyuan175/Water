using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using TMPro;
using GameDefine;
using Unity.Mathematics;

namespace QFramework.Example
{
    public class UIBeginSelectData : UIPanelData
    {
    }
    public partial class UIBeginSelect : UIPanel, ICanGetUtility, ICanSendEvent, ICanRegisterEvent, ICanGetModel
    {
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
        [SerializeField] TextMeshProUGUI[] redTXT;
        private StageModel stageModel;

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
            foreach (var i in redTXT)
                i.font = LevelManager.Instance.redFont;
            stageModel = this.GetModel<StageModel>();
            StringEventSystem.Global.Send("ClearTakeItem");
            InitUI();
            RigesterEvent();
            BindBtn();

            ContinueWinGoalNode.Hide();
        }

        protected override void OnShow()
        {
            UpdateWinNum();
            UpdateItem();
            SetBuffUI();
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

        private void Update()
        {
            if (BuffTag.IsActive())
                TxtCoinBuffTimer.text = CountDownTimerManager.Instance.GetRemainingTimeText(GameConst.DOUBLE_COIN_SIGN);

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
                UIKit.OpenPanel<UIWinStreakRemoveHide>(UILevel.PopUI);
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
            //1.5倍金币连胜相关
            int _curCoinWinNum = stageModel.GoldCoinsMultipleStreakWinNum;
            int _winNum_Coin = math.min(_curCoinWinNum, GameDefine.GameConst.TEN_CONTINUE_WIN_NUM);
            TxtCoinWinProgress.text = $"{_winNum_Coin}/{GameDefine.GameConst.TEN_CONTINUE_WIN_NUM}";
            //0.081f * 连胜次数 + 0.095f映射值(1-10连胜映射公式)
            ImgCoinWinProcess.fillAmount = 0.081f * _winNum_Coin + 0.095f;

            //连胜去黑水相关
            int _curRemoveHideWinNum = stageModel.RemoveHideStreakWinNum;
            int _winNum_RemoveHide = math.min(_curRemoveHideWinNum, GameDefine.GameConst.TEN_CONTINUE_WIN_NUM);
            TxtProgress.text = $"{_winNum_RemoveHide} / {GameDefine.GameConst.TEN_CONTINUE_WIN_NUM}";
            ImgProgress.fillAmount = _winNum_RemoveHide * 1f / GameDefine.GameConst.TEN_CONTINUE_WIN_NUM;
        }

        /// <summary>
        /// 更新道具显示状态
        /// </summary>
        void UpdateItem()
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_AddOneHalfBottle)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_AddOneHalfBottle);
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_RemoveOneBottleHideWater)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_RemoveOneBottleHideWater);
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_RemoveOneDebuffBottle)))
                AddItemIfNotExists((int)SpecialRewardsType.Unlimited_S_RemoveOneDebuffBottle);

            //更新道具数量
            UpdateItemDisplay(stageModel.ItemDic[6], addItemBtns[0]);
            UpdateItemDisplay(stageModel.ItemDic[7], addItemBtns[1]);
            UpdateItemDisplay(stageModel.ItemDic[8], addItemBtns[2]);
        }

        /// <summary>
        /// 更新道具角标状态
        /// </summary>
        /// <param name="itemCount"></param>
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

        /// <summary>
        /// buff UI显示(目前只有双倍金币)
        /// </summary>
        private void SetBuffUI()
        {
            //后续如果是其他buff时长也要处理显示,每个buff的条件单独处理
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.DOUBLE_COIN_SIGN))
                BuffTag.Show();
            else 
                BuffTag.Hide();
        }

        #region 引导动画相关
        /// <summary>
        /// 设置进关道具引导动画
        /// </summary>
        private void SetEnterPropsGuideUI(UnLockMechanism type)
        {

            foreach(var i in addItemBtns)
            {
                i.Hide();
            }
            ItemGuidePanel.gameObject.Show();
            int startID = 6;
            SpineHandleItem.AnimationState.SetAnimation(0, "animation", true);
            int _tempIndex = 0;
            switch (type)
            {
                case UnLockMechanism.S_AddOneHalfBottle:
                    GuideBtnItem1.Show();
                    SpineHandleItem.GetComponent<RectTransform>().position = GuideBtnItem1.GetComponent<RectTransform>().position;
                    GuideBtnItem1.onClick.AddListener(() =>
                    {
                        SetEnterPropsUnLockUI(UnLockMechanism.S_AddOneHalfBottle);
                        ItemGuidePanel.Hide();
                        _tempIndex = 1;
                    });
                    break;
                case UnLockMechanism.S_RemoveOneBottleHideWater:
                    GuideBtnItem2.Show();
                    SpineHandleItem.GetComponent<RectTransform>().position = GuideBtnItem2.GetComponent<RectTransform>().position;
                    GuideBtnItem2.onClick.AddListener(() =>
                    {
                        SetEnterPropsUnLockUI(UnLockMechanism.S_RemoveOneBottleHideWater);
                        ItemGuidePanel.Hide();
                        _tempIndex = 2;
                    });
                    break;

                case UnLockMechanism.S_RemoveOneDebuffBottle:
                    GuideBtnItem3.Show();
                    SpineHandleItem.GetComponent<RectTransform>().position = GuideBtnItem3.GetComponent<RectTransform>().position;
                    GuideBtnItem3.onClick.AddListener(() =>
                    {
                        SetEnterPropsUnLockUI(UnLockMechanism.S_RemoveOneDebuffBottle);
                        ItemGuidePanel.Hide();
                        _tempIndex = 3;
                    });
                    break;
            }
            int _itemId = startID + _tempIndex;
            var _rewardType = (SpecialRewardsType)_itemId;
            string _sign = GameEnum.GetDescription(_rewardType);
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
        }

        /// <summary>
        /// 解锁进关道具状态
        /// </summary>
        private void SetEnterPropsUnLockUI(UnLockMechanism type)
        {
            Button _tempBtn;
            switch (type)
            {
                case UnLockMechanism.S_AddOneHalfBottle:
                    _tempBtn = selectBtns[0];
                    UnLimitItemNode1.Show();
                    break;

                case UnLockMechanism.S_RemoveOneBottleHideWater:
                    _tempBtn = selectBtns[1];
                    UnLimitItemNode2.Show();
                    break;

                case UnLockMechanism.S_RemoveOneDebuffBottle:
                    _tempBtn = selectBtns[2];
                    UnLimitItemNode3.Show();
                    break;
                default:
                    Debug.Log("按钮设置出错");
                    _tempBtn = null;
                    break;
            }

            _tempBtn.transform.Find("ImgLock").Hide();
            _tempBtn.interactable = true;

            UpdateItem();
        }

        private void SetEnterPropsLockUI(UnLockMechanism type)
        {
            switch (type)
            {
                case UnLockMechanism.S_AddOneHalfBottle:
                    selectBtns[0].interactable = false;
                    addItemBtns[0].Hide();
                    UnLimitItemNode1.Hide();
                    break;

                case UnLockMechanism.S_RemoveOneBottleHideWater:
                    selectBtns[1].interactable = false;
                    addItemBtns[1].Hide();
                    UnLimitItemNode2.Hide();
                    break;

                case UnLockMechanism.S_RemoveOneDebuffBottle:
                    selectBtns[2].interactable = false;
                    addItemBtns[2].Hide();
                    UnLimitItemNode3.Hide();
                    break;
            }
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
            // 彩色水
            if (_level == (int)GameDefine.UnLockMechanism.S_AddOneHalfBottle)
            {
                SetEnterPropsGuideUI(UnLockMechanism.S_AddOneHalfBottle);
            }

            if (_level > (int)GameDefine.UnLockMechanism.S_AddOneHalfBottle)
            {
                SetEnterPropsUnLockUI(UnLockMechanism.S_AddOneHalfBottle);
            }
            else
            {
                SetEnterPropsLockUI(UnLockMechanism.S_AddOneHalfBottle);
            }

            // 去黑
            if (_level == (int)GameDefine.UnLockMechanism.S_RemoveOneBottleHideWater)
            {
                SetEnterPropsGuideUI(UnLockMechanism.S_RemoveOneBottleHideWater);
            }

            if (_level > (int)GameDefine.UnLockMechanism.S_RemoveOneBottleHideWater)
            {
                SetEnterPropsUnLockUI(UnLockMechanism.S_RemoveOneBottleHideWater);
            }
            else
            {
                SetEnterPropsLockUI(UnLockMechanism.S_RemoveOneBottleHideWater);
            }

            // 移除一个瓶子负面
            if (_level == (int)GameDefine.UnLockMechanism.S_RemoveOneDebuffBottle)
            {
                SetEnterPropsGuideUI(UnLockMechanism.S_RemoveOneDebuffBottle);
            }

            if (_level > (int)GameDefine.UnLockMechanism.S_RemoveOneDebuffBottle)
            {
                SetEnterPropsUnLockUI(UnLockMechanism.S_RemoveOneDebuffBottle);
            }
            else
            {
                SetEnterPropsLockUI(UnLockMechanism.S_RemoveOneDebuffBottle);
            }

            //双倍金币
            if (_level == (int)GameDefine.UnLockMechanism.TimesGoldCoin)
            {
                SetGoldCoinGuideUI();
            }
            if (_level > (int)GameDefine.UnLockMechanism.TimesGoldCoin)
            {
                SetGoldCoinUnClockUI();
            }

            //连胜去黑
            if (_level >= (int)GameDefine.UnLockMechanism.RemoveHideWinStreakLevel)
                Mask.Hide();
            else
            {
                Mask.Show();
                Mask.GetComponentInChildren<TextMeshProUGUI>().font = LevelManager.Instance.redFont;
            }
        }
        #endregion
    }
}
