using UnityEngine;
using QFramework;
using UnityEngine.UI;
using JsonFileData;
using System;
using System.Linq;
using TMPro;
using DG.Tweening;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class BattlePassPanel : ViewController, ICanGetModel
    {
        private const string BPViP_GIFT_ID = "battlepass_vip";

        private BattlePassModel battlePassModel;
        private BattlePassADActivity mBattlePassADActivity;
        private Tween mCountDownTween;
        private Sequence topImageFillSequence;
        private Sequence buttomImageFillSequence;
        private int oldLevel = 0;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        private void Awake()
        {
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
            battlePassModel = this.GetModel<BattlePassModel>();

            InitUI();
        }

        private void OnEnable()
        {
            topImageFillSequence = DOTween.Sequence();
            buttomImageFillSequence = DOTween.Sequence();

            UpdateUI();
        }

        private void OnDisable()
        {
            mCountDownTween?.Kill();
            mCountDownTween = null;
            topImageFillSequence?.Kill();
            buttomImageFillSequence?.Kill();
            topImageFillSequence = null;
            buttomImageFillSequence = null;
            
            // 刷新Top面板的结果
            ImgBar.fillAmount = (float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions;
            ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = battlePassModel.RewardLevel.ToString();
            TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";

            // 边界情况
            if (GameDefine.GameConst.MAX_INT == battlePassModel.CurrentGetConditions)
            {
                ImgBar.fillAmount = 1;
                ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "30";
                TextTaskProgressBar.text = "Complete";
            }

            //刷新Buttom面板的结果
            for (int i = oldLevel; i < battlePassModel.RewardLevel; i++)
            {
                // 刷新进度条
                BattlePassContent.transform.GetChild(i).GetComponent<BattlePassContentPanel>().SetDividingLine(0, 0);
                // 刷新
                BattlePassContent.transform.GetChild(i).GetComponent<BattlePassContentPanel>().SetProgressBar(1);
                // 刷新宝箱状态
                BattlePassContent.transform.GetChild(i).GetComponent<BattlePassContentPanel>().UpdateUI(i);
            }

            GC.Collect();
        }

        private void Start()
        {
            BintEvent();
            SetBtnClick();
        }

        #region 初始化的时候调用
        public void InitUI()
        {
            InitButtomPanelUI();
            InitTopPanelUI();
        }

        public void InitTopPanelUI()
        {
            ImgBar.fillAmount = (float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions;
            ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = battlePassModel.RewardLevel.ToString();
            TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";
            BtnActivate.interactable = !battlePassModel.IsVip;
        }

        public void InitButtomPanelUI()
        {
            GameObject _prefab = BattlePassContent.transform.GetChild(0).gameObject;
            // 战令预制体增删
            for (int i = BattlePassContent.transform.childCount; i < battlePassModel.BPDate.Rewards.Length; i++)
                Instantiate(_prefab, BattlePassContent.transform);
            for (int i = battlePassModel.BPDate.Rewards.Length; i < BattlePassContent.transform.childCount; i++)
                BattlePassContent.transform.GetChild(i).gameObject.Hide();

            // 战令内容的设置
            for (int i = 0; i < battlePassModel.BPDate.Rewards.Length; i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().Initialize(_level);
            }
        }
        #endregion
        
        #region 每次进入面板时调用

        private void UpdateUI()
        {
            oldLevel = int.Parse(ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text);
            if (oldLevel == battlePassModel.BPDate.Rewards.Length - 1)
            {
                SmoothScrollController(oldLevel - 1);
                return;
            }
            UpTopPanelUI();
            UpButtomPanelUI();
        }

        private void UpTopPanelUI()
        {
            float oldFillAmout = ImgBar.fillAmount;

            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mBattlePassADActivity.ActivityStatus == GameActivityStatus.Active)
                    TxtCountDown.text = mBattlePassADActivity.GetActivityReamingTime();
                else
                    TxtCountDown.text = "Completeed";
            }, 1, 1f)
           .SetLoops(-1, LoopType.Restart)
           .SetUpdate(true);

            #region 顶部进度条动画


            for (int level = oldLevel + 1; level <= battlePassModel.RewardLevel; level++)
            {
                int _level = level;
                Tween fillTween = ImgBar.DOFillAmount(1, 1f / (battlePassModel.RewardLevel - oldLevel))
                    .SetEase(Ease.Linear)
                    .Pause()
                    .OnStepComplete(() =>
                    {
                        ImgBar.fillAmount = 0;
                        ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _level.ToString();
                        TextTaskProgressBar.text = $"{battlePassModel.BPDate.Rewards[_level].GetConditions}/{battlePassModel.BPDate.Rewards[_level].GetConditions}";
                    });

                topImageFillSequence.Append(fillTween);
            }

            topImageFillSequence.AppendCallback(() =>
            {

                ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = (battlePassModel.RewardLevel).ToString();
                TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";

            });
            Tween endFillTween = ImgBar.DOFillAmount((float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions, 1f / (battlePassModel.RewardLevel - oldLevel))
                   .SetEase(Ease.Linear)
                   .Pause()
                   .OnComplete(() =>
                   {
                       buttomImageFillSequence.Play();
                   });
            topImageFillSequence.Append(endFillTween);
            topImageFillSequence.OnComplete(() =>
            {
                if (GameDefine.GameConst.MAX_INT == battlePassModel.CurrentGetConditions)
                {
                    ImgBar.fillAmount = 1;
                    ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "30";
                    TextTaskProgressBar.text = "Comolete";
                }
            });
            topImageFillSequence.Play();

            #endregion
        }

        private void UpButtomPanelUI()
        {
            // 设置旧的奖励条
            BattlePassContent.transform.GetChild(oldLevel).GetComponent<BattlePassContentPanel>().SetDividingLine(0);
            buttomImageFillSequence = DOTween.Sequence();
            // 设置进度条动画和面板内容
            for (int i = oldLevel; i < battlePassModel.RewardLevel; i++)
            {
                int _level = i;
                Tween _tween = BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level, 1.0f / (battlePassModel.RewardLevel - oldLevel));
                buttomImageFillSequence.Append(_tween);
                if (i == (oldLevel + battlePassModel.RewardLevel) / 2)
                    SmoothScrollController(battlePassModel.RewardLevel);
            }
            buttomImageFillSequence.OnComplete(() =>
            {
                // 设置新奖励条
                BattlePassContent.transform.GetChild(battlePassModel.RewardLevel).GetComponent<BattlePassContentPanel>().SetDividingLine(1);
                // 战令内容的更新
                for (int i = 0; i < battlePassModel.BPDate.Rewards.Length; i++)
                {
                    int _level = i;
                    BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level);
                }
            });
        }

        #endregion

        private void BintEvent()
        {
            StringEventSystem.Global.Register(BPViP_GIFT_ID, OnPaySuccess)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void SetBtnClick()
        {
            BtnActivate.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIBattlePassPay>(UILevel.PopUI);
            });

            BtnClose.onClick.AddListener(() => { UIKit.GetPanel<UIBegin>().MenuBtnEvent(2); });
        }
        
        /// <summary>
        /// 礼包购买回调
        /// </summary>
        private void OnPaySuccess()
        {
            battlePassModel.HightBattlePassActivation();
            BtnActivate.interactable = !battlePassModel.IsVip;

            // 战令内容的更新
            for (int i = 0; i < battlePassModel.BPDate.Rewards.Length; i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level);
            }
            UIKit.OpenPanel<UIBuyPackSuccess>();
        }

        private void SmoothScrollController(int index)
        {
            // 
            if (index >= battlePassModel.BPDate.Rewards.Length)
                index = battlePassModel.BPDate.Rewards.Length - 1;
            RectTransform targetElement = ScrollView.content.GetChild(index) as RectTransform;
            Vector2 targetPosition = GetSnapToPositionToBringChildIntoView(targetElement);

            // 使用DOTween平滑滚动
            DOTween.To(
                () => ScrollView.content.anchoredPosition,
                x => ScrollView.content.anchoredPosition = x,
                targetPosition,
                1f
            ).SetEase(Ease.OutCubic);
        }
        
        private Vector2 GetSnapToPositionToBringChildIntoView(RectTransform child)
        {
            Canvas.ForceUpdateCanvases();

            Vector2 viewportLocalPosition = ScrollView.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;

            Vector2 result = new Vector2(
                0,
                -viewportLocalPosition.y - childLocalPosition.y - (child.rect.height / 2)
            );
            return result;
        }
    }
}
