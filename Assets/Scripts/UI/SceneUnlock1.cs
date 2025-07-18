using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using DG.Tweening;

//优化点：
//不要用ab包，使用预制体，通过场景进度加载放主界面节点下
//将方法提取成基类，新场景可以继承。接口约束一些信息，比如场景ID
//现在的方案,一个场景要一个AB包面板，还要一个预制体用于主界面显示

namespace QFramework.Example
{
    public class SceneUnlock1Data : UIPanelData
	{
	}

	public partial class SceneUnlock1 : UIPanel, ICanGetModel, ICanSendEvent
	{
        private const int PANEL_ID = 0;

        private readonly List<Spine.AnimationState.TrackEntryDelegate> mOnCompleteHandlers = new();
        //部件消耗和按钮位置
        private readonly (int, Vector2)[] mUnitMes = new (int, Vector2)[]
        {
              (1, new Vector2(253, 488)),
              (2, new Vector2(-164, 227)),
              (2, new Vector2(218, 244)),
              (3, new Vector2(-77, -242)),
              (3, new Vector2(-312, -337)),
              (3, new Vector2(300, -135)),
              (4, new Vector2(170, 19)),
              (4, new Vector2(-224, -569)),
              (4, new Vector2(0, -541)),
              (5, new Vector2(123, -441)),
              (5, new Vector2(308, -469)),
              (5, new Vector2(-205, -687)),
              (6, new Vector2(0, -367)),
              (6, new Vector2(0, 242)),
              (7, new Vector2(0, -137))
        };
        //Spine完成后特效初始位置
        private readonly Vector2[] mEffectStartPos = new Vector2[]
        {
            new(-50, 580),
            new(-422, 128),
            new(-50, 0),
            new(-298, -171),
            new(-494, -314),
            new(463, -324),
            new(292, 64),
            new(-75, -680),
            new(-431, -771),
            new(227, -689),
            new(442, -740),
            new(-357,528),
            new(0, -647),
            new(0, 100),
            new(130, -257),

        };
        //特效目标位置(宝箱)
        private readonly Vector2 mEffectBoxTargetPos = new (500, 900);

        [SerializeField] private SkeletonGraphic[] mAllUnitSpines;
        [SerializeField] private CanvasGroup[] mSpineCanvasGroups;
        [SerializeField] private Image[] mUnitImgs;
        [SerializeField] private Sprite[] mUnitSprites;
        [SerializeField] private Sprite[] mUnitIconSprites;
        [SerializeField] private Sprite mBoxOpenSprite;
        [SerializeField] private Transform[] mProgressNodes;
        [SerializeField] private RewardPackSO mRewardPackSO;

        private SceneUnlockModel mSceneUnlockModel;
        private StageModel mStageModel;
        //用于维护Spine的回调监听和注销
        private SkeletonGraphic[] mUnActiveUnitSpines;
        private int startUnitIndex;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SceneUnlock1Data ?? new SceneUnlock1Data();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            mSceneUnlockModel = this.GetModel<SceneUnlockModel>();
            mStageModel = this.GetModel<StageModel>();
            GameDefine.GameUtils.SotrArray(mAllUnitSpines);
            GameDefine.GameUtils.SotrArray(mUnitImgs);
            GameDefine.GameUtils.SotrArray(mSpineCanvasGroups);

            startUnitIndex = Mathf.Clamp(mSceneUnlockModel.SceneUnlockUnitIndex, 0, mUnitMes.Length);
            if (mSceneUnlockModel.SceneIndex > PANEL_ID)
            {
                startUnitIndex = mUnitMes.Length;
                ImgBox.sprite = mBoxOpenSprite;
            }

            InitPanel();
            InitSpineOnComplete();
        }

