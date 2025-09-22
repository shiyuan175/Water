using QFramework;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class UIGuideLevel1And2Data : UIPanelData
    {
        public int level;
    }

    public partial class UIGuideLevel1And2 : UIPanel
    {
        [SerializeField] private Sprite sprite_T;
        [SerializeField] private Sprite sprite_F;

        private const int FIRST = 1;
        private const int SECOND = 2;

        private bool level1_Step1 = false;
        private bool level1_Step2 = false;
        private Button BtnLevel1_Step1;
        private Button BtnLevel1_Step2;

        private bool level2_Step1 = false;
        private bool level2_Step2 = false;
        private bool level2_Step3 = false;
        //标记第二步是否执行过
        private bool alStep2 = false;
        private Button BtnLevel2_Step1;
        private Button BtnLevel2_Step2;
        private Button BtnLevel2_Step3;


        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGuideLevel1And2Data ?? new UIGuideLevel1And2Data();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            InitUI();
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

        private void Update()
        {
            if (mData.level == FIRST)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (level1_Step1
                        && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnLevel1_Step1.gameObject))
                    {
                        level1_Step1 = false;
                        BtnLevel1_Step1.onClick?.Invoke();

                        GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel1_Step2.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                        level1_Step2 = true;
                    }

                    else if (level1_Step2
                        && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnLevel1_Step2.gameObject))
                    {
                        level1_Step2 = false;
                        BtnLevel1_Step2.onClick?.Invoke();
                        CloseSelf();
                    }
                }
            }

            else if (mData.level == SECOND)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (level2_Step1
                        && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnLevel2_Step1.gameObject))
                    {
                        level2_Step1 = false;
                        BtnLevel2_Step1.onClick?.Invoke();
                        if (!alStep2)
                        {
                            GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel2_Step2.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                            Img_Left.sprite = sprite_T;
                            Img_Right.sprite = sprite_F;
                            level2_Step2 = true;
                        }
                        else
                        {
                            GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel2_Step3.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                            Img_Left.sprite = sprite_F;
                            Img_Right.sprite = sprite_T;
                            level2_Step3 = true;
                        }
                        Level2_Guide_Node.Show();
                    }

                    else if (level2_Step2
                        && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnLevel2_Step2.gameObject))
                    {
                        alStep2 = true;
                        level2_Step2 = false;
                        Level2_Guide_Node.Hide();
                        BtnLevel2_Step2.onClick?.Invoke();
                        SpineHandle.Hide();

                        //等待倒完水
                        ActionKit.Delay(2.2f, () =>
                        {
                            GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel2_Step1.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                            SpineHandle.Show();
                            level2_Step1 = true;
                        }).Start(this);

                    }

                    else if (level2_Step3
                       && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, BtnLevel2_Step3.gameObject))
                    {
                        level2_Step3 = false;
                        BtnLevel2_Step3.onClick?.Invoke();
                        CloseSelf();
                    }
                }

            }
        }

        private void InitUI()
        {
            TxtGuide.font = LevelManager.Instance.blueFont;

            if (mData.level == FIRST)
            {
                TxtGuide.text = "Tap the right bottle to pour";
                BtnLevel1_Step1 = LevelManager.Instance.nowBottles[0].bottle;
                BtnLevel1_Step2 = LevelManager.Instance.nowBottles[1].bottle;
                GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel1_Step1.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                level1_Step1 = true;
            }
            else if (mData.level == SECOND)
            {
                TxtGuide.text = "Only pour water into the same colort";
                BtnLevel2_Step1 = LevelManager.Instance.nowBottles[1].bottle;
                BtnLevel2_Step2 = LevelManager.Instance.nowBottles[0].bottle;
                BtnLevel2_Step3 = LevelManager.Instance.nowBottles[2].bottle;
                GameUtilityManager.Instance.GetLocalPositionInCanvas(BtnLevel2_Step1.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());

                int _offsetY = 250;
                GameUtilityManager.Instance.GetLocalPositionInCanvas(
                    BtnLevel2_Step2.GetComponent<RectTransform>(),
                    Img_Left.GetComponent<RectTransform>(),
                    (val) =>
                    {
                        Img_Left.transform.localPosition += new Vector3(0, _offsetY, 0);
                    } );

                GameUtilityManager.Instance.GetLocalPositionInCanvas(
                    BtnLevel2_Step3.GetComponent<RectTransform>(),
                    Img_Right.GetComponent<RectTransform>(),
                    (val) =>
                    {
                        Img_Right.transform.localPosition += new Vector3(0, _offsetY, 0);
                    });
                level2_Step1 = true;
            }
        }
    }
}
