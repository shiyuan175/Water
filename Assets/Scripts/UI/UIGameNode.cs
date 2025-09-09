using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using GameDefine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine;

namespace QFramework.Example
{
    public class UIGameNodeData : UIPanelData
    {
    }

    public partial class UIGameNode : UIPanel, IController
    {
        [SerializeField] private Sprite[] mRankLevelSprites;

        private StageModel stageModel;
        private TierRankActivityModel mTierRankActivityModel;
        private const int WIN_STREAK_RANKLEVEL_INTERVAL = 5;
        private const int MAX_SPRITE_INDEX = 8;
        private const string CLEAR_BWATER_PARTICLE_PATH = "Prefab/BlackMaskItem";

        private int mCacheRankSpriteIndex;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGameNodeData ?? new UIGameNodeData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            //真机模式下，AssetBundle 加载资源后需要关联材质
            TxtLevel.font.material.shader = Shader.Find(TxtLevel.font.material.shader.name);
            TxtLevel.text = LevelManager.Instance.levelId.ToString();
            stageModel = this.GetModel<StageModel>();
            mTierRankActivityModel = this.GetModel<TierRankActivityModel>();

            BindBtn();
            RegisterEvent();
        }

        protected override void OnShow()
        {
            if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() <= 5)
                BtnReturn.Hide();

