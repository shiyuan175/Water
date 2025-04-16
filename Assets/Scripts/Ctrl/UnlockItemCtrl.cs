using QFramework;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockItemCtrl : MonoBehaviour, ICanGetUtility
{
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public TextMeshProUGUI TxtName, TxtNeed;
    public Image ImgIcon;
    public Button BtnUnlock;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="num">ȡֵ��Χ1-5</param>
    public void SetItem(int scene, int num)
    {
        List<Sprite> useScene = null;
        List<string> useStr = null;
        switch(scene)
        {
            case 1:
                useScene = LevelManager.Instance.scene1;
                useStr = LevelManager.Instance.scenePartName1;
                break;
            case 2:
                useScene = LevelManager.Instance.scene2;
                useStr = LevelManager.Instance.scenePartName2;
                break;
            case 3:
                useScene = LevelManager.Instance.scene3;
                useStr = LevelManager.Instance.scenePartName3;
                break;
            case 4:
                useScene = LevelManager.Instance.scene4;
                useStr = LevelManager.Instance.scenePartName4;
                break;
        }

        ImgIcon.sprite = useScene[num - 1];
        TxtName.text = useStr[num - 1];
        TxtNeed.text = LevelManager.Instance.GetPartNeedStar(scene, num).ToString();

        BtnUnlock.onClick.RemoveAllListeners();
        BtnUnlock.onClick.AddListener(() =>
        {

            var nowStar = this.GetUtility<SaveDataUtility>().GetLevelClear() -1;   // -1

            var sceneNow = this.GetUtility<SaveDataUtility>().GetSceneRecord();
            var partNow = this.GetUtility<SaveDataUtility>().GetScenePartRecord();

            var offset = nowStar - LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow);
            //Debug.Log("�������-�������ǣ�" + nowStar);
            //Debug.Log("�������-ʹ�����ǣ�" + LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow));
            //Debug.Log("�������-ʣ�����ǣ�" + offset);
            //Debug.Log("�������-��Ҫ���ǣ�" + LevelManager.Instance.GetPartNeedStar(scene, num));
            //ʣ�������㹻�������� ���ڵ��� 
            if (offset >= LevelManager.Instance.GetPartNeedStar(scene, num))
            {
                LevelManager.Instance.UnlockScene(scene, num);
                UIKit.ClosePanel<UIUnlockScene>();
            }
        });
    }
}
