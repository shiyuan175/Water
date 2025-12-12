using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using GameDefine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.U2D;
using TMPro;


namespace QFramework.Example
{
    /// <summary>
    /// 游戏主界面数据类，用于传递全局机制和步数信息
    /// </summary>
    public class UIGameNodeData : UIPanelData
    {
        public GlobalMechanism GlobalMechanism;
        public int BeginSetp = 0;
        public int CanUseSetps = 0;
    }

    /// <summary>
    /// 游戏主界面UI控制器，负责管理游戏中的所有UI交互、道具系统、关卡进度等
    /// </summary>
    public partial class UIGameNode : UIPanel, IController
    {
        [SerializeField]
        private MagicCtrl magicCtrl;
        private const string ITEM_ENTRANCE_EFFECT_PATH = "Prefab/ItemEntranceEffect";
        private const int GET_THE_LAST_NUMBER_OF_LEVEL = 10;

        [Header("�ؿ��Ѷ�UI")]
        #region �ؿ��Ѷ�UI - 根据关卡难度动态切换的UI资源

        [SerializeField] private Sprite[] imgBtnItemBgSprites;
        [SerializeField] private Sprite[] imgTopBgSprites;
        [SerializeField] private Sprite[] imgBottomSpirtes;
        [SerializeField] private Sprite[] imgLevelSprites;
        [SerializeField] private Sprite[] imgBtnReturnSprites;
        [SerializeField] private Image[] imgBtnItemBg;
        [SerializeField] private Image imgBtnReturn;
        [SerializeField] private Image imgTopBg;
        [SerializeField] private Image imgBottom;
        [SerializeField] private Image imgLevel;

        #endregion

        [Header("ǰ��ع�������UI")]
        #region ǰ��ع�������UI - 显示前期新手关卡进度的UI元素
        [SerializeField] private GameObject g_Star_MagicBook_Guide;
        [SerializeField] private Image mImgProgressBar;
        [SerializeField] private TextMeshProUGUI mStoryTxt;
        #endregion

        [Header("���ѵ��߽���UI")]
        // 道具按钮上的文本显示（数量或解锁等级）
        [SerializeField] private TextMeshProUGUI mTxtAddHalfBottle;
        [SerializeField] private TextMeshProUGUI mTxtAddBottle;
        [SerializeField] private TextMeshProUGUI mTxtStepBack;
        [SerializeField] private TextMeshProUGUI mTxtRemoveHide;
        [SerializeField] private TextMeshProUGUI mTxtRemoveAll;


        // 资源加载器，用于加载图集等资源
        private ResLoader mResLoader;
        // 关卡模型，管理道具数据和游戏状态
        private StageModel stageModel;
        // 段位等级图集
        private SpriteAtlas mRankLevelSpriteAtlas;

        // 缓存的段位图标索引
        private int mCacheRankSpriteIndex;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        /// <summary>
        /// UI初始化，处理全局机制（魔法猫车）的初始化
        /// </summary>
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGameNodeData ?? new UIGameNodeData();
            #region ȫ�ֻ���--ħ��è�� - 如果是魔法猫车机制，初始化魔法控制器
            if (mData.GlobalMechanism == GlobalMechanism.WhiteMagicCar || mData.GlobalMechanism == GlobalMechanism.BlackMagicCar)
            {
                magicCtrl.Init(mData.GlobalMechanism);
            }
            #endregion
            // please add init code here
        }

        /// <summary>
        /// UI打开时调用，加载资源、绑定按钮、注册事件、消耗携带道具并自动使用
        /// </summary>
        protected override void OnOpen(IUIData uiData = null)
        {
            stageModel = this.GetModel<StageModel>();

            LoadRes();
            BindBtn();
            RegisterEvent();
            ConsumeTakeItems();

            AutoUseAllItems();
        }

        /// <summary>
        /// UI显示时调用，初始化各个UI模块（故事进度、段位、关卡难度、道具等）
        /// </summary>
        protected override void OnShow()
        {
            InitStoryUI();
            InitRankLevel();
            InitLevelUI();
            InitItemUI();
            SetItem();
        }

