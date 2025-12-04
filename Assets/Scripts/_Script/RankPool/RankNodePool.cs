using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class RankNodePool : MonoSingleton<RankNodePool>, ICanGetModel
{
    private const string RankNodePrefabPath = "Prefab/RankNode";
    private const int MAX_RANK_COUNT = 30;
    private const int MAX_PAGE_RANK_COUNT = 10;   // 单页最大节点数(现为13,逻辑不影响)

    private RankDataModel mRankDataModel;
    private List<GameObject> mTempRankFrontNodes; // 玩家前的节点
    private List<GameObject> mTempRankBackNodes;  // 玩家后的节点
    private GameObject mPlayerNode;
    private Transform mRankNodeParent;

    private SimpleObjectPool<GameObject> mRankNodePool;

    [SerializeField] private Sprite mPlayerSprite;
    [SerializeField] private Sprite mDefaultSprite;
    [SerializeField] private Sprite mFirSprite;
    [SerializeField] private Sprite mSecSprite;
    [SerializeField] private Sprite mThiSprite;
    
    public override void OnSingletonInit()
    {
        mRankDataModel = this.GetModel<RankDataModel>();
        mTempRankFrontNodes = new();
        mTempRankBackNodes = new();
        mRankNodeParent = GetComponent<Transform>();

        mRankNodePool = new SimpleObjectPool<GameObject>(
        () =>
        {
            var obj = Resources.Load(RankNodePrefabPath);
            var node = Instantiate(obj, mRankNodeParent) as GameObject;
            node.Hide();
            return node;
        },
        (obj) =>
        {
            obj.GetComponent<Image>().sprite = mDefaultSprite;
            obj.Hide();
            obj.transform.SetParent(mRankNodeParent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        },
        MAX_RANK_COUNT);
    }

    /// <summary>
    /// 排名上升
    /// </summary>
    /// 传入当前排名和目标分数
    public void RankRaise(int curRank, int targetScore, ScrollRect scrollRect)
    {
        #region 取节点、初始化

        // 计算前后对手节点数量
        int _frontCount;
        int _backCount;

        if (curRank >= MAX_RANK_COUNT)
        {
            _frontCount = MAX_RANK_COUNT - 1;
            _backCount = 0;
        }
        else
        {
            _frontCount = curRank - 1;
            _backCount = (MAX_RANK_COUNT - 1) - _frontCount;
        }

        // 取前面的节点(列表顺序按排名从高到低)
        for (int i = _frontCount; i > 0; i--)
        {
            int _index = curRank - i - 1;
            if (_index >= 0 && _index < mRankDataModel.RankDataList.Count)
            {
                var _node = mRankNodePool.Allocate();
                _node.transform.localScale = Vector3.one;
                _node.Show();
                _node.TryGetComponent<RankNode>(out var _rankNode);
                var _data = mRankDataModel.RankDataList[_index];
                _rankNode.Init(_index + 1, _data);
                ApplyRankSprite(_node.GetComponent<Image>(), _index + 1, mDefaultSprite);
                mTempRankFrontNodes.Add(_node);
            }
        }

        // 玩家节点
        mPlayerNode = mRankNodePool.Allocate();
        mPlayerNode.transform.localScale = Vector3.one;
        mPlayerNode.Show();
        mPlayerNode.GetComponent<Image>().sprite = mPlayerSprite;
        mPlayerNode.TryGetComponent<RankNode>(out var playerRankNode);
        playerRankNode.InitPlayer(curRank, targetScore);
        ApplyRankSprite(mPlayerNode.GetComponent<Image>(), curRank, mPlayerSprite);

        // 取后面的节点(列表顺序按排名从低到高)
        for (int i = 1; i <= _backCount; i++)
        {
            int _index = curRank - 1 + i;
            if (_index >= 0 && _index < mRankDataModel.RankDataList.Count)
            {
                var _node = mRankNodePool.Allocate();
                _node.transform.localScale = Vector3.one;
                _node.Show();
                _node.TryGetComponent<RankNode>(out var _rankNode);
                var _data = mRankDataModel.RankDataList[_index];
                _rankNode.Init(_index + 1, _data);
                ApplyRankSprite(_node.GetComponent<Image>(), _index + 1, mDefaultSprite);
                mTempRankBackNodes.Add(_node);
            }
        }

        //将所有节点按顺序设为GridLayoutGroup的子节点(做自动排序)
        foreach (var node in mTempRankFrontNodes)
        {
            node.transform.SetParent(scrollRect.content, false);
        }
        mPlayerNode.transform.SetParent(scrollRect.content, false);
        foreach (var node in mTempRankBackNodes)
        {
            node.transform.SetParent(scrollRect.content, false);
        }

        #endregion

        if (curRank == 1)
        {
            //Debug.Log("当前已经是第一名了");
            return;
        }

        //初始玩家视口位置
        SetScrollPositionCoroutine(scrollRect, curRank);

        //获取目标排名
        var playerTargetRank = mRankDataModel.GetPlayerRank(targetScore);

        if (curRank == playerTargetRank)
        {
            //Debug.Log("排名没有变化，直接返回");
            return;
        }

        //等待0.5f后开始滚动到GridLayoutGroup第二个节点的位置情况
        ActionKit.Delay(0.5f, () =>
        {
            int rankDifference = curRank - playerTargetRank; // 提升名次数
            int moveToIndex; // 最终子节点位置
            // 排名提升超过总节点数
            if (rankDifference >= MAX_RANK_COUNT - 1)
            {
                // 不在前10名内，作为第二个节点
                if (playerTargetRank > MAX_PAGE_RANK_COUNT)
                {
                    moveToIndex = 1;
                }
                //前十名内以排名为准
                else
                {
                    moveToIndex = playerTargetRank - 1;
                }
            }
            else
            {
                int frontNodesCount = mTempRankFrontNodes.Count;
                moveToIndex = frontNodesCount - rankDifference;
                if (moveToIndex < 0) moveToIndex = 0;
            }

            #region 排名上升+玩家排名变化

            // 1、玩家节点上升(禁用组件避免组件控制子节点)
            scrollRect.content.GetComponent<GridLayoutGroup>().Disable();
            scrollRect.content.GetComponent<ContentSizeFitter>().Disable();

            float targetY = CalculateGridChildLocalY(scrollRect, moveToIndex);
            var rect = mPlayerNode.GetComponent<RectTransform>();
            rect.DOScale(1.2f, 0.5f);
            rect.DOLocalMoveY(targetY, 2f).OnComplete(() =>
            {
                scrollRect.content.GetComponent<GridLayoutGroup>().Enable();
                scrollRect.content.GetComponent<ContentSizeFitter>().Enable();

                mPlayerNode.transform.SetSiblingIndex(moveToIndex);
                rect.localScale = Vector3.one;

                ApplyRankSprite(mPlayerNode.GetComponent<Image>(), playerTargetRank, mPlayerSprite);
            });

            // 2、ScrollRect滚动
            int playerIndex = moveToIndex;
            float targetNormalizedPos = CalculateNormalizedPositionForNode(scrollRect, playerIndex);
            DOTween.To(() => scrollRect.verticalNormalizedPosition,
                x => scrollRect.verticalNormalizedPosition = x,
                targetNormalizedPos,
                2f);

            // 3、玩家节点排名更新
            if (mPlayerNode.TryGetComponent<RankNode>(out var playerRankNode))
            {
                float startValue = curRank;
                DOTween.To(() => startValue, x =>
                {
                    startValue = x;
                    playerRankNode.TxtRank.text = Mathf.FloorToInt(startValue).ToString();
                }, playerTargetRank, 2f);
            }

            // 4、其他节点排名更新
            var content = scrollRect.content;
            List<GameObject> frontNodesNew = new List<GameObject>();
            List<GameObject> backNodesNew = new List<GameObject>();

            int totalCount = content.childCount;
            int playerSiblingIndex = mPlayerNode.transform.GetSiblingIndex();
            for (int i = 0; i < totalCount; i++)
            {
                var child = content.GetChild(i).gameObject;

                if (child == mPlayerNode)
                    continue; // 玩家节点不处理

                if (i < moveToIndex)
                    frontNodesNew.Add(child);
                //else if (i > moveToIndex)
                else
                    backNodesNew.Add(child);
            }

            // Tween前面节点排名
            for (int i = 0; i < frontNodesNew.Count; i++)
            {
                int nodeRank = playerTargetRank - frontNodesNew.Count + i; // 目标排名
                var node = frontNodesNew[i];
                if (node.TryGetComponent<RankNode>(out var rankNode))
                {
                    rankNode.RankingChangeOther(nodeRank, false, () =>
                    {
                        ApplyRankSprite(node.GetComponent<Image>(), nodeRank, mDefaultSprite);
                    });
                }
            }

            // Tween后面节点排名
            for (int i = 0; i < backNodesNew.Count; i++)
            {
                //+1会跳过玩家原先所在节点,这会造成目标排名数据被顶替效果
                //按玩家原先排名取后面的数据,并做动画,在动画完成时,往下顺延一位排名
                //且UI的更新传入后一名的(针对于前三名UI不同,如玩家顶替了前三名,则原第三名UI要更改)
                int nodeRank = playerTargetRank + i; //+1 // 目标排名
                var node = backNodesNew[i];
                if (node.TryGetComponent<RankNode>(out var rankNode))
                {
                    rankNode.RankingChangeOther(nodeRank, true, () =>
                    {
                        //回调
                        ApplyRankSprite(node.GetComponent<Image>(), nodeRank + 1, mDefaultSprite);
                    });
                    //ApplyRankSprite(node.GetComponent<Image>(), nodeRank, mDefaultSprite);
                }
            }
            #endregion

        }).Start(this);
    }

    public void ClearRankNode()
    {
        foreach (var node in mTempRankFrontNodes)
        {
            mRankNodePool.Recycle(node);
        }

        foreach (var node in mTempRankBackNodes)
        {
            mRankNodePool.Recycle(node);
        }

        if (mPlayerNode != null)
        {
            mRankNodePool.Recycle(mPlayerNode);
            mPlayerNode = null;
        }

        mTempRankFrontNodes.Clear();
        mTempRankBackNodes.Clear();
    }

    #region 节点位置等计算
    
    // 设置玩家节点在视口底部
    private void SetScrollPositionCoroutine(ScrollRect scrollRect, int curRank)
    {
        Canvas.ForceUpdateCanvases();

        if (mPlayerNode == null) return;

        var content = scrollRect.content;
        var viewport = scrollRect.viewport;

        var contentRect = content.GetComponent<RectTransform>();
        var viewportRect = viewport.GetComponent<RectTransform>();
        var playerRect = mPlayerNode.GetComponent<RectTransform>();

        // 获取玩家节点顶部到Content顶部的距离
        float playerTopY = Mathf.Abs(playerRect.anchoredPosition.y);

        // 玩家节点的高度
        float playerHeight = playerRect.rect.height / 2;
       
        // 让玩家的底部对齐 viewport 的底部
        // 计算需要滚动的距离：playerTopY + playerHeight - viewportHeight
        float offset = playerTopY + playerHeight - viewportRect.rect.height;

        // 将偏移量转成 normalizedPosition
        float scrollRange = contentRect.rect.height - viewportRect.rect.height;

        float targetNormalizedPos = 1f;

        if (scrollRange > 0f)
        {
            targetNormalizedPos = 1f - (offset / scrollRange);
            targetNormalizedPos = Mathf.Clamp01(targetNormalizedPos);
        }
        else
        {
            targetNormalizedPos = 1f;
        }

        scrollRect.verticalNormalizedPosition = targetNormalizedPos;
    }

    // 玩家节点在GridLayoutGroup中的本地Y位置
    private float CalculateGridChildLocalY(ScrollRect scrollRect, int siblingIndex)
    {
        var grid = scrollRect.content.GetComponent<GridLayoutGroup>();
        if (grid == null) return 0f;

        float spacing = grid.spacing.y;
        float cellHeight = grid.cellSize.y;
        float offset = grid.cellSize.y / 2;

        return -(cellHeight + spacing) * siblingIndex - offset;
    }

    // ScrollBar 的归一化位置计算
    private float CalculateNormalizedPositionForNode(ScrollRect scrollRect, int nodeIndex)
    {
        var contentRect = scrollRect.content.GetComponent<RectTransform>();
        var viewportRect = scrollRect.viewport;

        var grid = scrollRect.content.GetComponent<GridLayoutGroup>();
        if (grid == null) return 1f;

        float spacingY = grid.spacing.y;
        float cellHeight = grid.cellSize.y;

        float nodeHeightWithSpacing = cellHeight + spacingY;

        float contentHeight = contentRect.rect.height;
        float viewportHeight = viewportRect.rect.height;

        // 计算目标节点顶部距离content顶部的距离
        // nodeIndex从0开始，节点顶部位置为：nodeIndex * nodeHeightWithSpacing
        float nodeTopPos = nodeIndex * nodeHeightWithSpacing;

        // 计算使该节点底部对齐viewport底部时的content滚动位置
        // 计算content滚动距离 = 节点底部位置 - viewport高度
        float nodeBottomPos = nodeTopPos + cellHeight;

        // scrollPos = nodeBottomPos - viewportHeight
        float scrollPos = nodeBottomPos - viewportHeight;

        // 限制范围 scrollPos 不能小于0，不能大于 contentHeight - viewportHeight
        scrollPos = Mathf.Clamp(scrollPos, 0, contentHeight - viewportHeight);

        float normalizedPos = 1f - (scrollPos / (contentHeight - viewportHeight));

        return Mathf.Clamp01(normalizedPos);
    }

    private void ApplyRankSprite(Image image, int rank, Sprite del)
    {
        if (rank == 1)
            image.sprite = mFirSprite;
        else if (rank == 2)
            image.sprite = mSecSprite;
        else if (rank == 3)
            image.sprite = mThiSprite;
        else
            image.sprite = del;
    }
    #endregion

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
