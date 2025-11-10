using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using Spine.Unity;
using UnityEngine;
using VolcanicActivityData;

public class VA_HeadNodesCtrl : MonoBehaviour
{
    private const int TARGET_POS_OFFSET_X = 30;
    private const int TARGET_POS_OFFSET_Y = 20;
    private const int LAVA_POS_OFFSET_X = 20;
    private const int LAVA_POS_OFFSET_Y = 10;

    private readonly List<Tween> mTweens = new();
    private List<Transform> mHeadNodes;
    private List<SkeletonGraphic> mBurnSpineList;

    [SerializeField] private float height = 1f;
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

    public void Jump(Vector3 targetPos, LavaPosStruct lavaPosStruct, Action lastStepCall)
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
                Vector2 _baseTarget = targetPos;
                float _offsetX = UnityEngine.Random.Range(-TARGET_POS_OFFSET_X, TARGET_POS_OFFSET_X);
                float _offsetY = UnityEngine.Random.Range(-TARGET_POS_OFFSET_Y, TARGET_POS_OFFSET_Y);
                // 偏移值(+50是父节点与台阶初始偏移值,而直接操作头像跳到台阶上,需加上50偏移值)
                Vector2 _localOffset = new Vector3(_offsetX, _offsetY + 50);
                // 偏移值转换成世界坐标偏移
                Vector3 _worldOffset = mHeadNodes[_tempIndex].parent.TransformVector(_localOffset);
                _targetPos = (_baseTarget + (Vector2)_worldOffset);
            }
            else
            {
                Vector2 _baseTarget = _fail == 0 ? lavaPosStruct.LavaTranPos1.position : lavaPosStruct.LavaTranPos2.position;
                float _offsetX = UnityEngine.Random.Range(-LAVA_POS_OFFSET_X, LAVA_POS_OFFSET_X);
                float _offsetY = UnityEngine.Random.Range(-LAVA_POS_OFFSET_Y, LAVA_POS_OFFSET_Y);
                Vector2 _localOffset = new Vector3(_offsetX, _offsetY);
                Vector3 _worldOffset = mHeadNodes[_tempIndex].parent.TransformVector(_localOffset);
                _targetPos = (_baseTarget + (Vector2)_worldOffset);
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
        Vector3 _startPos = Tran.position;
        Vector3 _endPos = new(targetPos.x, targetPos.y, _startPos.z);

        Tween _tween = DOTween.To(() => 0f, t =>
        {
            float x = Mathf.Lerp(_startPos.x, _endPos.x, t);
            float y = Mathf.Lerp(_startPos.y, _endPos.y, t);

            //抛物线函数(0.5s处为顶点)
            float _parabolaOffset = 4 * t * (1 - t) * height;
            Tran.position = new Vector3(x, y + _parabolaOffset, _startPos.z);

        }, 1f, duration).SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            action?.Invoke();
            lastStepCall?.Invoke();
        });

        mTweens.Add(_tween);
    }
}
