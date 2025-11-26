using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

[MonoSingletonPath("[Analytics]/AnalyticsManager")]
public class AnalyticsManager : MonoSingleton<AnalyticsManager>, ICanGetUtility, ICanSendEvent
{
    private const string ANALYTICS_EVENT_LEVEL_COMPLETE = "level_complete";
    private const string LEVEL = "level";
    private const string DETAILS = "details";

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            //Debug.Log("Unity Services 初始化成功");

            AnalyticsService.Instance.StartDataCollection();
            //Debug.Log("Analytics 数据收集已启动");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Unity Services 初始化失败: {e.Message}");
            throw;
        }

    }

    public void SendLevelEvent(string del)
    {
        Dictionary<string, object> _levelEvent = new Dictionary<string, object>
        {
            { LEVEL, this.GetUtility<SaveDataUtility>().GetCurrentLevel()},
            { DETAILS, del}
        };
        SendServerEvent(ANALYTICS_EVENT_LEVEL_COMPLETE, _levelEvent);
    }

    private void SendServerEvent(string eventName, Dictionary<string, object> parameters)
    {
        // 传递的键需要在Unity Analytics中预先定义
        var customEvent = new CustomEvent(eventName);
        foreach (var pair in parameters)
        {
            customEvent[pair.Key] = pair.Value;
        }

        // 发送事件
        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}