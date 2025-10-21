using GameDefine;
using JsonFileData;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIManager : MonoSingleton<RewardUIManager>
{
    [SerializeField] private RewardSpriteMappingSO mRewardSpriteMappingSO;
    [SerializeField] private Animator BoxAnimator;
    [SerializeField] private Animator AddCoinTxtUp;
    [SerializeField] private Button BtnContinue;
    [SerializeField] private RectTransform mRectTransformPar;
    [SerializeField] private ParticleTargetMoveCtrl CoinParticle;

    public SimpleObjectPool<Image> RewardPool;
    private List<Image> mActiveRewards;
    private RectTransform mMask;
    private TextMeshProUGUI txtCoinAdd;
    private List<int> availableSlots;
    private List<System.Action> actionList;
    private System.Action openBoxCallBack;

    private const int YAXIS = 0;//800

    public override void OnSingletonInit()
    {
        mMask = BoxAnimator.transform.parent.GetComponent<RectTransform>();
        txtCoinAdd = AddCoinTxtUp.GetComponent<TextMeshProUGUI>();
        txtCoinAdd.font = LevelManager.Instance.redFont;
        actionList = new List<System.Action>();
        availableSlots = new List<int>();
        mActiveRewards = new List<Image>();
        mRewardSpriteMappingSO.Initialize();

        RewardPool = new SimpleObjectPool<Image>(
        () =>
        {
            var par = Resources.Load<GameObject>("Prefab/PropPoolNode");
            var image = Instantiate(par, mRectTransformPar).GetComponent<Image>();
            return image;

        },
        (Image img) =>
        {
            img.Hide();
            img.transform.SetParent(mRectTransformPar);
            img.rectTransform.localPosition = Vector3.zero;
            img.rectTransform.localScale = Vector3.one;
        },
        initCount: 10);

        BtnContinue.onClick.AddListener(() =>
        {
            BtnContinue.interactable = false;
            StartCoroutine(ContinueClickEvent());
        });
    }

    #region 发放奖励表现

    /// <summary>
    /// 发放奖励表现
    /// </summary>
    /// <param name="addCoin"></param>
    /// <param name="call"></param>
    /// <param name="openBox">开箱/直接发放</param>
    /// <param name="packSOs"></param>
    public void PlayRewardAnim(int? addCoin, bool openBox, System.Action call, params IPackSoInterface[] packSOs)
    {
        var _packSO = new List<IPackSoInterface>();
        int _slotCount = 0;
        foreach (var pack in packSOs)
        {
            if (pack == null) continue;
            _packSO.Add(pack);
            _slotCount += pack.ItemReward.Count + pack.SpecialRewards.Count;
        }

        openBoxCallBack = call;
        availableSlots.Clear();
        actionList.Clear();
        mMask.Show();

        for (int i = 0; i < _slotCount; i++)
            availableSlots.Add(i);

        float _waitValue = 0f;
        if (openBox)
        {
            BoxAnimator.Show();
            BoxAnimator.Play("BoxOpen");
            _waitValue = 1f;
        }

        // 等待盒子打开动画完成(1秒)
        ActionKit.Delay(_waitValue, () =>
        {
            if (_slotCount != 0)
                BtnContinue.Show();
            else
                mMask.Hide();

            BoxAnimator.Hide();

            foreach (var pack in _packSO)
            {
                foreach (var item in pack.ItemReward)
                {
                    var image = RewardPool.Allocate();
                    image.TryGetComponent(out PropRewardPoolNode _node);
                    if (_node == null)
                        _node = image.gameObject.AddComponent<PropRewardPoolNode>();
                    _node.Init(mRewardSpriteMappingSO.GetRewardSprite(item.NormalRewardsType), 
                        SetRandomScreenPosition(image, _slotCount), item.Quantity, false);
                    actionList.Add(() => _node.MoveOffScreen());
                }

                foreach (var item in pack.SpecialRewards)
                {
                    var image = RewardPool.Allocate();
                    image.TryGetComponent(out PropRewardPoolNode _node);
                    if (_node == null)
                        _node = image.gameObject.AddComponent<PropRewardPoolNode>();

                    _node.Init(mRewardSpriteMappingSO.GetRewardSprite(item.SpecialRewardType), 
                        SetRandomScreenPosition(image, _slotCount), item.Duration, true);
                    actionList.Add(() => _node.MoveOffScreen());
                }
            }

            if ((addCoin ?? 0) > 0)
            {
                CoinParticle.Play(100);
                PopupCoinText((int)addCoin);

                if (actionList.Count == 0)
                {
                    openBoxCallBack?.Invoke();
                    openBoxCallBack = null;
                }
            }
        }).Start(this);
    }

    public void PlayRewardAnim(System.Action call = null, bool openBox = true, params RewardItem[] rewardItems)
    {
       /* var _packSO = new List<IPackSoInterface>();*/
        var itemDict = new Dictionary<string, int>();
        int _slotCount = 0;
        foreach (var reward in rewardItems)
        {
            itemDict[reward.itemType] = itemDict.GetValueOrDefault(reward.itemType) + reward.itemQuantity;
        }
        _slotCount = itemDict.Count;
        openBoxCallBack = call;
        availableSlots.Clear();
        actionList.Clear();
        mMask.Show();

        for (int i = 0; i < _slotCount; i++)
            availableSlots.Add(i);

        float _waitValue = 0f;
        if (openBox)
        {
            BoxAnimator.Show();
            BoxAnimator.Play("BoxOpen");
            _waitValue = 1f;
        }

        // 等待盒子打开动画完成(1秒)
        ActionKit.Delay(_waitValue, () =>
        {
            if (_slotCount != 0)
                BtnContinue.Show();
            else
                mMask.Hide();

            BoxAnimator.Hide();

            foreach (var reward in itemDict)
            {
                // 跳过金币
                if (reward.Key == "Coins")
                    continue;

                var image = RewardPool.Allocate();
                image.TryGetComponent(out PropRewardPoolNode _node);

                if (_node == null)
                    _node = image.gameObject.AddComponent<PropRewardPoolNode>();

                SpecialRewardsType _rewardEnum1;
                if (Enum.TryParse<SpecialRewardsType>(reward.Key, out _rewardEnum1))
                    _node.Init(mRewardSpriteMappingSO.GetRewardSprite(reward.Key),
                       SetRandomScreenPosition(image, _slotCount), reward.Value, true);
                else
                    _node.Init(mRewardSpriteMappingSO.GetRewardSprite(reward.Key),
                       SetRandomScreenPosition(image, _slotCount), reward.Value, false);

                actionList.Add(() => _node.MoveOffScreen());
                
            }
            // 金币播放
            int coins;
            if ((itemDict.TryGetValue("Coins",out coins)))
            {
                CoinParticle.Play(100);
                PopupCoinText((int)coins);

                if (actionList.Count == 0)
                {
                    openBoxCallBack?.Invoke();
                    openBoxCallBack = null;
                }
            }
        }).Start(this);
    }

    public void PopupCoinText(float value)
    {
        txtCoinAdd.text = $"+{value}";
        AddCoinTxtUp.Play("TxtUp");
        AudioKit.PlaySound("resources://Audio/AddCoin");
    }

    public Sprite GetRewardSprite<T>(T rewardType) where T : Enum
    {
        return mRewardSpriteMappingSO.GetRewardSprite<T>(rewardType);
    }
    
    #endregion

    #region 外部访问对象池方法

    public Image Allocate()
    {
        var _img = RewardPool.Allocate();
        _img.Show();
        mActiveRewards.Add( _img );
        return _img;
    }

    public void RecyleAll()
    {
        foreach (var item in mActiveRewards)
        {
            item.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            item.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            RewardPool.Recycle(item);
        }
        mActiveRewards.Clear();
    }

    #endregion

    private IEnumerator ContinueClickEvent()
    {
        foreach (var item in actionList)
        {
            item?.Invoke();
            yield return new WaitForSeconds(0.2f);
        }

        BtnContinue.Hide();
        BtnContinue.interactable = true;
        mMask.Hide();

        if (openBoxCallBack != null)
        {
            openBoxCallBack?.Invoke();
            openBoxCallBack = null;
        }
    }

    private Vector2 SetRandomScreenPosition(Image propImage ,int slotCount)
    {
        if (availableSlots.Count == 0)
        {
            Debug.LogWarning("槽位用尽，请先调用 PrepareSlotLayout！");
            return Vector2.zero;
        }

        // 抽一个槽位索引
        int slotIndex = availableSlots[UnityEngine.Random.Range(0, availableSlots.Count)];
        availableSlots.Remove(slotIndex);

        // 每行最大个数
        int maxPerRow = 5;
        // 道具间隔 210，整体居中
        float spacing = 210f;
        // 行间隔
        float rowSpacing = 250f;

        int row = slotIndex / maxPerRow;
        int indexInRow = slotIndex % maxPerRow;
        // 当前行要摆多少个
        int totalRows = Mathf.CeilToInt((float)slotCount / maxPerRow);
        int itemsInThisRow = maxPerRow;
        if (row == totalRows - 1 && slotCount % maxPerRow != 0)
        {
            itemsInThisRow = slotCount % maxPerRow;
        }
        // 该排 X 轴中心居中
        float x = indexInRow * spacing - (itemsInThisRow - 1) * spacing * 0.5f;
        float y = YAXIS - row * rowSpacing;

        return new Vector2(x, y);

        //float x = slotIndex * spacing - (slotCount - 1) * spacing * 0.5f;
        //return new Vector2(x, YAXIS);
    }
}
