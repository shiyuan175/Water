using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

[MonoSingletonPath("[Analytics]/AnalyticsManager")]
public class AnalyticsManager : MonoSingleton<AnalyticsManager>, ICanGetUtility, ICanSendEvent
{
    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            //Debug.Log("Unity Services ��ʼ���ɹ�");

            AnalyticsService.Instance.StartDataCollection();
            //Debug.Log("Analytics �����ռ�������");

            //TestSendEvent();
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Unity Services ��ʼ��ʧ��: {e.Message}");
            throw;
        }

    }

    public void SendLevelEvent(string del)
    {
        Dictionary<string, object> _levelEvent = new Dictionary<string, object>
        {
            { GameDefine.GameConst.LEVEL, this.GetUtility<SaveDataUtility>().GetLevelClear()},
            { GameDefine.GameConst.DETAILS, del}
        };
        SendServerEvent(GameDefine.GameConst.ANALYTICS_EVENT_LEVEL_COMPLETE, _levelEvent);
    }

    private void SendServerEvent(string eventName, Dictionary<string, object> parameters)
    {
        // �ɰ�-������
        //AnalyticsService.Instance.CustomData(eventName, parameters);

        // �°�-�����Զ����¼�����
        // ���ݵļ���Ҫ��Unity Analytics��Ԥ�ȶ���
        var customEvent = new CustomEvent(eventName);
        foreach (var pair in parameters)
        {
            customEvent[pair.Key] = pair.Value;
        }

        // �����¼�
        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void TestSendEvent()
    {
        //���ݵļ���Ҫ��Unity Analytics��Ԥ�ȶ���
        var customEvent = new CustomEvent("completeLevel");
        customEvent["level"] = 5;
        customEvent["details"] = "�Զ�������¼�";

        // �����¼�
        AnalyticsService.Instance.RecordEvent(customEvent);
        //Debug.Log("�����Զ������ݷ����¼�");
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}