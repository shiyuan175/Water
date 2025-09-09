using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

namespace QFramework.Example
{
    public class UIBeginSelectData : UIPanelData
    {
    }
    public partial class UIBeginSelect : UIPanel, ICanGetUtility, ICanSendEvent, ICanRegisterEvent, ICanGetModel
    {
        [SerializeField] private Sprite[] giftSprites;
        [SerializeField] private Button[] addItemBtns;
        [SerializeField] private Sprite[] imgBgSprites;
        [SerializeField] private Button[] selectBtns;
        [SerializeField] private GameObject[] selectImgs;
        [SerializeField] private TextMeshProUGUI[] itemNumTxts;

        [Header("consecutive_coin")]
        [SerializeField] private Image ImgCoinWinProcess;
        [SerializeField] private TextMeshProUGUI TxtCoinWinProgress;
        [SerializeField] private Image imgBk;
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
            int currentLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            if(currentLevel>10)
            {
                switch (currentLevel % 10)
                {
                    case 4:
                        imgBk.transform.GetComponent<Image>().sprite = imgBgSprites[1];
                        break;
                    case 9:
                        imgBk.transform.GetComponent<Image>().sprite = imgBgSprites[2];
                        break;
                    default:
                        imgBk.transform.GetComponent<Image>().sprite = imgBgSprites[0];
                        break;
                }
            }
            TxtLevelTitle.text = $"Level {currentLevel}";
        }

        protected override void OnShow()
        {
            stageModel = this.GetModel<StageModel>();
            StringEventSystem.Global.Send("ClearTakeItem");

            RigesterEvent();

            UpdateWinNum();
            UpdateItem();

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
                addItemBtns[i].onClick.AddListener(() =>
                {
                    if (CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN))
                        UIKit.OpenPanel<UIBuyItem>(UILevel.Common, new UIBuyItemData() { item = _itemId });
                });
            }

            for (int i = 0; i < selectBtns.Length; i++)
            {
                int _itemId = i + startID;
                var _tempIndex = i;

                selectBtns[i].onClick.AddListener(() =>
                {
                    if (stageModel.ItemDic[_itemId] > 0 && CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN)) 
                    {
                        var show = !selectImgs[_tempIndex].gameObject.activeSelf;
                        selectImgs[_tempIndex].gameObject.SetActive(show);
                        if (show)
                            AddItemIfNotExists(_itemId);
                        else
                            RemoveItemIfExists(_itemId);
                    }
                });
            }

            ImgReward.Hide();
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

        void RigesterEvent()
        {
            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                UpdateItem();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<UnlimtItemEvent>(e =>
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
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN))
            {
                UnLimitNode.Show();
                AddItemIfNotExists(6);
                AddItemIfNotExists(7);
                AddItemIfNotExists(8);

                return;
            }
            else
            {
                StringEventSystem.Global.Send("ClearTakeItem");
                UnLimitNode.Hide();
            }

            UpdateItemDisplay(stageModel.ItemDic[6], itemNumTxts[0], addItemBtns[0]);
            UpdateItemDisplay(stageModel.ItemDic[7], itemNumTxts[1], addItemBtns[1]);
            UpdateItemDisplay(stageModel.ItemDic[8], itemNumTxts[2], addItemBtns[2]);
        }

        /// <summary>
        /// 更新道具角标状态
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="txtItem"></param>
        /// <param name="btnAdd"></param>
        void UpdateItemDisplay(int itemCount, TextMeshProUGUI txtItem, Button btnAdd)
        {
            if (itemCount > 0)
            {
                btnAdd.Hide();
                txtItem.transform.parent.Show();
                txtItem.text = itemCount.ToString();
            }
        }
    }
}