            InitRankLevel();
            SetTakeItem();
            SetItem();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            stageModel = null;
            BtnStepBack.onClick.RemoveAllListeners();
            BtnRemoveHide.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnHalfBottle.onClick.RemoveAllListeners();
            BtnRemoveAll.onClick.RemoveAllListeners();
            BtnReturn.onClick.RemoveAllListeners();
            BtnItem1.onClick.RemoveAllListeners();
            BtnItem2.onClick.RemoveAllListeners();
            BtnItem3.onClick.RemoveAllListeners();
        }

        private void InitRankLevel()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            if (level == GameConst.WIN_STREAK_BEGIN_LEVEL)
            {
                ImgRankLevel_Label.Show();
                ActionKit.Delay(5f, () =>
                {
                    ImgRankLevel_Label.Hide();

                }).Start(this);
            }
            else ImgRankLevel_Label.Hide();

            if (level >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            {
                ImgRankLevel.Show();
                TxtRankLevel.text = mTierRankActivityModel.StreakWinNum.ToString();
                mCacheRankSpriteIndex = Mathf.Max(0, (mTierRankActivityModel.StreakWinNum - 1) / WIN_STREAK_RANKLEVEL_INTERVAL);
                mCacheRankSpriteIndex = mCacheRankSpriteIndex >= MAX_SPRITE_INDEX ? MAX_SPRITE_INDEX : mCacheRankSpriteIndex;
                ImgRankLevel.sprite = mRankLevelSprites[mCacheRankSpriteIndex];
            }
            else ImgRankLevel.Hide();
        }

        private void BindBtn()
        {
            BtnStepBack.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    if (stageModel.ItemDic[1] <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 1 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    if (LevelManager.Instance.ReturnLast())
                        stageModel.ReduceItem(1, 1);
                }
            });

            BtnRemoveHide.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    if (stageModel.ItemDic[2] <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 2 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    //判断是否有黑水瓶
                    if (LevelManager.Instance.hideBottleList.Count != 0)
                    {
                        LevelManager.Instance.RemoveHide(() =>
                        {
                            stageModel.ReduceItem(2, 1);
                        });
                    }
                }
            });

            BtnAddBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    if (stageModel.ItemDic[3] <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 3 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(false, () =>
                    {
                        stageModel.ReduceItem(3, 1);
                    });
                }
            });

            BtnHalfBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    if (stageModel.ItemDic[4] <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 4 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(true, () =>
                    {
                        stageModel.ReduceItem(4, 1);
                    });
                }
            });

            BtnRemoveAll.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    if (stageModel.ItemDic[5] <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 5 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    if (LevelManager.Instance.CheckAllDebuff())
                    {
                        LevelManager.Instance.RemoveAll(() =>
                        {
                            //清空操作记录的障碍(避免回退恢复)
                            foreach (var bottle in LevelManager.Instance.nowBottles)
                            {
                                foreach (var record in bottle.moveRecords)
                                {
                                    record.isFreeze = false;
                                    record.isClearHide = false;
                                    record.isNearHide = false;
                                    record.limitColor = 0;

                                    for (int i = 0; i < record.hideWaters.Count; i++)
                                    {
                                        record.hideWaters[i] = false;
                                    }

                                    for (int i = 0; i < record.waterItems.Count; i++)
                                    {
                                        record.waterItems[i] = WaterItem.None;
                                    }

                                    for (int i = 0; i < record.bombCount.Count; i++)
                                    {
                                        record.bombCount[i] = 0;
                                    }
                                }
                            }
                        });
                        stageModel.ReduceItem(5, 1);
                    }
                }
            });

            BtnReturn.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIRetry>();
            });

            BtnItem1.onClick.AddListener(() =>
            {
                UseItem(6, BtnItem1);
            });
            BtnItem2.onClick.AddListener(() =>
            {
                UseItem(7, BtnItem2);
            });
            BtnItem3.onClick.AddListener(() =>
            {
                LevelManager.Instance.ShowItemSelect();
                GameCtrl.Instance.SeletedItem(bottele => { UseItem(8, BtnItem3, bottele); });
            });
        }

        private void RegisterEvent()
        {
            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                SetItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LevelStartEvent>(e =>
            {
                SetTakeItem();
                TxtLevel.text = LevelManager.Instance.levelId.ToString();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register("StreakWinItem", (int count) =>
            {
                ClearBottleBlackWater(count, false);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.VICTORY_EVENT, () =>
            {
                int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

                //未开启连胜功能
                if (level - 1 < GameConst.WIN_STREAK_BEGIN_LEVEL)
                {
                    OpenUIVictory();
                    return;
                }

                //飞星效果
                var _spriteIndex = Mathf.Max(0, (mTierRankActivityModel.StreakWinNum - 1) / WIN_STREAK_RANKLEVEL_INTERVAL);
                _spriteIndex = _spriteIndex >= MAX_SPRITE_INDEX ? MAX_SPRITE_INDEX : _spriteIndex;
                var _rankPromotion = _spriteIndex > mCacheRankSpriteIndex;
                FlightEffects.Show();
                FlightEffects.DOMove(ImgRankLevel.transform.position, 1f)
                .OnComplete(() =>
                {
                    TxtRankLevel.text = mTierRankActivityModel.StreakWinNum.ToString();

                    //段位无变化
                    if (!_rankPromotion)
                    {
                        OpenUIVictory();
                        return;
                    }

                    ImgRankSprite_mid.sprite = mRankLevelSprites[mCacheRankSpriteIndex];
                    ImgRankSprite_mid.SetNativeSize();
                    SpineRankPromotion.Show();

                    SpineRankPromotion.AnimationState.SetAnimation(0, "animation", false);

                    ActionKit.Delay(0.5f, () =>
                    {
                        ImgRankSprite_mid.sprite = mRankLevelSprites[_spriteIndex];
                        ImgRankSprite_mid.SetNativeSize();
                    }).Start(this);

                    SpineRankPromotion.AnimationState.Complete += (trackEntry) =>
                    {
                        SpineRankPromotion.Hide();


                        if (mTierRankActivityModel.CompareWithHistoryBestRank(_spriteIndex))
                        {
                            //Debug.Log("首次晋升段位");
                            CoinManager.Instance.AddCoin(300);
                            RewardUIManager.Instance.PlayRewardAnim(null, 300);

                            ActionKit.Delay(1.5f, () =>
                            {
                                OpenUIVictory();
                            }).Start(this);
                            return;
                        }
                        //奖励已经领取
                        OpenUIVictory();
                    };
                });

            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        #region 道具相关

        /// <summary>
        /// 使用携带道具
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemObj"></param>
        /// <param name="botter">作用与哪个瓶子(打乱水块道具传入)</param>
        void UseItem(int itemID, Button itemObj, BottleCtrl botter = null)
        {
            switch (itemID)
            {
                case 6:
                    LevelManager.Instance.AddBottle(true, () =>
                    {
                        if (CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN))//!HealthManager.Instance.UnLimitHp
                            stageModel.ReduceItem(6, 1);
                        TxtItem1.text = "0";
                    });
                    break;

                case 7:
                    if (!(LevelManager.Instance.hideBottleList.Count > 0))
                        return;
                    ClearBottleBlackWater(2, true, () =>
                    {
                        if (CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN))
                            stageModel.ReduceItem(7, 1);
                        TxtItem2.text = "0";
                    });
                    break;

                case 8:
                    // 索引列表用于随机洗牌
                    List<int> _indices = Enumerable.Range(0, botter.waters.Count).ToList();
                    do
                    {
                        for (int i = 0; i < _indices.Count; i++)
                        {
                            int randIndex = UnityEngine.Random.Range(i, _indices.Count);
                            (_indices[i], _indices[randIndex]) = (_indices[randIndex], _indices[i]);
                        }
                    }
                    while (Enumerable.SequenceEqual(_indices.Select(i => botter.waters[i]), botter.waters));

                    List<int> _newWaters = new List<int>();
                    List<bool> _newHideWater = new List<bool>();
                    List<WaterItem> _newWaterItems = new List<WaterItem>();
                    List<int> _newBombs = new List<int>();

                    foreach (int idx in _indices)
                    {
                        _newWaters.Add(botter.waters[idx]);
                        _newHideWater.Add(botter.hideWaters[idx]);
                        _newWaterItems.Add(botter.waterItems[idx]);
                        _newBombs.Add(botter.bombCounts[idx]);
                    }
                    // 替换原列表
                    botter.waters = _newWaters;
                    botter.hideWaters = _newHideWater;
                    botter.waterItems = _newWaterItems;
                    botter.bombCounts = _newBombs;

                    //修改水块颜色和切换道具位置
                    for (int i = 0; i < botter.waters.Count; i++)
                    {
                        var useColor = botter.waters[i] - 1;
                        if (useColor < 1000)
                            botter.waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor], i == botter.topIdx);
                        else
                            botter.waterImg[i].SetColorState((ItemType)botter.waters[i], LevelManager.Instance.ItemColor, i == botter.topIdx);
                    }

                    //修改水面位置，修改水面颜色并播放水面动画
                    botter.SetNowSpinePos(botter.waters.Count);
                    botter.PlaySpineWaitAnim();
                    botter.CheckWaterItem();
                    botter.UpdateBomb();
                    botter.SetHideShow(true);
                    LevelManager.Instance.HideItemSelect();

                    if (CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN))
                        stageModel.ReduceItem(8, 1);
                    TxtItem3.text = "0";
                    //Debug.Log("打乱顺序成功");
                    break;
            }

            //if (!CheckHaveItem(itemID))//调整为仅使用一次
            itemObj.interactable = false;
        }

        /// <summary>
        /// 祛除瓶中所有黑水
        /// </summary>
        /// <param name="count">祛除的瓶子数量</param>
        /// <param name="effctNow">是否立即生效</param>
        /// <param name="action">回调(道具使用时传入)</param>
        private void ClearBottleBlackWater(int count, bool useItem, Action action = null)
        {
            if (LevelManager.Instance.hideBottleList.Count > 0)
            {
                var tempList = new List<BottleCtrl>(LevelManager.Instance.hideBottleList);

                while (tempList.Count > count)
                {
                    int randIndex = UnityEngine.Random.Range(0, tempList.Count);
                    tempList.RemoveAt(randIndex);
                }

                if (useItem)
                    useItemClearBWater(tempList, action);
                else
                    StreaWinClearBWater(tempList, action);
            }
        }

        private void StreaWinClearBWater(List<BottleCtrl> tempList, Action action)
        {
            var _particle = Resources.Load(CLEAR_BWATER_PARTICLE_PATH);
            var _tempObj = Instantiate(_particle) as GameObject;
            //UIKit.OpenPanel<UIMask>(UILevel.PopUI);//遮罩
            _tempObj.transform.DOLocalMoveY(0, 0.8f);
            _tempObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f)
            .OnComplete(() =>
            {
                foreach (var item in tempList)
                {
                    item.StarSetHideShow();
                }
                action?.Invoke();
                Destroy(_tempObj);
            });
        }

        private void useItemClearBWater(List<BottleCtrl> tempList, Action action)
        {
            foreach (var item in tempList)
            {
                for (int i = 0; i < item.hideWaters.Count; i++)
                {
                    item.hideWaters[i] = false;
                }
                item.SetHideShow(true);
            }

            action?.Invoke();
        }

        /// <summary>
        /// 使用携带道具按钮事件
        /// </summary>
        /// 进入游戏/重置关卡调用
        void SetTakeItem()
        {
            var takeItems = LevelManager.Instance.takeItem;

            var buttons = new[] { BtnItem1, BtnItem2, BtnItem3 };
            var texts = new[] { TxtItem1, TxtItem2, TxtItem3 };
            var itemIds = new[] { 6, 7, 8 };

            for (int i = 0; i < itemIds.Length; i++)
            {
                int itemId = itemIds[i];

                bool active = (takeItems.Contains(itemId) && CheckHaveItem(itemId))
                    || !CountDownTimerManager.Instance.IsTimerFinished(GameDefine.GameConst.UNLIMIT_ITEM_SIGN);
                buttons[i].interactable = active;
                texts[i].text = active ? "1" : "0";
            }
        }

        /// <summary>
        /// 下方道具栏道具更新
        /// </summary>
        private void SetItem()
        {
            BtnAddStepBack.gameObject.SetActive(stageModel.ItemDic[1] <= 0);
            TxtRefreshNum.text = stageModel.ItemDic[1].ToString();

            BtnAddRemove.gameObject.SetActive(stageModel.ItemDic[2] <= 0);
            TxtRemoveHideNum.text = stageModel.ItemDic[2].ToString();

            BtnAddAddBottle.gameObject.SetActive(stageModel.ItemDic[3] <= 0);
            TxtAddBottleNum.text = stageModel.ItemDic[3].ToString();

            BtnAddHalfBottle.gameObject.SetActive(stageModel.ItemDic[4] <= 0);
            TxtAddHalfBottleNum.text = stageModel.ItemDic[4].ToString();

            BtnAddRemoveBottle.gameObject.SetActive(stageModel.ItemDic[5] <= 0);
            TxtRemoveAllNum.text = stageModel.ItemDic[5].ToString();
        }

        /// <summary>
        /// 检查是否拥有道具
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        bool CheckHaveItem(int itemID)
        {
            if (stageModel.ItemDic[itemID] > 0)
                return true;
            else return false;
        }

        #endregion


        private void OpenUIVictory()
        {
            UIKit.ClosePanel<UIMask>();
            AudioKit.PlaySound("resources://Audio/Victory");
            UIKit.OpenPanel<UIVictory>();
        }
    }
}
