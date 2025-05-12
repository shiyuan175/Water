using UnityEngine;
using UnityEngine.UI;
using QFramework;
using GameDefine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System;

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

        public GameObject LevelNode, SceneNode1, SceneNode2, SceneNode3, SceneNode4;

        public ScenePartCtrl ScenePart1, ScenePart2, ScenePart3, ScenePart4;
        public ParticleTargetMoveCtrl coinFx, starFx;

        private GameObject HomeNode => Panels[2];
        #region BottomMenuSetting
        [SerializeField] private List<Button> bottomMenuBtns;
        [SerializeField] private List<RectTransform> bottomMenuRect;
        [SerializeField] private List<GameObject> Panels;
        [SerializeField] private RectTransform selectedImg;

        private int nowButton = 2;
        private readonly Vector2 SELECTED = new Vector2(256, 200);  // ѡ�зŴ�Ĵ�С
        private readonly Vector2 NSELECTED = new Vector2(206, 200); // δѡ�еĴ�С
        private readonly float minScaleValue = 0.5f;                // ��ť����Сֵ(����С��Ŵ�)
        private readonly float maxScaleValue = 1.2f;                // ��ť�ķŴ�ֵ
        private readonly float targetPosY = 80f;                    // ��ť����̧��ĸ߶�
        private readonly float initPosY = 15f;                      // ��ť�ĳ�ʼλ��
        #endregion

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIBeginData ?? new UIBeginData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            //���ģʽ�£�AssetBundle ������Դ����Ҫ��������
            TxtLevel.font.material.shader = Shader.Find(TxtLevel.font.material.shader.name);
            //TxtImgprogress.font.material.shader = Shader.Find(TxtImgprogress.font.material.shader.name);
        }

        protected override void OnShow()
        {
            BindBtn();
            RegisterEvent();
            SetVitality();
            SetCoin();
            SetStar();
            //InitBeginMenuButton();//����Ҫ��ʼ������ʹ��

            //������Ϸʱ������ǰ��ػ�ֱ�ӿ�ʼ��Ϸ����Ҫ����UI
            var levelNow = this.GetUtility<SaveDataUtility>().GetLevelClear();
            if (levelNow <= 5)
            {
                StartOrOverChangePanel(true, false);
                TxtLevel.text = LevelManager.Instance.levelId.ToString();
                BottomMenuBtns.Hide();
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
            BtnStepBack.onClick.RemoveAllListeners();
            BtnRemoveHide.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnHalfBottle.onClick.RemoveAllListeners();
            BtnRemoveAll.onClick.RemoveAllListeners();

            BtnStepBack.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(1);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 1 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    if (LevelManager.Instance.ReturnLast())
                    {
                        UseItemUpdateNum(1);
                    }
                }
            });

            BtnRemoveHide.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(2);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 2 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    //�ж��Ƿ��к�ˮƿ
                    if (LevelManager.Instance.hideBottleList.Count != 0)
                    {
                        LevelManager.Instance.RemoveHide(() =>
                        {
                            UseItemUpdateNum(2);
                        });
                    }
                }
            });

            BtnAddBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(3);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 3 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(false, () =>
                    {
                        UseItemUpdateNum(3);
                    });
                }
            });

            BtnHalfBottle.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(4);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 4 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    LevelManager.Instance.AddBottle(true, () =>
                    {
                        UseItemUpdateNum(4);
                    });
                }
            });

            BtnRemoveAll.onClick.AddListener(() =>
            {
                if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
                {
                    var num = this.GetUtility<SaveDataUtility>().GetItemNum(5);
                    if (num <= 0)
                    {
                        UIBuyItemData data = new UIBuyItemData() { item = 5 };
                        UIKit.OpenPanel<UIBuyItem>(data);
                        return;
                    }
                    if (LevelManager.Instance.CheckAllDebuff())
                    {
                        LevelManager.Instance.RemoveAll();
                        UseItemUpdateNum(5);
                    }
                }
            });

            BtnStart.onClick.RemoveAllListeners();
            BtnStart.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIBeginSelect>();
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
                UIKit.OpenPanel<UIUnlockScene>();
            });

            BtnGetReward.onClick.RemoveAllListeners();
            //��ȡ�꽱���ص� 
            BtnGetReward.onClick.AddListener(() =>
            {
                //���³�����
                SetScene();
                StartCoroutine(FlyReward());
            });

            BtnItem1.onClick.RemoveAllListeners();
            BtnItem2.onClick.RemoveAllListeners();
            BtnItem3.onClick.RemoveAllListeners();

            BtnItem1.onClick.AddListener(() =>
            {
                if (CheckHaveItem(6))
                    UseItem(6, BtnItem1);
            });
            BtnItem2.onClick.AddListener(() =>
            {
                if (CheckHaveItem(7))
                    UseItem(7, BtnItem2);
            });
            BtnItem3.onClick.AddListener(() =>
            {
                if (CheckHaveItem(8))
                {
                    LevelManager.Instance.ShowItemSelect();
                    GameCtrl.Instance.SeletedItem(bottele => { UseItem(8, BtnItem3, bottele); });
                }
            });

            BtnHead.onClick.RemoveAllListeners();
            BtnHead.onClick.AddListener(() =>
            {
                UIKit.OpenPanel("UIPersonal");
            });

            BtnCoin.onClick.AddListener(() =>
            {
                InitBeginMenuButton(0);
            });

            //�ײ�����ť����
            foreach (var btn in bottomMenuBtns)
            {
                btn.onClick.AddListener(() =>
                {
                    int index = bottomMenuBtns.IndexOf(btn);
                    //�л�����
                    ChangePanel(index);
                    if (nowButton != index)
                    {
                        for (int i = 0; i < bottomMenuRect.Count; i++)
                        {
                            var rt = bottomMenuBtns[i].GetComponent<RectTransform>();
                            if (i == index)
                            {
                                //����ѡ��Ч��
                                rt.localScale = new Vector3(minScaleValue, minScaleValue, minScaleValue);
                                rt.DOScale(new Vector3(maxScaleValue, maxScaleValue, 1), 0.1f);
                                rt.DOLocalMoveY(targetPosY, 0.1f);
                                bottomMenuRect[index].sizeDelta = SELECTED;
                            }
                            else
                            {
                                //����δѡ��Ч��
                                rt.DOScale(Vector3.one, 0.2f);
                                rt.DOLocalMoveY(initPosY, 0.2f);
                                bottomMenuRect[i].sizeDelta = NSELECTED;
                            }

                        }
                        //�ȴ�һ֡
                        ActionKit.DelayFrame(1, () =>
                        {
                            //ͬ����ť����λ��(�������ð�ť�µ�������ʾ)
                            for (int i = 0; i < bottomMenuBtns.Count; i++)
                            {
                                var rt = bottomMenuBtns[i].GetComponent<RectTransform>();
                                rt.DOLocalMoveX(bottomMenuRect[i].localPosition.x, 0.2f);
                            }
                            //���»�����
                            selectedImg.DOMove(bottomMenuRect[index].position, 0.1f);
                            nowButton = index;
                        }).Start(this);
                    }
                });
            }
        }

        //�¼�ע��
        void RegisterEvent()
        {
            //��ʼ��Ϸ�¼�
            this.RegisterEvent<LevelStartEvent>(e =>
            {
                BottomMenuBtns.Hide();
                TxtLevel.text = LevelManager.Instance.levelId.ToString();
                SetTakeItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            //ʤ������=��������ҳ�¼�
            this.RegisterEvent<LevelClearEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                InitBeginMenuButton();
                StartOrOverChangePanel(false, true);
                StartCoroutine(ShowFx());

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<CoinChangeEvent>(e =>
            {
                SetCoin(e.coin);
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
                StartOrOverChangePanel(false, true);
                SetScene();
                InitBeginMenuButton();
                HealthManager.Instance.UseHp();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                SetTakeItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameStartEvent>(e =>
            {
                LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetLevelClear());
                StartOrOverChangePanel(true, false);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register("StreakWinItem", (int count) =>
            {
                ClearBottleBlackWater(count);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        #region �ײ��˵�����ť�л�

        /// <summary>
        /// �˵���ť����л�����
        /// </summary>
        /// <param name="index"></param>
        void ChangePanel(int index)
        {
            for (int i = 0; i < Panels.Count; i++)
            {
                if (i == index)
                    Panels[i].Show();
                else
                    Panels[i].Hide();
            }
        }

        /// <summary>
        /// �л���ʼ��Ϸ�ͽ�����Ϸ���
        /// </summary>
        /// <param name="levelNode">��Ϸ����״̬</param>
        /// <param name="homeNode">��ҳ��ʼ����״̬</param>
        void StartOrOverChangePanel(bool levelNode, bool homeNode)
        {
            LevelNode.gameObject.SetActive(levelNode);
            HomeNode.gameObject.SetActive(homeNode);
        }

        /// <summary>
        /// ��ʾ����ʼ���ײ��˵���ť
        /// </summary>
        void InitBeginMenuButton(int index = -1)
        {
            BottomMenuBtns.Show();
            //�вδ��룬��ʼ��ť���(�л���Ӧ����)
            if (index > -1)
                bottomMenuBtns[index].onClick.Invoke();
        }

        #endregion

        #region �������

        /// <summary>
        /// ʹ��Я�����߰�ť�¼�
        /// </summary>
        /// ������Ϸ/���ùؿ�����
        void SetTakeItem()
        {
            if (LevelManager.Instance.takeItem.Count > 0)
            {
                //��ʾ��������
                //TxtItem1.text = this.GetUtility<SaveDataUtility>().GetItemNum(6).ToString();
                //TxtItem2.text = this.GetUtility<SaveDataUtility>().GetItemNum(7).ToString();
                //TxtItem3.text = this.GetUtility<SaveDataUtility>().GetItemNum(8).ToString();
                BtnItem1.interactable = LevelManager.Instance.takeItem.Contains(6) && CheckHaveItem(6);
                BtnItem2.interactable = LevelManager.Instance.takeItem.Contains(7) && CheckHaveItem(7);
                BtnItem3.interactable = LevelManager.Instance.takeItem.Contains(8) && CheckHaveItem(8);

                //��ʾֻ��ֻ��һ��
                if (BtnItem1.interactable)
                    TxtItem1.text = "1";
                if (BtnItem2.interactable)
                    TxtItem2.text = "1"; 
                if (BtnItem3.interactable)
                    TxtItem3.text = "1";
            }
            else
            {
                BtnItem1.interactable = false;
                BtnItem2.interactable = false;
                BtnItem3.interactable = false;
                TxtItem1.text = "0";
                TxtItem2.text = "0";
                TxtItem3.text = "0";
            }

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
        /// ʹ��Я������
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemObj"></param>
        /// <param name="botter">�������ĸ�ƿ��(����ˮ����ߴ���)</param>
        void UseItem(int itemID, Button itemObj, BottleCtrl botter = null)
        {
            switch (itemID)
            {
                case 6:
                    LevelManager.Instance.AddBottle(true, () =>
                    {
                        this.GetUtility<SaveDataUtility>().ReduceItemNum(6);
                        TxtItem1.text = "0";
                        //TxtItem1.text = this.GetUtility<SaveDataUtility>().GetItemNum(6).ToString();//��ʾԭ����
                    });
                    break;

                case 7:
                    if (!(LevelManager.Instance.hideBottleList.Count > 0))
                        return;
                    ClearBottleBlackWater(2, true, () =>
                    {
                        this.GetUtility<SaveDataUtility>().ReduceItemNum(7);
                        TxtItem2.text = "0";
                        //TxtItem2.text = this.GetUtility<SaveDataUtility>().GetItemNum(7).ToString();
                    });
                    break;

                case 8:
                    //Debug.Log("����ʹ�õ���");
                    // �����б��������ϴ��
                    List<int> indices = Enumerable.Range(0, botter.waters.Count).ToList();
                    do
                    {
                        for (int i = 0; i < indices.Count; i++)
                        {
                            int randIndex = UnityEngine.Random.Range(i, indices.Count);
                            (indices[i], indices[randIndex]) = (indices[randIndex], indices[i]);
                        }
                    }
                    while (Enumerable.SequenceEqual(indices, Enumerable.Range(0, botter.waters.Count)));

                    List<int> newWaters = new List<int>();
                    List<bool> newHideWater = new List<bool>();
                    foreach (int idx in indices)
                    {
                        newWaters.Add(botter.waters[idx]);
                        newHideWater.Add(botter.hideWaters[idx]);
                    }
                    // �滻ԭ�б�
                    botter.waters = newWaters;
                    botter.hideWaters = newHideWater;

                    //�޸�ˮ����ɫ���л�����λ��
                    for (int i = 0; i < botter.waters.Count; i++)
                    {
                        var useColor = botter.waters[i] - 1;
                        if (useColor < 1000)
                        {
                            botter.waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor]);
                        }
                        else
                        {
                            // ���ݵ����������ö�Ӧ����ʾ�Ͷ���
                            botter.waterImg[i].SetColorState((ItemType)botter.waters[i], LevelManager.Instance.ItemColor);
                        }
                    }
                    //�޸�ˮ��λ��(�е��ߵ����ˮ��λ�ò�һ��)���޸�ˮ����ɫ������ˮ�涯���������Ʊ�����λ��
                    botter.SetNowSpinePos(botter.waters.Count);
                    botter.PlaySpineWaitAnim();
                    botter.CheckWaterItem();

                    //����С���⣬�ͺ�ˮ�齻��λ��ʱ(���ǽ�������һ��)��Ҳ�ᴥ������
                    botter.SetHideShow(true);
                    LevelManager.Instance.HideItemSelect();

                    this.GetUtility<SaveDataUtility>().ReduceItemNum(8);
                    //TxtItem3.text = this.GetUtility<SaveDataUtility>().GetItemNum(8).ToString();
                    TxtItem3.text = "0";
                    //Debug.Log("����˳��ɹ�");
                    break;
            }

            //if (!CheckHaveItem(itemID))//��ʹ��һ��
            itemObj.interactable = false;
        }

        /// <summary>
        /// ���ƿ�����к�ˮ
        /// </summary>
        /// <param name="count">�����ƿ������</param>
        /// <param name="effctNow">�Ƿ�������Ч</param>
        /// <param name="action">�ص�(����ʹ��ʱ����)</param>
        private void ClearBottleBlackWater(int count, bool effctNow = false, Action action = null)
        {
            if (LevelManager.Instance.hideBottleList.Count > 0)
            {
                var tempList = new List<BottleCtrl>(LevelManager.Instance.hideBottleList);

                while (tempList.Count > count)
                {
                    int randIndex = UnityEngine.Random.Range(0, tempList.Count);
                    tempList.RemoveAt(randIndex);
                }
                foreach (var item in tempList)
                {
                    for (int i = 0; i < item.hideWaters.Count; i++)
                    {
                        item.hideWaters[i] = false;
                    }
                    item.SetHideShow(effctNow);
                    LevelManager.Instance.hideBottleList.Remove(item);
                }

                action?.Invoke();
            }
        }

        /// <summary>
        /// �·����������߸���
        /// </summary>
        private void SetItem()
        {
            var saveU = this.GetUtility<SaveDataUtility>();
            BtnAddStepBack.gameObject.SetActive(saveU.GetItemNum(1) <= 0);
            TxtRefreshNum.text = saveU.GetItemNum(1).ToString();

            BtnAddRemove.gameObject.SetActive(saveU.GetItemNum(2) <= 0);
            TxtRemoveHideNum.text = saveU.GetItemNum(2).ToString();

            BtnAddAddBottle.gameObject.SetActive(saveU.GetItemNum(3) <= 0);
            TxtAddBottleNum.text = saveU.GetItemNum(3).ToString();

            BtnAddHalfBottle.gameObject.SetActive(saveU.GetItemNum(4) <= 0);
            TxtAddHalfBottleNum.text = saveU.GetItemNum(4).ToString();

            BtnAddRemoveBottle.gameObject.SetActive(saveU.GetItemNum(5) <= 0);
            TxtRemoveAllNum.text = saveU.GetItemNum(5).ToString();
        }

        /// <summary>
        /// ʹ�õ��ߺ�Ļص������¶�Ӧ����ʣ������
        /// </summary>
        void UseItemUpdateNum(int itemId)
        {
            var saveU = this.GetUtility<SaveDataUtility>();
            switch (itemId)
            {
                case 1:
                    saveU.ReduceItemNum(1);
                    BtnAddStepBack.gameObject.SetActive(saveU.GetItemNum(1) <= 0);
                    TxtRefreshNum.text = saveU.GetItemNum(1).ToString();
                    break;
                case 2:
                    saveU.ReduceItemNum(2);
                    BtnAddRemove.gameObject.SetActive(saveU.GetItemNum(2) <= 0);
                    TxtRemoveHideNum.text = saveU.GetItemNum(2).ToString();
                    break;
                case 3:
                    saveU.ReduceItemNum(3);
                    BtnAddAddBottle.gameObject.SetActive(saveU.GetItemNum(3) <= 0);
                    TxtAddBottleNum.text = saveU.GetItemNum(3).ToString();
                    break;
                case 4:
                    saveU.ReduceItemNum(4);
                    BtnAddHalfBottle.gameObject.SetActive(saveU.GetItemNum(4) <= 0);
                    TxtAddHalfBottleNum.text = saveU.GetItemNum(4).ToString();
                    break;
                case 5:
                    saveU.ReduceItemNum(5);
                    BtnAddRemoveBottle.gameObject.SetActive(saveU.GetItemNum(5) <= 0);
                    TxtRemoveAllNum.text = saveU.GetItemNum(5).ToString();
                    break;
            }
        }

        #endregion

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
                num = CoinManager.Instance.Coin;

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
            //���Լ�¼һ����ʹ����������
            var useStar = LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow);
            TxtStar.text = (nowStar - useStar).ToString();

            //Debug.Log("������������");
        }

        /// <summary>
        /// ��������
        /// </summary>
        void SetVitality()
        {
            TxtHeart.text = HealthManager.Instance.NowHp.ToString();
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
            CoinManager.Instance.AddCoin(20);
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

            //�������(����Ч��)
            if (bool.Parse(this.GetUtility<SaveDataUtility>().GetOverUnLock()))
            {
                TxtArea.text = "Area " + 5;
                ImgProgress.fillAmount = 0;
                TxtImgprogress.text = 0 + " / 5";
                return;
            }

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
            yield return new WaitForSeconds(1.5f);
            CoinManager.Instance.AddCoin(200);
        }
    }
}