        /// <summary>
        /// UI隐藏时调用（当前为空实现）
        /// </summary>
        protected override void OnHide()
        {
        }

        /// <summary>
        /// UI关闭时调用，清理所有监听器和资源
        /// </summary>
        protected override void OnClose()
        {
            stageModel = null;
            BtnStepBack.onClick.RemoveAllListeners();
            BtnRemoveHide.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnHalfBottle.onClick.RemoveAllListeners();
            BtnRemoveAll.onClick.RemoveAllListeners();
            BtnReturn.onClick.RemoveAllListeners();

            if (mResLoader != null)
            {
                mResLoader.Recycle2Cache();
                mResLoader = null;
                mRankLevelSpriteAtlas = null;
            }
        }

        /// <summary>
        /// 加载资源，如果关卡达到段位系统开启等级，则加载段位图集
        /// </summary>
        private void LoadRes()
        {
            if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() >= GameDefine.GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                mResLoader = ResLoader.Allocate();
                mRankLevelSpriteAtlas = mResLoader.LoadSync<SpriteAtlas>
                    (ABResourceDefine.RANK_LEVEL_ATLAS_BUNDLENAME, ABResourceDefine.RANK_LEVEL_ATLAS_ASSETNAME);
            }
        }

        /// <summary>
        /// 绑定所有按钮的点击事件（返回按钮、各种道具按钮）
        /// </summary>
        private void BindBtn()
        {
            BtnReturn.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIRetry>();
            });

            BtnRemoveAll.onClick.AddListener(BtnRemoveAllOnClick);
            BtnAddBottle.onClick.AddListener(BtnAddBottleOnClick);
            BtnHalfBottle.onClick.AddListener(BtnHalfBottleOnClick);
            BtnRemoveHide.onClick.AddListener(BtnRemoveHideOnClick);
            BtnStepBack.onClick.AddListener(BtnSetpBackOnClick);
        }

        /// <summary>
        /// 注册游戏事件：道具刷新、道具解锁、关卡开始、胜利事件（包含段位晋升逻辑）
        /// </summary>
        private void RegisterEvent()
        {
            // 道具数量刷新事件
            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                SetItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 道具解锁事件
            this.RegisterEvent<UnLockItem>(e =>
            {
                UnLockItem(e.PropType);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 关卡开始事件，更新关卡编号和故事UI
            this.RegisterEvent<LevelStartEvent>(eventId =>
            {
                TxtLevel.text = LevelManager.Instance.levelId.ToString();
                InitStoryUI();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 胜利事件，处理段位系统和胜利界面
            StringEventSystem.Global.Register(GameConst.VICTORY_EVENT, () =>
            {
                int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
                //ԭ�ǵڰ˹ز���ʾ��λ(5~7��ֱ�ӷ���)
                //�����ǵ�������ʾ�������ͨ��ʱ�ᴥ������
                // 如果关卡未达到段位系统开启等级，直接打开胜利界面
                if (level - 1 < GameConst.IN_GAME_RANK_BEGIN_LEVEL)
                {
                    OpenUIVictory();
                    return;
                }

                var _tempWin = stageModel.InGameRankStreakWinNum;

                //����Ч�� - 播放飞行特效到段位图标位置
                var curRankIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                FlightEffects.Show();
                FlightEffects.DOMove(ImgRankLevel.transform.position, 1f)
                .OnComplete(() =>
                {
                    // 更新段位连胜数显示
                    TxtRankLevel.text = _tempWin.ToString();

                    //��λ�޽��� - 如果段位没有提升，直接打开胜利界面
                    if (curRankIndex <= mCacheRankSpriteIndex)
                    {
                        OpenUIVictory();
                        return;
                    }

                    // 播放段位晋升动画
                    ImgRankSprite_mid.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mCacheRankSpriteIndex));
                    ImgRankSprite_mid.SetNativeSize();
                    SpineRankPromotion.Show();

                    SpineRankPromotion.AnimationState.SetAnimation(0, "animation", false);

                    // 延迟0.5秒后切换到新段位图标
                    ActionKit.Delay(0.5f, () =>
                    {
                        ImgRankSprite_mid.sprite =
                        mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(curRankIndex));
                        ImgRankLevel.sprite =
                        mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(curRankIndex));

                        ImgRankSprite_mid.SetNativeSize();
                    }).Start(this);

                    // 动画完成后检查是否首次达到该段位
                    SpineRankPromotion.AnimationState.Complete += (trackEntry) =>
                    {
                        SpineRankPromotion.Hide();

                        if (stageModel.CompareWithHistoryBestRank(curRankIndex))
                        {
                            //Debug.Log("�״ν�����λ") - 首次达到该段位，奖励300金币
                            CoinManager.Instance.AddCoin(300);
                            RewardUIManager.Instance.PlayRewardAnim(300, true, null);

                            ActionKit.Delay(1.5f, () =>
                            {
                                OpenUIVictory();
                            }).Start(this);
                            return;
                        }
                        //�����Ѿ���ȡ - 已经获取过该段位奖励，直接打开胜利界面
                        OpenUIVictory();
                    };
                });

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        #region UI��ʼ�� - 各个UI模块的初始化方法

        /// <summary>
        /// ǰ��ع���UI - 初始化前期新手关卡的故事进度条UI（前15关）
        /// </summary>
        private void InitStoryUI()
        {
            int _curLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            // 超过新手关卡数量则不显示故事进度
            if (_curLevel > GameDefine.GameConst.NEWBIE_LEVEL_COUNT)
                return;

            // 首次显示，直接设置进度
            if (!g_Star_MagicBook_Guide.activeSelf)
            {
                g_Star_MagicBook_Guide.Show();
                mStoryTxt.font = LevelManager.Instance.greenFont;

                mImgProgressBar.fillAmount = (float)(_curLevel) / GameConst.NEWBIE_LEVEL_COUNT;
                mStoryTxt.text = $"Story {_curLevel}/{GameConst.NEWBIE_LEVEL_COUNT}";
            }
            else
            {
                // 已显示，使用动画更新进度条
                var fillamount = (float)(_curLevel) / GameConst.NEWBIE_LEVEL_COUNT;
                mImgProgressBar.DOFillAmount(fillamount, 1f);
                mStoryTxt.text = $"Story {_curLevel}/{GameConst.NEWBIE_LEVEL_COUNT}";
            }
        }

        /// <summary>
        /// �޸��Ѷ�UI
        /// </summary>
        private void InitLevelUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            TxtLevel.text = level.ToString();

            if (level <= 5)
                BtnReturn.Hide();

            if (level < GET_THE_LAST_NUMBER_OF_LEVEL)
                return;
            int _index = 0;
            switch (level % GET_THE_LAST_NUMBER_OF_LEVEL)
            {
                case (int)LevelHardType.Hard:
                    _index = 1;
                    break;

                case (int)LevelHardType.VeryHand:
                    _index = 2;
                    break;

                    // t��ʼ��Ϊ0������û����Defailtȡ0
            }
            if (_index != 0)
                SetTextTip();
            // ����ť�ı�����ɫ
            /*foreach (var i in imgBtnItemBg)
                i.sprite = imgBtnItemBgSprites[_index];*/
            imgTopBg.sprite = imgTopBgSprites[_index];
            imgLevel.sprite = imgLevelSprites[_index];
            imgBottom.sprite = imgBottomSpirtes[_index];
            imgBtnReturn.sprite = imgBtnReturnSprites[_index];

        }

        /// <summary>
        /// ���ѹؿ�����UI - 显示困难关卡提示（淡入淡出动画）
        /// </summary>
        private void SetTextTip()
        {
            // ���ö��� - 显示提示面板
            LevelTipPanel.Show();
            float _durationTime = 1.5f;
            CanvasGroup _canvasGroup = LevelTipPanel.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            // 淡入动画
            _canvasGroup.DOFade(1f, _durationTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // 淡出动画
                    _canvasGroup.DOFade(0f, _durationTime)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        LevelTipPanel.Hide();
                    });

                });

            // �����ı� 5-20��ƫתֵ - 随机生成50-70%的失败率文本
            TextLevelTip.text = UnityEngine.Random.Range(50, 70).ToString() + "% of players were defeated at this level";
        }

        /// <summary>
        /// ��λUI - 初始化段位系统UI，显示当前段位和连胜数
        /// </summary>
        private void InitRankLevel()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            // 如果正好是段位系统开启关卡，显示"Rank"标签5秒后隐藏
            if (level == GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                ImgRankLevel_Label.Show();
                ActionKit.Delay(5f, () =>
                {
                    ImgRankLevel_Label.Hide();

                }).Start(this);
            }
            else ImgRankLevel_Label.Hide();

            // 达到段位系统开启等级，显示段位图标
            if (level >= GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                ImgRankLevel.Show();
                var _tempWin = stageModel.InGameRankStreakWinNum;
                TxtRankLevel.text = _tempWin.ToString();
                //5����ʤ����һ����λ,�ܶ�λ��9(��ʼ0) - 每5连胜晋升一个段位，最高段位为9（从0开始）
                mCacheRankSpriteIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                ImgRankLevel.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mCacheRankSpriteIndex));
            }

            else ImgRankLevel.Hide();
        }

        /// <summary>
        /// �۳�Я���������� - 进入关卡时消耗玩家携带的道具（半瓶、去黑水、去Debuff）
        /// 如果对应的限时无限使用道具已过期，则扣除普通道具数量
        /// </summary>
        private void ConsumeTakeItems()
        {
            var takeItems = LevelManager.Instance.takeItem;
            // 可携带的普通道具ID
            var itemIds = new[]
            {
                NormalRewardsType.S_AddOneHalfBottle,
                NormalRewardsType.S_RemoveOneBottleHideWater,
                NormalRewardsType.S_RemoveOneDebuffBottle
            };
            // 对应的无限使用道具ID
            var unlimitItems = new[]
            {
               SpecialRewardsType.Unlimited_S_AddOneHalfBottle,
               SpecialRewardsType.Unlimited_S_RemoveOneBottleHideWater,
               SpecialRewardsType.Unlimited_S_RemoveOneDebuffBottle
            };
            for (int i = 0; i < itemIds.Length; i++)
            {
                string _sign = GameEnum.GetDescription(itemIds[i]);

                // 检查玩家是否携带该道具且有库存
                bool _isTakeItem = (takeItems.Contains((int)itemIds[i]) && (stageModel.ItemDic[(int)itemIds[i]] > 0));
                _sign = GameEnum.GetDescription(unlimitItems[i]);
                // 如果携带了道具且对应的无限使用道具已过期，则扣除1个道具
                if (_isTakeItem && CountDownTimerManager.Instance.IsTimerFinished(_sign))
                {
                    stageModel.ReduceItem((int)itemIds[i], 1);
                }

            }
        }

        #endregion

        #region ���ѵ������

        /// <summary>
        /// ��ʾ����ͼ�� - 根据当前关卡等级显示或锁定各个道具按钮
        /// </summary>
        private void InitItemUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            // 检查"加瓶"道具是否解锁
            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelAddBottle)
                UnLockItem(NormalRewardsType.AddOneBottle);
            else
            {
                // 未解锁，显示解锁等级（红色字体）
                mTxtAddBottle.font = LevelManager.Instance.redFont;
                mTxtAddBottle.text = $"{(int)UIGuideLevel.UIGuideLevelAddBottle}";
            }

            // 检查"半瓶"道具是否解锁
            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelHalfBottle)
                UnLockItem(NormalRewardsType.AddHalfBottle);
            else
            {
                mTxtAddHalfBottle.font = LevelManager.Instance.redFont;
                mTxtAddHalfBottle.text = $"{(int)UIGuideLevel.UIGuideLevelHalfBottle}";
            }

            // 检查"去黑水"道具是否解锁
            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveHide)
                UnLockItem(NormalRewardsType.RemoveHide);
            else
            {
                mTxtRemoveHide.font = LevelManager.Instance.redFont;
                mTxtRemoveHide.text = $"{(int)UIGuideLevel.UIGuideLevelRemoveHide}";
            }

            // 检查"去Debuff"道具是否解锁
            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveAll)
                UnLockItem(NormalRewardsType.RemoveAll);
            else
            {
                mTxtRemoveAll.font = LevelManager.Instance.redFont;
                mTxtRemoveAll.text = $"{(int)UIGuideLevel.UIGuideLevelRemoveAll}";
            }

            // 检查"撤回"道具是否解锁
            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelStepBack)
                UnLockItem(NormalRewardsType.StepBack);
            else
            {
                mTxtStepBack.font = LevelManager.Instance.redFont;
                mTxtStepBack.text = $"{(int)UIGuideLevel.UIGuideLevelStepBack}";
            }
        }

        /// <summary>
        /// ���߽��� - 解锁道具按钮，显示道具图标并启用按钮交互
        /// </summary>
        /// <param name="PropType"></param>
        private void UnLockItem(NormalRewardsType PropType)
        {
            Transform transform = null;
            // 根据道具类型找到对应的按钮
            switch (PropType)
            {
                case NormalRewardsType.StepBack:
                    transform = BtnStepBack.transform;
                    break;

                case NormalRewardsType.RemoveHide:
                    transform = BtnRemoveHide.transform;
                    break;

                case NormalRewardsType.AddHalfBottle:
                    transform = BtnHalfBottle.transform;
                    break;

                case NormalRewardsType.AddOneBottle:
                    transform = BtnAddBottle.transform;
                    break;

                case NormalRewardsType.RemoveAll:
                    transform = BtnRemoveAll.transform;
                    break;
            }

            // 显示道具图标，隐藏锁，启用按钮，恢复颜色
            transform.Find("ImgItem").Show();
            transform.Find("ImgLock").Hide();
            transform.GetComponent<Button>().interactable = true;
            transform.Find("ImgItem").GetComponent<Image>().color = Color.white;
        }

        /// <summary>
        /// �·�������UI���� - 刷新所有道具的数量显示和购买按钮状态
        /// </summary>
        private void SetItem()
        {
            stageModel = this.GetModel<StageModel>();
            // 撤回道具：数量为0时显示购买按钮
            BtnAddStepBack.gameObject.SetActive(stageModel.ItemDic[1] <= 0);
            TxtRefreshNum.text = stageModel.ItemDic[1].ToString();

            // 去黑水道具
            BtnAddRemove.gameObject.SetActive(stageModel.ItemDic[2] <= 0);
            TxtRemoveHideNum.text = stageModel.ItemDic[2].ToString();

            // 加瓶道具
            BtnAddAddBottle.gameObject.SetActive(stageModel.ItemDic[3] <= 0);
            TxtAddBottleNum.text = stageModel.ItemDic[3].ToString();

            // 半瓶道具
            BtnAddHalfBottle.gameObject.SetActive(stageModel.ItemDic[4] <= 0);
            TxtAddHalfBottleNum.text = stageModel.ItemDic[4].ToString();

            // 去Debuff道具
            BtnAddRemoveBottle.gameObject.SetActive(stageModel.ItemDic[5] <= 0);
            TxtRemoveAllNum.text = stageModel.ItemDic[5].ToString();
        }

        /// <summary>
        /// 撤回按钮点击事件：撤销上一步操作，消耗1个撤回道具
        /// </summary>
        private void BtnSetpBackOnClick()
        {
            // 检查是否可以使用道具（没有播放特效动画且可以倒水）
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                // 道具数量不足，打开购买界面
                if (stageModel.ItemDic[1] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 1 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                // 执行撤回操作，成功后扣除道具
                if (LevelManager.Instance.ReturnLast())
                    stageModel.ReduceItem(1, 1);
            }
        }
        /// <summary>
        /// 去黑水按钮点击事件：移除一个黑水瓶的黑水，消耗1个去黑水道具
        /// </summary>
        private void BtnRemoveHideOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[2] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 2 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                //�ж��Ƿ��к�ˮƿ - 判断是否有黑水瓶
                if (LevelManager.Instance.hideBottleList.Count != 0)
                {
                    LevelManager.Instance.RemoveHide(() =>
                    {
                        stageModel.ReduceItem(2, 1);
                    });
                }
            }
        }
        /// <summary>
        /// 加瓶按钮点击事件：添加一个空瓶，消耗1个加瓶道具
        /// </summary>
        private void BtnAddBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[3] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 3 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                // false表示添加空瓶
                LevelManager.Instance.AddBottle(false, () =>
                {
                    stageModel.ReduceItem(3, 1);
                });
            }
        }
        /// <summary>
        /// 半瓶按钮点击事件：添加一个半满的瓶子，消耗1个半瓶道具
        /// </summary>
        private void BtnHalfBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[4] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 4 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                // true表示添加半满瓶
                LevelManager.Instance.AddBottle(true, () =>
                {
                    stageModel.ReduceItem(4, 1);
                });
            }
        }
        private void BtnRemoveAllOnClick()
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
                        
                        foreach (var bottle in LevelManager.Instance.nowBottles)
                        {
                            foreach (var record in bottle.moveRecords)
                            {
                                record.isFreeze = false;
                                record.isClearHide = false;
                                record.isNearHide = false;
                                record.limitColor = 0;

                                for (int i = 0; i < record.HideWaterTypes.Count; i++)
                                {
                                    record.HideWaterTypes[i] = HideWaterType.None;
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
        }
        #endregion

        #region ʱ���� - 异步队列执行系统，确保携带道具特效按顺序播放

        // �첽���� - 异步动作队列
        private readonly Queue<Action<Action>> mActionQueue = new();
        // 是否正在执行动作
        private bool mIsRunning = false;

        /// <summary>
        /// 将动作加入队列
        /// </summary>
        private void EnqueueAction(Action<Action> action)
        {
            mActionQueue.Enqueue(action);
            TryRunNext();
        }

        /// <summary>
        /// 尝试执行下一个动作（如果没有正在执行的动作且队列不为空）
        /// </summary>
        private void TryRunNext()
        {
            if (mIsRunning || mActionQueue.Count == 0) return;

            mIsRunning = true;
            var action = mActionQueue.Dequeue();
            // 执行动作，动作完成后回调通知继续执行下一个
            action.Invoke(() =>
            {
                mIsRunning = false;
                TryRunNext();
                //ActionKit.Delay(0.3f, () =>
                //{

                //}).Start(this);
            });
        }

        /// <summary>
        /// 自动使用所有携带的道具（包括连胜奖励和进入关卡时选择的道具）
        /// 所有道具效果按队列顺序依次播放
        /// </summary>
        private void AutoUseAllItems()
        {
            var level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            // 连胜10次奖励：去除一半黑水瓶的黑水
            if (level >= (int)GameDefine.UnLockMechanism.RemoveHideWinStreakLevel
                && stageModel.RemoveHideStreakWinNum >= GameConst.TEN_CONTINUE_WIN_NUM)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("��ʤȥ����Ч");
                    StreaWinClearBWater(_nextItem);
                });
            }

            // 携带道具1：去除一个瓶子的黑水
            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_RemoveOneBottleHideWater)
                && level >= (int)UnLockMechanism.EnterLevelSelectProps)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("ȥ�ڵ�����Ч");
                    RemoveOneBottleHideWater(_nextItem);
                });
            }

            // 携带道具2：去除一个瓶子的Debuff
            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_RemoveOneDebuffBottle)
                && level >= (int)UnLockMechanism.EnterLevelSelectProps)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("ȥDebuff������Ч");
                    RemoveOneDebuffBottle(_nextItem);
                });
            }

            // 携带道具3：添加一个半瓶（随机颜色变为彩虹水）
            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_AddOneHalfBottle)
                && level >= (int)UnLockMechanism.EnterLevelSelectProps)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("����ƿ����Ч");
                    AddOneHalfBottle(_nextItem);
                });
            }
        }

        #endregion

        #region Я��������� - 进入关卡时自动使用的携带道具效果

        /// <summary>
        /// ���ƿ�����к�ˮ - 清除黑水瓶的黑水状态
        /// </summary>
        /// <param name="useItem">�Ƿ��ɵ�����Ч - 是否由道具触发（true=清除1个，false=清除一半）</param>
        /// <param name="action">完成后的回调</param>
        private void ClearBottleBlackWater(bool useItem, Action action = null)
        {
            if (LevelManager.Instance.hideBottleList.Count > 0)
            {
                //�޳�ħ�������մ�ƿ��ƿ�� - 排除魔法瓶和空黑水瓶
                var _tempList = new List<BottleCtrl>(LevelManager.Instance.hideBottleList);
                _tempList.RemoveAll(item => item.isClearHide || item.isNearHide);

                if (_tempList.Count == 0)
                {
                    action?.Invoke();
                    return;
                }

                // 道具效果清除1个，连胜奖励清除一半
                int _removeCount = useItem ? 1 : _tempList.Count / 2;
                //���д���(ֻ��һ����ˮƿ) - 容错处理（只有一个黑水瓶）
                _removeCount = Math.Min(_removeCount, _tempList.Count);

                // 随机移除多余的瓶子，保留需要清除的数量
                while (_tempList.Count > _removeCount)
                {
                    int randIndex = UnityEngine.Random.Range(0, _tempList.Count);
                    _tempList.RemoveAt(randIndex);
                }

                // 对选中的瓶子执行清除黑水操作
                foreach (var item in _tempList)
                {
                    LevelManager.Instance.hideBottleList.Remove(item);
                    item.StarSetHideShow();
                }
                action?.Invoke();
            }
            else
                action?.Invoke();
        }

        /// <summary>
        /// ��ʤȥ�� - 连胜10次奖励，清除一半黑水瓶的黑水
        /// </summary>
        /// <param name="action">完成后的回调</param>
        private void StreaWinClearBWater(Action action)
        {
            // 播放粒子特效后执行清除
            PlayParticleEffect(() =>
            {
                // false表示清除一半黑水瓶
                ClearBottleBlackWater(false, () =>
                {
                    action?.Invoke();
                });
            });
        }

        /// <summary>
        /// ȥ��һƿ��ˮ
        /// </summary>
        /// <param name="onComplete"></param>
        private void RemoveOneBottleHideWater(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_RemoveOneBottleHideWater);
            PlayParticleEffect(() =>
            {
                ClearBottleBlackWater(true, () =>
                {
                    // ����֪ͨ�������
                    onComplete?.Invoke();
                });
            }, _sprite);
        }

        /// <summary>
        /// ����һ��ƿ�� - 携带道具效果，将一个随机颜色转换为彩虹水（万能颜色）
        /// </summary>
        /// <param name="onComplete">完成后的回调</param>
        private void AddOneHalfBottle(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_AddOneHalfBottle);
            void _changeRainBowWater(Action callback)
            {
                var _tempWater = new List<int>(LevelManager.Instance.clearList);
                //1���Ƴ�ҩˮ��Ҫ��������ɫ - 排除药水需要转换的颜色（特殊机制）
                var changeColors = LevelManager.Instance.nowLevel.changeList.Select(x => x.NeedChangeColor);
                _tempWater = _tempWater.Except(changeColors).ToList();
                //2���Ƴ���Ҫ���������ɫ - 排除只需要收集1个的颜色（避免过早完成）
                _tempWater = _tempWater.GroupBy(x => x).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
                //3���Ƴ�ħ��������ɫ �� ����ƿ��ɫ - 排除魔法瓶颜色和限制瓶颜色
                var hideColors = LevelManager.Instance.nowBottles
                    .Where(b => b.isClearHide || b.limitColor > 0)
                    .Select(b => b.isClearHide ? b.unlockClear : b.limitColor);
                _tempWater = _tempWater.Except(hideColors).ToList();

                //4��ȡ�����ɫ - 从剩余颜色中随机选择一个
                var _colorIdx = _tempWater[UnityEngine.Random.Range(0, _tempWater.Count)];
                LevelManager.Instance.clearList.Remove(_colorIdx);
                LevelManager.Instance.clearList.Add((int)ItemType.RainBowWater);

                //�����������ɫ��ƿ�ӣ�ִ�з��� - 遍历所有包含该颜色的瓶子，执行转换
                foreach (var bottle in LevelManager.Instance.nowBottles)
                {
                    if (bottle.waters.Contains(_colorIdx))
                    {
                        bottle.ChangeWaterToRainBowWater(_colorIdx);
                    }
                }

                callback?.Invoke();
            }

            PlayParticleEffect(() => _changeRainBowWater(onComplete), _sprite);
        }

        /// <summary>
        /// �Ƴ�һ��ƿ�ӵĸ���״̬ - 携带道具效果，随机移除1个瓶子的所有Debuff
        /// </summary>
        /// <param name="onComplete">完成后的回调</param>
        private void RemoveOneDebuffBottle(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_RemoveOneDebuffBottle);
            void _removeDeuff(Action callback)
            {
                var _tempbottle = new List<BottleCtrl>(LevelManager.Instance.nowBottles);
                BottleCtrl _bottle = null;
                // 随机选择一个有Debuff的瓶子
                while (_tempbottle.Count != 0)
                {
                    var _randomIndex = UnityEngine.Random.Range(0, _tempbottle.Count);
                    _bottle = _tempbottle[_randomIndex];
                    if (_bottle.CheckDebuff())
                        break;
                    else
                        _tempbottle.RemoveAt(_randomIndex);
                }

                // 将选中的瓶子恢复正常状态
                _bottle?.SetNormal(true);
                callback?.Invoke();
            }

            PlayParticleEffect(() => _removeDeuff(onComplete), _sprite);
            /*#region ԭ����ˮ�鹦��
            // �����б��������ϴ��
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
            // �滻ԭ�б�
            botter.waters = _newWaters;
            botter.hideWaters = _newHideWater;
            botter.waterItems = _newWaterItems;
            botter.bombCounts = _newBombs;

            //�޸�ˮ����ɫ���л�����λ��
            for (int i = 0; i < botter.waters.Count; i++)
            {
                var useColor = botter.waters[i] - 1;
                if (useColor < 1000)
                    botter.waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor], i == botter.topIdx);
                else
                    botter.waterImg[i].SetColorState((ItemType)botter.waters[i], LevelManager.Instance.ItemColor, i == botter.topIdx);
            }

            //�޸�ˮ��λ�ã��޸�ˮ����ɫ������ˮ�涯��
            botter.SetNowSpinePos(botter.waters.Count);
            botter.PlaySpineWaitAnim();
            botter.CheckWaterItem();
            botter.UpdateBomb();
            botter.SetHideShow(true);
            LevelManager.Instance.HideItemSelect();

            TxtItem3.text = "0";
            //Debug.Log("����˳��ɹ�");
            #endregion*/
        }

        /// <summary>
        /// �����볡���� - 播放道具使用的粒子特效动画（放大缩小）
        /// </summary>
        /// <param name="action">特效完成后的回调</param>
        /// <param name="sprite">道具图标（可选）</param>
        private void PlayParticleEffect(Action action, Sprite sprite = null)
        {
            var _particle = Resources.Load(ITEM_ENTRANCE_EFFECT_PATH);
            var _tempObj = Instantiate(_particle) as GameObject;

            // 如果提供了图标，替换默认图标
            if (sprite != null)
                _tempObj.GetComponent<SpriteRenderer>().sprite = sprite;

            //UIKit.OpenPanel<UIMask>(UILevel.PopUI);//���� - 打开遮罩
            // 向下移动到中心
            _tempObj.transform.DOLocalMoveY(0, 1f);
            // 放大动画
            _tempObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f)
            .OnComplete(() =>
            {
                action?.Invoke();
                Destroy(_tempObj);
            });
        }

        /// <summary>
        /// 打开胜利界面（延迟0.5秒，播放胜利音效）
        /// </summary>
        private void OpenUIVictory()
        {
            ActionKit.Delay(0.5f, () =>
            {
                UIKit.ClosePanel<UIMask>();
                AudioKit.PlaySound("resources://Audio/Victory");
                UIKit.OpenPanel<UIVictory>();
            }).Start(this);
        }
        #endregion
    }
}
