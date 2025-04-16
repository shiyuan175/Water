using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.SocialPlatforms;

namespace QFramework.Example
{
	public class UIBeginData : UIPanelData
	{
	}
	public partial class UIBegin : UIPanel, ICanRegisterEvent, ICanGetUtility
    {
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

		public List<Animator> ButtonAnim;

		public GameObject BeginNode, LevelNode, SceneNode1, SceneNode2, SceneNode3, SceneNode4;
        public ScenePartCtrl ScenePart1, ScenePart2, ScenePart3, ScenePart4; 
        public ParticleTargetMoveCtrl coinFx, starFx;
		int nowButton = 2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIBeginData ?? new UIBeginData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtLevel.font.material.shader = Shader.Find(TxtLevel.font.material.shader.name);
        }

        protected override void OnShow()
        {
            BindBtn();
            RegisterEvent();
            SetText();
            SetCoin();
            SetStar();
            SetVitality();
            SetItem();
            InitBeginMenuButton();

            var levelNow = this.GetUtility<SaveDataUtility>().GetLevelClear();
            if (levelNow <= 5)
            {
                LevelNode.SetActive(true);
                BeginNode.SetActive(false);
            }
            SetScene();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        //��ť����
        void BindBtn()
        {
            BtnRefresh.onClick.RemoveAllListeners();
            BtnRemoveHide.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnHalfBottle.onClick.RemoveAllListeners();
            BtnRemoveAll.onClick.RemoveAllListeners();

			BtnRefresh.onClick.AddListener(() =>
			{
                if (!LevelManager.Instance.isPlayFxAnim)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(1);
                    if(num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 1 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    if(LevelManager.Instance.ReturnLast())
                    {
                        this.GetUtility<SaveDataUtility>().ReduceItemNum(1);
                    }
                    SetItem();
                }
            });

            BtnRemoveHide.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(2);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 2 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.RemoveHide();
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(2);
                    SetItem();
                }
            });

            BtnAddBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(3);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 3 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(false);
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(3);
                    SetItem();
                }
            });

			BtnHalfBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(4);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 4 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(true);
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(4);
                    SetItem();
                }
            });

            BtnRemoveAll.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(5);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 5 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.RemoveAll();
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(5);
                    SetItem();
                }
            });

            BtnStart.onClick.RemoveAllListeners();
            BtnStart.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIBeginSelect>();
            });

            BeginMenuButton1.onClick.RemoveAllListeners();
			BeginMenuButton1.onClick.AddListener(() =>
            {
				nowButton = 1;
                CheckBeginMenuButton();
            });

            BeginMenuButton2.onClick.RemoveAllListeners();
            BeginMenuButton2.onClick.AddListener(() =>
            {
				nowButton = 2;
                CheckBeginMenuButton();
            });

            BeginMenuButton3.onClick.RemoveAllListeners();
            BeginMenuButton3.onClick.AddListener(() =>
            {
				nowButton = 3;
                CheckBeginMenuButton();
            });

            BtnReturn.onClick.RemoveAllListeners();
            BtnReturn.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIRetry>();
            });

            BtnHeart.onClick.RemoveAllListeners();
            BtnHeart.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIMoreLife>();
            });

            BtnArea.onClick.RemoveAllListeners();
            BtnArea.onClick.AddListener(() =>
            {
                //�ѻ��������
                var nowStar = this.GetUtility<SaveDataUtility>().GetLevelClear() - 1;// -1
                //��ǰ�������
                var scene = this.GetUtility<SaveDataUtility>().GetSceneRecord();
                //��ǰ�����������
                var num = this.GetUtility<SaveDataUtility>().GetScenePartRecord();
                //ʣ���������
                var offset = nowStar - LevelManager.Instance.GetUnlockNeedStar(scene, num);
                //Debug.Log("�������� nowStar:" + nowStar);
                //Debug.Log("������� sceneNow :" + scene);
                //Debug.Log("����������� partNow :" + num);
                //Debug.Log("ʹ������" + LevelManager.Instance.GetUnlockNeedStar(scene, num));
                //Debug.Log("ʣ������ offset" + offset);
                //ʣ�����Ǵ��ڵ�����һ������������ �� ��������δ��
                //Debug.Log("��Ҫ���� ��" + LevelManager.Instance.GetPartNeedStar(scene, num));
                //Debug.Log(this.GetUtility<SaveDataUtility>().GetSceneBox());
                if (offset >= LevelManager.Instance.GetPartNeedStar(scene, num) ||
                    (this.GetUtility<SaveDataUtility>().GetSceneBox() != scene) && scene != 1)
                {
                    UIKit.OpenPanel<UIUnlockScene>();
                }
                else
                {
                    UIKit.OpenPanel<UILessStar>();
                }
            });

            BtnGetReward.onClick.RemoveAllListeners();
            //��ȡ�꽱���ص� 
            BtnGetReward.onClick.AddListener(() =>
            {
                //���³�����
                SetScene();
                StartCoroutine(FlyReward());
            });
        }

        //�¼�ע��
        void RegisterEvent()
        {
            this.RegisterEvent<LevelStartEvent>(e =>
            {
                TxtLevel.text = "Level " + LevelManager.Instance.levelId;
                SetTakeItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LevelClearEvent>(e =>
            {
                //SetScene();
                ReturnBegin(e.coin);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<CoinChangeEvent>(e =>
            {
                SetCoin();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<VitalityChangeEvent>(e =>
            {
                SetVitality();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<UnlockSceneEvent>(e =>
            {
                SetScene();
                ShowFx(e.scene, e.part);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<RewardSceneEvent>(e =>
            {
                ShowReward();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ReturnMainEvent>(e =>
            {
                LevelNode.SetActive(false);
                BeginNode.SetActive(true);
                SetScene();
                CheckBeginMenuButton();
                this.GetUtility<SaveDataUtility>().CostVitality();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                SetTakeItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameStartEvent>(e =>
            {
                LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetLevelClear());
                LevelNode.SetActive(true);
                BeginNode.SetActive(false);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            //������ť��ʼѡ��
            StringEventSystem.Global.Register("InitBeginMenuButton", () =>
            {
                InitBeginMenuButton();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        /// �����ҳ�˵���ǰ״̬
        /// </summary>
		void CheckBeginMenuButton()
		{
            switch (nowButton)
            {
                case 1:
                    ButtonAnim[0].Play("BeginMenuButtonSelect");
                    ButtonAnim[1].Play("BeginMenuButtonUnSelect");
                    ButtonAnim[2].Play("BeginMenuButtonUnSelect");
                    break;
                case 2:
                    ButtonAnim[0].Play("BeginMenuButtonUnSelect");
                    ButtonAnim[1].Play("BeginMenuButtonSelect");
                    ButtonAnim[2].Play("BeginMenuButtonUnSelect");
                    break;
                case 3:
                    ButtonAnim[0].Play("BeginMenuButtonUnSelect");
                    ButtonAnim[1].Play("BeginMenuButtonUnSelect");
                    ButtonAnim[2].Play("BeginMenuButtonSelect");
                    break;
            }
        }

        /// <summary>
        /// ��ʼ����ҳ�˵���ǰ״̬
        /// </summary>
        void InitBeginMenuButton()
        {
            ButtonAnim[0].Play("BeginMenuButtonUnSelect");
            ButtonAnim[1].Play("BeginMenuButtonSelect");
            ButtonAnim[2].Play("BeginMenuButtonUnSelect");
        }

        /// <summary>
        /// ʹ��Я�����߰�ť�¼�
        /// </summary>
        void  SetTakeItem()
        {
            BtnItem1.onClick.RemoveAllListeners();
            BtnItem2.onClick.RemoveAllListeners();
            BtnItem3.onClick.RemoveAllListeners();

            //�ж��Ƿ��е���
            BtnItem1.onClick.AddListener(()=>
            {
                if (CheckHaveItem(6))
                    UseItem(6,BtnItem1);
            });
            BtnItem2.onClick.AddListener(() =>
            {
                if (CheckHaveItem(7))
                    UseItem(7, BtnItem2);
            });
            BtnItem3.onClick.AddListener(() =>
            {
                if (CheckHaveItem(8))
                    UseItem(8, BtnItem3);
            });

            BtnItem1.gameObject.SetActive(LevelManager.Instance.takeItem.Contains(6));
            BtnItem2.gameObject.SetActive(LevelManager.Instance.takeItem.Contains(7));
            BtnItem3.gameObject.SetActive(LevelManager.Instance.takeItem.Contains(8));

            TxtItem1.text = this.GetUtility<SaveDataUtility>().GetItemNum(6).ToString();
            TxtItem2.text = this.GetUtility<SaveDataUtility>().GetItemNum(7).ToString();
            TxtItem3.text = this.GetUtility<SaveDataUtility>().GetItemNum(8).ToString();

            SetItem();
        }

        /// <summary>
        /// ����Ƿ�ӵ�е���
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        bool CheckHaveItem(int itemID)
        {
            if (this.GetUtility<SaveDataUtility>().GetItemNum(itemID) > 0)
                return true;
            else return false;
        }

        /// <summary>
        /// ʹ��Я������ --����Ч����δʵ��
        /// </summary>
        /// <param name="itemID"></param>
        void UseItem(int itemID, Button itemObj)
        {
            switch (itemID)
            {
                case 6:
                    LevelManager.Instance.AddBottle(true);
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(6);
                    TxtItem1.text = this.GetUtility<SaveDataUtility>().GetItemNum(6).ToString();

                    break;
                case 7:
                    LevelManager.Instance.AddBottle(true);
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(7);
                    TxtItem2.text = this.GetUtility<SaveDataUtility>().GetItemNum(7).ToString();

                    break;
                case 8:
                    LevelManager.Instance.AddBottle(true);
                    this.GetUtility<SaveDataUtility>().ReduceItemNum(8);
                    TxtItem3.text = this.GetUtility<SaveDataUtility>().GetItemNum(8).ToString();

                    break;
            }

            if (!CheckHaveItem(itemID))
                itemObj.Hide();
        }

        /// <summary>
        /// �����������佱��
        /// </summary>
        void ShowReward()
        {
            RewardNode.gameObject.SetActive(true);
            ImgItem1.gameObject.SetActive(true);
            ImgItem2.gameObject.SetActive(true);
            ImgItem3.gameObject.SetActive(true);
            ImgItem4.gameObject.SetActive(true);
            ImgItem5.gameObject.SetActive(true);
            ImgItem6.gameObject.SetActive(true);
            ImgItem7.gameObject.SetActive(true);
            ImgItem8.gameObject.SetActive(true);
        }

        /// <summary>
        /// ���½������
        /// </summary>
        /// <param name="num"></param>
        void SetCoin(int num = 0)
        {
            if (num == 0)
            {
                num = this.GetUtility<SaveDataUtility>().GetCoinNum();
            }

            TxtCoin.text = num.ToString();
        }

        /// <summary>
        /// ������������
        /// </summary>
        void SetStar()
        {
            var nowStar = this.GetUtility<SaveDataUtility>().GetLevelClear() - 1;
            var sceneNow = this.GetUtility<SaveDataUtility>().GetSceneRecord();
            var partNow = this.GetUtility<SaveDataUtility>().GetScenePartRecord();
            var useStar = LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow);
            TxtStar.text = (nowStar - useStar).ToString();

            //Debug.Log("������������");
        }

        /// <summary>
        /// ��������
        /// </summary>
        void SetVitality()
        {
            TxtHeart.text = this.GetUtility<SaveDataUtility>().GetVitalityNum().ToString();
        }

        /// <summary>
        /// ���¹ؿ��ı�
        /// </summary>
        void SetText()
		{
            TxtLevel.text = "Level " + LevelManager.Instance.levelId;
        }

        /// <summary>
        /// ������ҳ
        /// </summary>
        /// <param name="coin"></param>
        void ReturnBegin(int coin = 0)
        {
            SetCoin(this.GetUtility<SaveDataUtility>().GetCoinNum() - coin);
            BeginNode.SetActive(true);
            LevelNode.SetActive(false);
            StartCoroutine(ShowFx());
        }

        void SetItem()
        {
            var saveU = this.GetUtility<SaveDataUtility>();

            BtnAddRefresh.gameObject.SetActive(saveU.GetItemNum(1) <= 0);
            BtnAddRemove.gameObject.SetActive(saveU.GetItemNum(2) <= 0);
            BtnAddAddBottle.gameObject.SetActive(saveU.GetItemNum(3) <= 0);
            BtnAddHalfBottle.gameObject.SetActive(saveU.GetItemNum(4) <= 0);
            BtnAddRemoveBottle.gameObject.SetActive(saveU.GetItemNum(5) <= 0);
            TxtRefreshNum.text = saveU.GetItemNum(1).ToString();
            TxtRemoveHideNum.text = saveU.GetItemNum(2).ToString();
            TxtAddBottleNum.text = saveU.GetItemNum(3).ToString();
            TxtAddHalfBottleNum.text = saveU.GetItemNum(4).ToString();
            TxtRemoveAllNum.text = saveU.GetItemNum(5).ToString();
        }

        /// <summary>
        /// ��Һ����ǵķ�������Ч��
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowFx()
        {
            starFx.Play(10);
            
            yield return new WaitForSeconds(1.5f);
            SetStar();
            TxtCoinAdd.Play("TxtUp");
            coinFx.Play(10);

            yield return new WaitForSeconds(1.5f);
            SetCoin();
        }

        /// <summary>
        /// ������ҳ���������Ͳ���UI
        /// </summary>
        void SetScene()
		{
            //Debug.Log("���³���");
            SetStar();
            var levelNow = this.GetUtility<SaveDataUtility>().GetLevelClear();
            var sceneNow = this.GetUtility<SaveDataUtility>().GetSceneRecord();
            var partNow = this.GetUtility<SaveDataUtility>().GetScenePartRecord();
            TxtArea.text = "Area " + sceneNow;

            //���ó���
            switch (sceneNow)
            {
                case 1:
                    SceneNode1.SetActive(true);
                    SceneNode2.SetActive(false);
                    SceneNode3.SetActive(false);
                    SceneNode4.SetActive(false);
                    break;
                case 2:
                    SceneNode1.SetActive(false);
                    SceneNode2.SetActive(true);
                    SceneNode3.SetActive(false);
                    SceneNode4.SetActive(false);
                    break;
                case 3:
                    SceneNode1.SetActive(false);
                    SceneNode2.SetActive(false);
                    SceneNode3.SetActive(true);
                    SceneNode4.SetActive(false);
                    break;
                case 4:
                    SceneNode1.SetActive(false);
                    SceneNode2.SetActive(false);
                    SceneNode3.SetActive(false);
                    SceneNode4.SetActive(true);
                    break;
            }
            ImgProgress.fillAmount = partNow / 5f;
            TxtImgprogress.text = partNow + " / 5";
            //������Ҫ�жϱ����ŵȣ�ֻ��Ҫ�������UI����
            //if (this.GetUtility<SaveDataUtility>().GetSceneBox() == sceneNow)
            //{
            //    TxtArea.text = "Area " + (sceneNow);// + 1
            //    ImgProgress.fillAmount = 0;
            //    TxtImgprogress.text = 0 + " / 5";
            //}
            SetScenePart(sceneNow, partNow);

        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="partNow"></param>
        void SetScenePart(int scene, int partNow)
        {
            switch (scene)
            {
                case 1:
                    ScenePart1.ScenePart1.SetActive(partNow >= 1);
                    ScenePart1.ScenePart2.SetActive(partNow >= 2);
                    ScenePart1.ScenePart3.SetActive(partNow >= 3);
                    ScenePart1.ScenePart4.SetActive(partNow >= 4);
                    ScenePart1.ScenePart5.SetActive(partNow >= 5);
                    break;
                case 2:
                    ScenePart2.ScenePart1.SetActive(partNow >= 1);
                    ScenePart2.ScenePart2.SetActive(partNow >= 2);
                    ScenePart2.ScenePart3.SetActive(partNow >= 3);
                    ScenePart2.ScenePart4.SetActive(partNow >= 4);
                    ScenePart2.ScenePart5.SetActive(partNow >= 5);
                    break;
                case 3:
                    ScenePart3.ScenePart1.SetActive(partNow >= 1);
                    ScenePart3.ScenePart2.SetActive(partNow >= 2);
                    ScenePart3.ScenePart3.SetActive(partNow >= 3);
                    ScenePart3.ScenePart4.SetActive(partNow >= 4);
                    ScenePart3.ScenePart5.SetActive(partNow >= 5);
                    break;
                case 4:
                    ScenePart4.ScenePart1.SetActive(partNow >= 1);
                    ScenePart4.ScenePart2.SetActive(partNow >= 2);
                    ScenePart4.ScenePart3.SetActive(partNow >= 3);
                    ScenePart4.ScenePart4.SetActive(partNow >= 4);
                    ScenePart4.ScenePart5.SetActive(partNow >= 5);
                    break;

            }
        }

        /// <summary>
        /// ������������������Ч
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="num"></param>
        void ShowFx(int scene, int num)
        {
            switch (scene)
            {
                case 1:
                    StartCoroutine(ScenePart1.ShowUnlock(num));
                    break;
                case 2:
                    StartCoroutine(ScenePart2.ShowUnlock(num));
                    break;
                case 3:
                    StartCoroutine(ScenePart3.ShowUnlock(num));
                    break;
                case 4:
                    StartCoroutine(ScenePart4.ShowUnlock(num));
                    break;
            }
        }

        /// <summary>
        /// ���߷���Ч��(��ɻص�)
        /// </summary>
        /// <returns></returns>
        IEnumerator FlyReward()
        {
            RewardNode.gameObject.SetActive(false);

            ImgItem1.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem1.gameObject.SetActive(false);
                    ImgItem1.transform.position = Begin1.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem2.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem2.gameObject.SetActive(false);
                    ImgItem2.transform.position = Begin2.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem3.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem3.gameObject.SetActive(false);
                    ImgItem3.transform.position = Begin3.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem4.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem4.gameObject.SetActive(false);
                    ImgItem4.transform.position = Begin4.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem5.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem5.gameObject.SetActive(false);
                    ImgItem5.transform.position = Begin5.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem6.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem6.gameObject.SetActive(false);
                    ImgItem6.transform.position = Begin6.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem7.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem7.gameObject.SetActive(false);
                    ImgItem7.transform.position = Begin7.transform.position;
                });
            yield return new WaitForSeconds(0.2f);
            ImgItem8.transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem8.gameObject.SetActive(false);
                    ImgItem8.transform.position = Begin8.transform.position;
                });

            RewardCoinFx.Play(10);

        }
    }
}
