using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

namespace QFramework.Example
{
    public class SceneUnlock1Data : UIPanelData
	{
	}

	public partial class SceneUnlock1 : UIPanel, ICanGetModel, ICanGetUtility, ICanSendEvent
	{
        private const int PANEL_ID = 0;

        private readonly List<Spine.AnimationState.TrackEntryDelegate> mOnCompleteHandlers = new();
        //部件消耗和按钮位置
        private readonly (int, Vector2)[] mUnitMes = new (int, Vector2)[]
        {
              (1, new Vector2(100, 450)),
              (1, new Vector2(-200, 150)),
              (1, new Vector2(50, 200)),
              (1, new Vector2(300, 200)),
              (1, new Vector2(-400, -500)),
              (2, new Vector2(200, -500)),
              (2, new Vector2(250, 300)),
              (2, new Vector2(-100, 50)),
        };
        //完成前、后待机
        private readonly int[] mStandbyBeforeCompleteSpineIdx = new int[] { 1, 2, 7 };
        private readonly int[] mStandbyAfterCompleteSpineIdx = new int[] { 3};

        [SerializeField] private SkeletonGraphic[] mAllUnitSpines;
        [SerializeField] private CanvasGroup[] mSpineCanvasGroups;
        [SerializeField] private Image[] mUnitImgs;
        [Header("Unlock completed sprites")]
        [SerializeField] private Sprite[] mUnitSprites;
        [SerializeField] private Sprite[] mUnitIconSprites;
        [SerializeField] private Sprite mBoxOpenSprite;
        [SerializeField] private Transform[] mProgressNodes;
        [SerializeField] private GiftPackSO mRewardPackSO;
        [SerializeField] private Image mBgUnitImg;

        private Spine.AnimationState.TrackEntryDelegate mHeartRiseCallBack;
        private SceneUnlockModel mSceneUnlockModel;
        private RewardGrantUtility mRewardGrantUtility;
        //用于维护Spine的回调监听和注销
        private SkeletonGraphic[] mUnActiveUnitSpines;
        private int mStartUnitIndex;
        private bool mRewardSign;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SceneUnlock1Data ?? new SceneUnlock1Data();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            mSceneUnlockModel = this.GetModel<SceneUnlockModel>();
            mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();
            GameDefine.GameUtils.SotrArray(mAllUnitSpines);
            GameDefine.GameUtils.SotrArray(mUnitImgs);
            GameDefine.GameUtils.SotrArray(mSpineCanvasGroups);

            mStartUnitIndex = Mathf.Clamp(mSceneUnlockModel.GetSceneUnitIndex(PANEL_ID), 0, mUnitMes.Length);

            InitPanel();
            InitSpineOnComplete();
        }