        protected override void OnShow()
		{
            BtnUnitUnlock.onClick.AddListener(() =>
            {
                int _index = mSceneUnlockModel.SceneUnlockUnitIndex;
                int _realIndex = _index - (mAllUnitSpines.Length - mUnActiveUnitSpines.Length);
                
                if (mSceneUnlockModel.RemainingStar >= mUnitMes[_index].Item1)
                {
                    mSceneUnlockModel.AddUnitIndex();
                    mSceneUnlockModel.UseStar(mUnitMes[_index].Item1);
                    BtnUnitUnlock.interactable = false;

                    FlightEffectsTo_UnlockBtn(_index, () =>
                    {
                        BtnUnitUnlock.Hide();
                        mUnitImgs[_index].Hide();
                        mSpineCanvasGroups[_index].alpha = 1;
                        //mUnActiveUnitSpines[_realIndex].Show();
                        mUnActiveUnitSpines[_realIndex].AnimationState.SetAnimation(0, "animation", false);
                    });
                }
                else
                {
                    UIKit.OpenPanel<UILessStar>();
                }
            });

            BtnReturen.onClick.AddListener(() =>
            {
                this.SendEvent(new UnlockSceneEvent());
                CloseSelf();
            });
        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            for (int i = 0; i < mUnActiveUnitSpines.Length; i++)
            {
                mUnActiveUnitSpines[i].AnimationState.Complete -= mOnCompleteHandlers[i];
            }
            BtnUnitUnlock.onClick.RemoveAllListeners();
            BtnReturen.onClick.RemoveAllListeners();

            mSceneUnlockModel = null;
            mStageModel = null;
            mOnCompleteHandlers.Clear();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface; 
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitPanel()
        {
            //隐藏特效
            FlightEffectsToBox.Hide();
            FlightEffectsToBtn.Hide();

            //部件
            for (int i = 0; i < startUnitIndex; i++)
            {
                mUnitImgs[i].sprite = mUnitSprites[i];
                mUnitImgs[i].SetNativeSize();
            }

            //进度
            for (int i = 0; i < startUnitIndex; i++)
            {
                mProgressNodes[i].Find("Over").Show();
            }

            //星星数
            TxtRemainStar.text = mSceneUnlockModel.RemainingStar.ToString();

            //按钮
            if (startUnitIndex >= mUnitMes.Length)
                BtnUnitUnlock.Hide();
            else
            {
                BtnUnitUnlock.Show();
                ImgUnitIcon.sprite = mUnitIconSprites[startUnitIndex];
                BtnUnitUnlock.transform.localPosition = mUnitMes[startUnitIndex].Item2;
                TxtNeedStar.text = mUnitMes[startUnitIndex].Item1.ToString();
            }

            //未激活Spine
            int remaining = mAllUnitSpines.Length - startUnitIndex;
            mUnActiveUnitSpines = new SkeletonGraphic[remaining];
            System.Array.Copy(mAllUnitSpines, startUnitIndex, mUnActiveUnitSpines, 0, remaining);
        }

        /// <summary>
        /// 注册Spine回调
        /// </summary>
        private void InitSpineOnComplete()
        {
            for (int i = 0; i < mUnActiveUnitSpines.Length; i++)
            {
                int _realIndex = startUnitIndex + i;
                int _tempIndex = i;
                void handler(TrackEntry trackEntry)
                {
                    FlightEffectsTo_Box(_realIndex);

                    mUnActiveUnitSpines[_tempIndex].Hide();
                    mUnitImgs[_realIndex].Show();
                    mUnitImgs[_realIndex].sprite = mUnitSprites[_realIndex];
                    mUnitImgs[_realIndex].SetNativeSize();

                    UpdateUnlockBtn();
                }
                mUnActiveUnitSpines[i].AnimationState.Complete += handler;
                mOnCompleteHandlers.Add(handler);
            }
        }

        /// <summary>
        /// 解锁按钮位置更新
        /// </summary>
        private void UpdateUnlockBtn()
        {
            if (mSceneUnlockModel.SceneIndex > PANEL_ID)
            {
                BtnUnitUnlock.Hide();
                return;
            }

            int _index = mSceneUnlockModel.SceneUnlockUnitIndex;
            if (_index >= mUnitMes.Length)
            {
                BtnUnitUnlock.Hide();
                return;
            }
            BtnUnitUnlock.Show();
            BtnUnitUnlock.interactable = true;
            ImgUnitIcon.sprite = mUnitIconSprites[_index];
            BtnUnitUnlock.transform.localPosition = mUnitMes[_index].Item2;
            TxtNeedStar.text = mUnitMes[_index].Item1.ToString();
        }

        private void FlightEffectsTo_Box(int realIndex)
        {
            FlightEffectsToBox.Show();
            FlightEffectsToBox.localPosition = mEffectStartPos[realIndex];
            FlightEffectsToBox.DOLocalMove(mEffectBoxTargetPos, 0.5f).OnComplete(() =>
            {
                FlightEffectsToBox.Hide();
                mProgressNodes[realIndex].Find("Over").Show();

                //开箱判断，部件ID失效(已更新为下一个场景部件ID,要用场景ID判断)
                if (mSceneUnlockModel.SceneIndex > PANEL_ID)
                {
                    UIKit.ClosePanel<UIMask>();
                    StartCoroutine(RewardItemManager.Instance.PlayRewardAnim(mRewardPackSO, true));
                    ImgBox.sprite = mBoxOpenSprite;
                }
            });
        }

        private void FlightEffectsTo_UnlockBtn(int realIndex, System.Action action)
        {
            if (mSceneUnlockModel.SceneUnlockUnitIndex >= mUnitMes.Length)
            {
                mSceneUnlockModel.AddSceneIndex();
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
                //先发送奖励，在播放动画
                foreach (var item in mRewardPackSO.ItemReward)
                {
                    mStageModel.AddItem(item.ItemIndex, item.Quantity);
                }
                CoinManager.Instance.AddCoin(mRewardPackSO.Coins);
            }

            FlightEffectsToBtn.Show();
            FlightEffectsToBtn.position = TxtRemainStar.transform.position;
            ActionKit.Delay(0.1f, () =>
            {
                FlightEffectsToBtn.DOLocalMove(mUnitMes[realIndex].Item2, 0.5f).OnComplete(() =>
                {
                    FlightEffectsToBtn.Hide();
                    action?.Invoke();
                });
                TxtRemainStar.text = mSceneUnlockModel.RemainingStar.ToString();

            }).Start(this);
        }
    }
}
