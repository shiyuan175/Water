using GameDefine;
using QFramework;
using Spine.Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class UIPaidItemsGuideData : UIPanelData
    {
        public NormalRewardsType PropType;
    }

    public partial class UIPaidItemsGuide : UIPanel, ICanGetModel, ICanSendEvent
    {
        [SerializeField] private TextMeshProUGUI[] mTxtRed;

        private const int DelItemCount = 1;
        private readonly Dictionary<NormalRewardsType, string> PropRules = new Dictionary<NormalRewardsType, string>
        {
            { NormalRewardsType.StepBack, "Take a step back" },
            { NormalRewardsType.RemoveHide, "Remove the black mark" },
            { NormalRewardsType.AddOneBottle, "Add one waterbottle" },
            { NormalRewardsType.AddHalfBottle, "Add one grid of waterbottle" },
            { NormalRewardsType.RemoveAll, "Remove all negative effects" },
        };

        private GameGlobalModel mGameGlobalModel;

        // ���˵��ߵ�����
        private bool StepBackGuideStep1 = false;
        private bool StepBackGuideStep2 = false;
        private Button BtnStepBackGuide1;
        private Button BtnStepBackGuide2;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIPaidItemsGuideData ?? new UIPaidItemsGuideData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            mGameGlobalModel = this.GetModel<GameGlobalModel>();

            InitFont();
            InitUI();
            BindBtn();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (StepBackGuideStep1
                    && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnStepBackGuide1.gameObject))
                {
                    StepBackGuideStep1 = false;
                    BtnStepBackGuide1.onClick?.Invoke();

                    GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnStepBackGuide2.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                    StepBackGuideStep2 = true;
                }

                else if (StepBackGuideStep2
                    && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnStepBackGuide2.gameObject))
                {
                    StepBackGuideStep2 = false;
                    BtnStepBackGuide2.onClick?.Invoke();
                    SpineHandle.Hide();

                    //�ȴ���ˮ����
                    ActionKit.Delay(1.5f, () =>
                    {
                        GetItemGuideNode.Show();
                    }).Start(this);
                }
            }
        }

        private void InitFont()
        {
            TxtTitle_Red.font = LevelManager.Instance.redFont;
            TxtItemDetails_Red.font = LevelManager.Instance.redFont;

            foreach (var item in mTxtRed)
            {
                item.font = LevelManager.Instance.redFont;
            }
        }

        private void InitUI()
        {
            //�ڵ�����
            if (mData.PropType == NormalRewardsType.StepBack)
            {
                GetItemGuideNode.Hide();

                BtnStepBackGuide1 = LevelManager.Instance.nowBottles[0].bottle;
                BtnStepBackGuide2 = LevelManager.Instance.nowBottles[7].bottle;

                StepBackGuideStep1 = true;

                SpineHandle.Show();
                GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnStepBackGuide1.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            }
            else
                GetItemGuideNode.Show();

            //�ı���ֵ
            TxtItemDetails_Red.text = PropRules[mData.PropType];
            ImgItem.sprite = RewardUIManager.Instance.GetRewardSprite(mData.PropType);
        }

        private void BindBtn()
        {
            BtnGet.onClick.AddListener(() =>
            {
                GetItemGuideNode.Hide();
                mGameGlobalModel.AddItem((int)mData.PropType, DelItemCount);

                BtnUseGuide.Show();
                BtnUseGuide.transform.position = GetGuideBtnComp().transform.position;

                SpineHandle.Show();
                var current = SpineHandle.AnimationState.GetCurrent(0);
                //���ⱻ����
                if (current == null || current.Animation.Name != "animation")
                    SpineHandle.AnimationState.SetAnimation(0, "animation", true);

                SpineHandle.transform.position = BtnUseGuide.transform.position;
            });

            BtnUseGuide.onClick.AddListener(() =>
            {
                mGameGlobalModel.ReduceItem((int)mData.PropType, 1);
                UseItem();
                CloseSelf();

                this.SendEvent(new UnLockItem
                {
                    PropType = mData.PropType,
                });
            });
        }

        private Image GetGuideBtnComp()
        {
            Image img = null;
            switch (mData.PropType)
            {
                case NormalRewardsType.StepBack:
                    img = UIKit.GetPanel<UIGameNode>().BtnStepBack.image;
                    break;
                case NormalRewardsType.RemoveHide:
                    img = UIKit.GetPanel<UIGameNode>().BtnRemoveHide.image;
                    break;
                case NormalRewardsType.AddOneBottle:
                    img = UIKit.GetPanel<UIGameNode>().BtnAddBottle.image;
                    break;
                case NormalRewardsType.AddHalfBottle:
                    img = UIKit.GetPanel<UIGameNode>().BtnHalfBottle.image;
                    break;
                case NormalRewardsType.RemoveAll:
                    img = UIKit.GetPanel<UIGameNode>().BtnRemoveAll.image;
                    break;
            }
            return img;
        }

        private void UseItem()
        {
            switch (mData.PropType)
            {
                case NormalRewardsType.StepBack:
                    LevelManager.Instance.ReturnLast();
                    break;
                case NormalRewardsType.RemoveHide:
                    LevelManager.Instance.RemoveHide();
                    break;
                case NormalRewardsType.AddOneBottle:
                    LevelManager.Instance.AddBottle(false);
                    break;
                case NormalRewardsType.AddHalfBottle:
                    LevelManager.Instance.AddBottle(true);
                    break;
                case NormalRewardsType.RemoveAll:
                    LevelManager.Instance.RemoveAll();
                    break;
            }

        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