        protected override void OnShow()
		{
            BtnBox.onClick.AddListener(() =>
            {
                ImgReward.gameObject.SetActive(!ImgReward.gameObject.activeSelf);
            });

            BtnUnitUnlock.onClick.AddListener(() =>
            {
                int _index = mSceneUnlockModel.SceneUnlockUnitIndex;
                int _realIndex = _index - (mAllUnitSpines.Length - mUnActiveUnitSpines.Length);
                
                if (mSceneUnlockModel.RemainingStars >= mUnitMes[_index].Item1)
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

                        SpineHeartRise.GetComponent<CanvasGroup>().alpha = 1f;
                        SpineHeartRise.AnimationState.SetAnimation(0, "tx", false);
                    });
                }
                else
                {
                    UIKit.OpenPanel<UILessStar>(new UILessStarData { CurPanel = this});
                }
            });

            BtnReturen.onClick.AddListener(() =>
            {
                this.SendEvent(new UnlockSceneBackEvent());
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
            SpineHeartRise.AnimationState.Complete -= mHeartRiseCallBack;

            BtnUnitUnlock.onClick.RemoveAllListeners();
            BtnReturen.onClick.RemoveAllListeners();
            BtnBox.onClick.RemoveAllListeners();

            mSceneUnlockModel = null;
            mRewardGrantUtility = null;
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
            //宝箱状态判定
            if (mSceneUnlockModel.SceneUnlockUnitIndex >= mUnitMes.Length)
                BtnBox.image.sprite = mBoxOpenSprite;

            //隐藏特效
            FlightEffectsToBox.Hide();
            FlightEffectsToBtn.Hide();

            //部件
            for (int i = 0; i < mStartUnitIndex; i++)
            {
                mUnitImgs[i].sprite = mUnitSprites[i];
                if (mUnitImgs[i] != mBgUnitImg)
                    mUnitImgs[i].SetNativeSize();
            }

            //进度
            for (int i = 0; i < mStartUnitIndex; i++)
            {
                mProgressNodes[i].Find("Over").Show();
            }

            //星星数
            TxtRemainStar.text = mSceneUnlockModel.RemainingStars.ToString();

            //按钮
            if (mStartUnitIndex >= mUnitMes.Length)
                BtnUnitUnlock.Hide();
            else
            {
                BtnUnitUnlock.Show();
                ImgUnitIcon.sprite = mUnitIconSprites[mStartUnitIndex];
                BtnUnitUnlock.transform.localPosition = mUnitMes[mStartUnitIndex].Item2;
                TxtNeedStar.text = mUnitMes[mStartUnitIndex].Item1.ToString();
            }

            //未激活Spine
            int remaining = mAllUnitSpines.Length - mStartUnitIndex;
            mUnActiveUnitSpines = new SkeletonGraphic[remaining];
            System.Array.Copy(mAllUnitSpines, mStartUnitIndex, mUnActiveUnitSpines, 0, remaining);

            //待机Spine(完成前)
            for (int i = 0; i < mStandbyBeforeCompleteSpineIdx.Length; i++)
            {
                if ((mSceneUnlockModel.SceneUnlockUnitIndex <= mStandbyBeforeCompleteSpineIdx[i]))
                {
                    mSpineCanvasGroups[mStandbyBeforeCompleteSpineIdx[i]].alpha = 1;
                    mAllUnitSpines[mStandbyBeforeCompleteSpineIdx[i]].AnimationState.SetAnimation(0, "daiji", true);
                    mUnitImgs[mStandbyBeforeCompleteSpineIdx[i]].Hide();
                }
            }
            //待机Spine(完成后)
            for (int i = 0; i < mStandbyAfterCompleteSpineIdx.Length; i++)
            {
                if ((mSceneUnlockModel.SceneUnlockUnitIndex > mStandbyAfterCompleteSpineIdx[i]))
                {
                    mSpineCanvasGroups[mStandbyAfterCompleteSpineIdx[i]].alpha = 1;
                    mAllUnitSpines[mStandbyAfterCompleteSpineIdx[i]].AnimationState.SetAnimation(0, "daiji", true);
                    mUnitImgs[mStandbyAfterCompleteSpineIdx[i]].Hide();
                }
            }
        }

        /// <summary>
        /// 注册Spine回调
        /// </summary>
        private void InitSpineOnComplete()
        {
            for (int i = 0; i < mUnActiveUnitSpines.Length; i++)
            {
                int _realIndex = mStartUnitIndex + i;
                int _tempIndex = i;
                void handler(TrackEntry trackEntry)
                {
                    if (trackEntry.Animation.Name == "daiji") return;
                    FlightEffectsTo_Box(_realIndex);

                    mUnActiveUnitSpines[_tempIndex].Hide();
                    mUnitImgs[_realIndex].Show();
                    mUnitImgs[_realIndex].sprite = mUnitSprites[_realIndex];
                    if (mUnitImgs[_realIndex] != mBgUnitImg)
                        mUnitImgs[_realIndex].SetNativeSize();

                    UpdateUnlockBtn();

                    if (mStandbyAfterCompleteSpineIdx.Contains(_realIndex))
                    {
                        mAllUnitSpines[_realIndex].Show();
                        mAllUnitSpines[_realIndex].AnimationState.SetAnimation(0, "daiji", true);
                        mUnitImgs[_realIndex].Hide();
                    }
                }
                mUnActiveUnitSpines[i].AnimationState.Complete += handler;
                mOnCompleteHandlers.Add(handler);
            }

            mHeartRiseCallBack = _ =>
            {
                SpineHeartRise.GetComponent<CanvasGroup>().alpha = 0f;
                SpineHeartRise.AnimationState.SetEmptyAnimation(0, 0);
                SpineHeartRise.AnimationState.GetCurrent(0).TrackTime = 0f;
            };
            SpineHeartRise.AnimationState.Complete += mHeartRiseCallBack;
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
            FlightEffectsToBox.position = mAllUnitSpines[realIndex].transform.position;
            //FlightEffectsToBox.localPosition = mEffectToBoxStartPos[realIndex];
            FlightEffectsToBox.DOMove(BtnBox.transform.position, 0.5f).OnComplete(() =>
            {
                FlightEffectsToBox.Hide();
                mProgressNodes[realIndex].Find("Over").Show();

                //�������
                if (mRewardSign)
                {
                    mRewardSign = false;
                    UIKit.ClosePanel<UIMask>();
                    RewardUIManager.Instance.PlayRewardAnim(mRewardPackSO.Coins, true, null, mRewardPackSO);
                    BtnBox.image.sprite = mBoxOpenSprite;
                }
            });
        }

        private void FlightEffectsTo_UnlockBtn(int realIndex, System.Action action)
        {
            if (mSceneUnlockModel.SceneUnlockUnitIndex >= mUnitMes.Length)
            {
                mRewardSign = true;
                //mSceneUnlockModel.AddSceneIndex();
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
                //��������
                mRewardGrantUtility.GrantReward(mRewardPackSO);
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
                TxtRemainStar.text = mSceneUnlockModel.RemainingStars.ToString();

            }).Start(this);
        }
    }
}
