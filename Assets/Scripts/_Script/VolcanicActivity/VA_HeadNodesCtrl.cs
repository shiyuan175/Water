using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using Spine.Unity;
using UnityEngine;

public class VA_HeadNodesCtrl : MonoBehaviour
{
    private const int TARGET_POS_OFFSET_X = 30;
    private const int TARGET_POS_OFFSET_Y = 20;
    private const int LAVA_POS_OFFSET_X = 20;
    private const int LAVA_POS_OFFSET_Y = 10;

    private readonly List<Tween> mTweens = new();
    //点位Pos值代表下一个位置(索引0表示起始点跳到第一个台阶，1表示第一个台阶跳到第二个以此类推)
    //目标点位(在HeadNodesPar节点下的目标位置)
    private readonly Vector2[] mTargetPos = new[]
    {
         new Vector2(-371, 19),
         new Vector2(-216, 97),
         new Vector2(-120, 172),
         new Vector2(342, 31),
         new Vector2(243, 121),
         new Vector2(21, 163),
         new Vector2(-220, 125),
    };
    //岩浆点位(在HeadNodesPar节点下的目标位置)
    private readonly Vector2[][] mLavaPos = new[]
    {
        new Vector2[] { new (-260, 91), new (-141, 105) },
        new Vector2[] { new (-248, 0), new (-50, 100) },
        new Vector2[] { new (-139, 70), new (0, 96) },
        new Vector2[] { new (160, 74), new (272, -74) },
        new Vector2[] { new (55, 111), new (213, 0) },
        new Vector2[] { new (-120, 65), new (140, 80) },
        new Vector2[] { new (-299, 14), new (-170, -9) },
    };

    private List<Transform> mHeadNodes;
    private List<SkeletonGraphic> mBurnSpineList;

    [SerializeField] private float height = 100f;
    [SerializeField] private float duration = 1f;

    private void Awake()
    {
        mHeadNodes = new List<Transform>();
        mBurnSpineList = new List<SkeletonGraphic>();

        foreach (Transform item in transform)
        {
            mHeadNodes.Add(item.GetChild(0));
            mBurnSpineList.Add(item.GetChild(1).GetComponent<SkeletonGraphic>());
        }

        foreach (SkeletonGraphic item in mBurnSpineList)
        {
            item.AnimationState.Complete += (trackEntry) =>
            {
                item.Hide();
            };
            item.Hide();
        }
    }

    private void OnDestroy()
    {
        foreach (var tween in mTweens)
        {
            if (tween.IsActive()) tween.Kill();
        }
        mTweens.Clear();
    }

    public void Jump(int tempStep ,Action lastStepCall)
    {
        //设定3或4人跳过
        int _temp = UnityEngine.Random.Range(3, 5);
        int _count = mHeadNodes.Count;
        int _fail = 0;

        for (int i = _count - 1; i >= 0; i--)
        {
            Action _action = null;
            int _tempIndex = i;
            Vector2 _targetPos;

            if (_tempIndex >= _count - _temp)
            {
                Vector2 _baseTarget = mTargetPos[tempStep];
                float _offsetX = UnityEngine.Random.Range(-TARGET_POS_OFFSET_X, TARGET_POS_OFFSET_X);
                float _offsetY = UnityEngine.Random.Range(-TARGET_POS_OFFSET_Y, TARGET_POS_OFFSET_Y);

                _targetPos = _baseTarget + new Vector2(_offsetX, _offsetY);
            }
            else
            {
                Vector2 _baseTarget = mLavaPos[tempStep][_fail];
                float _offsetX = UnityEngine.Random.Range(-LAVA_POS_OFFSET_X, LAVA_POS_OFFSET_X);
                float _offsetY = UnityEngine.Random.Range(-LAVA_POS_OFFSET_Y, LAVA_POS_OFFSET_Y);
                _targetPos = _baseTarget + new Vector2(_offsetX, _offsetY);
                ++_fail;
                //调入岩浆回调
                _action = () =>
                {
                    mBurnSpineList[_tempIndex].Show();
                    Tween _tween = mHeadNodes[_tempIndex].DOLocalMoveY(-110f, 3f);
                    mTweens.Add(_tween);
                    mBurnSpineList[_tempIndex].AnimationState.SetAnimation(0, "idle", false);
                };
            }

            bool isLast = (i == 0); 
            JumpToTarget(mHeadNodes[_tempIndex].parent, _targetPos, _action, isLast ? lastStepCall : null);
        }
    }

    //抛物线运动
    private void JumpToTarget(Transform Tran, Vector2 targetPos, Action action, Action lastStepCall)
    {
        Vector3 _startPos = Tran.localPosition;
        Vector3 _endPos = new (targetPos.x, targetPos.y, _startPos.z);

        Tween _tween = DOTween.To(() => 0f, t =>
        {
            float x = Mathf.Lerp(_startPos.x, _endPos.x, t);
            float y = Mathf.Lerp(_startPos.y, _endPos.y, t);

            //抛物线函数(0.5s处为顶点)
            float _parabolaOffset = 4 * t * (1 - t) * height;

            Tran.localPosition = new Vector3(x, y + _parabolaOffset, _startPos.z);

        }, 1f, duration).SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            action?.Invoke();
            lastStepCall?.Invoke();
        });

        mTweens.Add(_tween); 
    }
}
