using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Button rebtn;
    public Button addbtn;

    private void Awake()
    {
        HealthManager healthManager = HealthManager.Instance;
    }

    void Start()
    {
        rebtn.onClick.AddListener(() =>
        {
            HealthManager.Instance.CancelUnLimitHp();
        });

        addbtn.onClick.AddListener(() =>
        {
            HealthManager.Instance.SetUnLimitHp(120);
        });
        //Debug.Log(IsNetworkReachability());
    }

    /// <summary>
    /// ����ɴ���
    /// </summary> 
    /// <returns></returns>
    public bool IsNetworkReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                print("��ǰʹ�õ��ǣ�WiFi������ĸ��£�");
                return true;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                print("��ǰʹ�õ����ƶ����磬�Ƿ�������£�");
                return true;
            default:
                print("��ǰû���������������������ٽ��в�����");
                return false;
        }
    }    
}
