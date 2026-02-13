using System;
using System.Linq;
using Game.Water;
using QFramework;
using UnityEngine;

namespace Game.Water
{
    public class UIVictoryData : UIPanelData
    {
    }

    public partial class UIVictory : UIPanel, ICanSendEvent, ICanGetUtility, ICanGetModel
    {
        //16关开始，每两关一个广告，50关之后，每关一个
        private const int START_AD_LIMIT = 16;
        private const int START_AD_LIMIT2 = 50;

        private SaveDataUtility saveDataUtility;
        [SerializeField] private Sprite[] unlockSprites;
        private readonly int[] UNLOCKLEVEL = new int[]
            { 3, 61, 71, 81, 91, 101, 111, 121, 131, 141, 151, 171, 191, 211, 231, 251, 351, 451 };

        public Material TMPFont_red;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIVictoryData ?? new UIVictoryData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            saveDataUtility = this.GetUtility<SaveDataUtility>();
        }

        protected override void OnShow()
        {
            ShowAnim();
            BtnSkip.onClick.AddListener(() =>
            {
                UnlockNewItem();
            });
            WaitClose();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnSkip.onClick.RemoveAllListeners();
        }

        private void WaitClose()
        {
            ActionKit.Delay(3f, () =>
            {
                UnlockNewItem();
            }).Start(this);
        }

        private void ShowAnim()
        {
            //目前不播放
            //AnimGo.Play("victoryAnim");
            Horn.Show();
            HornGo1.Play("hornRotation");
            HornGo2.Play("hornRotation");
            HornGo3.Play("hornRotation");
            HornGo4.Play("hornRotation");

            HornSpine1.AnimationState.SetAnimation(0, "animation", false);

            HornSpine2.AnimationState.SetAnimation(0, "animation", false);
            HornSpine3.AnimationState.SetAnimation(0, "animation", false);
            var track = HornSpine4.AnimationState.SetAnimation(0, "animation", false);
        }

        private void UnlockNewItem()
        {
            int curLevel = saveDataUtility.GetCurrentLevel();

            if (curLevel > START_AD_LIMIT)
            {
                if (curLevel > START_AD_LIMIT2)
                    TopOnADManager.Instance.ShowIntersAd(null, null);

                else if (curLevel % 2 == 1)
                    TopOnADManager.Instance.ShowIntersAd(null, null);
            }

            Horn.Hide();
            if (UNLOCKLEVEL.Contains(curLevel))
            {
                int _idx = Array.IndexOf(UNLOCKLEVEL, curLevel);
                NewItemNode.Show();
                TxtNewItem.fontSharedMaterial = TMPFont_red;
                TxtNewItem.Hide();
                TxtNewItem.Show();
                TxtNewItem.text = GameConst.GameplayTutorialInfo[curLevel].guideInfo;
                ImgNewItem.sprite = unlockSprites[_idx];
                ImgNewItem.SetNativeSize();

                BtnNewItemClose.onClick.AddListener(() =>
                {
                    NewItemNode.Hide();
                    LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
                    CloseSelf();
                });
            }
            else
            {
                LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
                CloseSelf();
            }
        }
    }
}
