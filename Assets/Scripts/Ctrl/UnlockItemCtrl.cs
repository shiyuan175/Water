using QFramework;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockItemCtrl : MonoBehaviour, ICanGetUtility, ICanSendEvent, ICanGetModel
{
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public TextMeshProUGUI TxtName, TxtNeed;
    public Image ImgIcon;
    public Button BtnUnlock;

    private StageModel stageModel;

    void Start()
    {
        stageModel = this.GetModel<StageModel>();
    }

    void OnDestroy()
    {
        stageModel = null;     
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="num">ȡֵ��Χ1-5</param>
    public void SetItem(int scene, int num)
    {
        IReadOnlyList<Sprite> useScene = null;
        IReadOnlyList<string> useStr = null;

        //������1��ʼ����
        int _index = scene - 1;

        if (_index >= 0 && _index < LevelManager.Instance.SceneUnLockSOs.Count)
        {
            useScene = LevelManager.Instance.SceneUnLockSOs[_index].SceneSprites;
            useStr = LevelManager.Instance.SceneUnLockSOs[_index].ScenePartName;
        }
        
        ImgIcon.sprite = useScene[num - 1];
        TxtName.text = useStr[num - 1];
        TxtNeed.text = LevelManager.Instance.GetPartNeedStar(scene, num).ToString();

        BtnUnlock.onClick.RemoveAllListeners();
        BtnUnlock.onClick.AddListener(() =>
        {
            var nowStar = this.GetUtility<SaveDataUtility>().GetLevelClear() - 1;
            var sceneNow = this.GetUtility<SaveDataUtility>().GetSceneRecord();
            var partNow = this.GetUtility<SaveDataUtility>().GetScenePartRecord();
            var offset = nowStar - LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow);
            //Debug.Log("�������ǣ�" + nowStar);
            //Debug.Log("ʹ�����ǣ�" + LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow));
            //Debug.Log("ʣ�����ǣ�" + offset);
            //Debug.Log("��Ҫ���ǣ�" + LevelManager.Instance.GetPartNeedStar(scene, num));
            //ʣ�������㹻�������� ���ڵ��� 
            if (offset >= LevelManager.Instance.GetPartNeedStar(scene, num))
            {
                this.GetUtility<SaveDataUtility>().SetScenePartRecord(num);
                if(num == GameDefine.GameConst.SCENE_PART_COUNT)
                    stageModel.SceneBoxUnlock = true;

                this.SendEvent<UnlockSceneEvent>(new UnlockSceneEvent() 
                { 
                    part = num ,
                    scene = scene 
                });
                UIKit.ClosePanel<UIUnlockScene>();
            }
            else
            {
                UIKit.ClosePanel<UIUnlockScene>();
                UIKit.OpenPanel<UILessStar>();
            }
        });
    }
}
