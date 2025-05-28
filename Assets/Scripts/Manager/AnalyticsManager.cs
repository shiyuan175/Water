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

            TestSendEvent();
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Unity Services ��ʼ��ʧ��: {e.Message}");
            throw;
        }

    }

    public void SendServerEvent(string eventName, Dictionary<string, object> parameters)
    {
        // �ɰ�-������
        //AnalyticsService.Instance.CustomData(eventName, parameters);

        // �°�-�����Զ����¼����� ʾ��
        var customEvent = new CustomEvent("level_complete");
        customEvent["level"] = 5;
        customEvent["time_spent"] = 120.5f;

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