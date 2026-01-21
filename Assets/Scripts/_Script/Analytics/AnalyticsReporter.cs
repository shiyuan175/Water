using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Analytics;
using System;
using QFramework;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Spine;

public class AnalyticsReporter : MonoBehaviour ,ICanGetModel
{
    private const int MAX_RETRY = 3;
    private const string SECRET = "xexdf9%8DSld#wp";
    private const string NONCECHARSET = "abcdefghijklmnopqrstuvwxyz0123456789";
    private readonly string PublicIPTraceUrl = "https://1.1.1.1/cdn-cgi/trace";
    private readonly string ReportApiUrl = "https://43.133.30.51/api/1.0.0/report";

    private bool mIsReporting;
    private GameGlobalModel mGameGlobalModel;
    private IdentityData mIdentityData;
    private EventCache mFailReporterCache;
    private Queue<string> mReporterQueue;

    private void Awake()
    {
        mReporterQueue = new Queue<string>();
        mFailReporterCache = new EventCache();
        mIdentityData = AnalyticsData.ReadFromJson<IdentityData>(AnalyticsData.IdentityPath);
        mGameGlobalModel = this.GetModel<GameGlobalModel>();
        StartCoroutine(Init());
    }

    private void Start()
    {
        TypeEventSystem.Global.Register<ReportLevelEvent>(levelInfo =>
        {
            LevelEvent levelEvent = new()
            {
                uid = mIdentityData.uid,
                deviceid = mIdentityData.deviceId,
                stageid = levelInfo.level,
                type = levelInfo.type,
                createtime = AnalyticsData.NowSeconds(),
                coins = CoinManager.Instance.Coin
            };

            if (levelInfo.iswin.HasValue)
                levelEvent.iswin = levelInfo.iswin.Value;

            List<Items> items = new();
            foreach (var item in mGameGlobalModel.ItemDic)
            {
                Items itemInfo = new()
                {
                    itemId = item.Key,
                    count = item.Value,
                };
                items.Add(itemInfo);
            }

            var json = JsonConvert.SerializeObject(levelEvent);
            mReporterQueue.Enqueue(json);
            StartReporter();
        }).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void StartReporter()
    {
        if (mIsReporting) return;

#if !UNITY_EDITOR
        StartCoroutine(ReporterWorker());
#endif
    }

    private IEnumerator Init()
    {
        var eventCache = AnalyticsData.ReadFromJson<EventCache>(AnalyticsData.EventCachePath);
        eventCache?.Events.ForEach(mReporterQueue.Enqueue);

        PublicIPApiResponse ipInfo = null;
        yield return StartCoroutine(GetPublicIp(mes =>
        {
            ipInfo = AnalyticsData.ParseIp(mes);
        }));

        LaunchEvent launchEvent = new();
        if (ipInfo != null)
        {
            launchEvent.ip = ipInfo.Ip;
            launchEvent.country = ipInfo.CountryCode;
            launchEvent.city = ipInfo.CityCode;
        }
        launchEvent.uid = mIdentityData.uid;
        launchEvent.deviceid = mIdentityData.deviceId;
        launchEvent.platform = mIdentityData.platform;
        launchEvent.createtime = AnalyticsData.NowSeconds();

        string launchEventJson = JsonConvert.SerializeObject(launchEvent);
        mReporterQueue.Enqueue(launchEventJson);

        StartReporter();
    }

    private IEnumerator GetPublicIp(Action<string> action)
    {
        using UnityWebRequest req = UnityWebRequest.Get(PublicIPTraceUrl);
        req.timeout = 2;

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            action?.Invoke(null);
        }
        else
        {
            var mes = req.downloadHandler.text;
            action?.Invoke(mes);
        }
    }

    private IEnumerator PostRequest(string json, string ts, string nonce, string sign , Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest(ReportApiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-ts", ts);
        request.SetRequestHeader("x-nonce", nonce);
        request.SetRequestHeader("x-sign", sign);

        //StringBuilder headerLog = new StringBuilder();
        //headerLog.AppendLine($"x-ts : {ts}");
        //headerLog.AppendLine($"x-nonce: {nonce}");
        //headerLog.AppendLine($"x-sign: {sign}");
        //Debug.Log(headerLog);
        Debug.Log($"JSON Body: {json}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ReportApiResponse response = JsonUtility.FromJson<ReportApiResponse>(request.downloadHandler.text);
            Debug.Log($"Success: {response.code}, {response.msg}, {response.time}");
            if (response.code == "0000") action?.Invoke(true);
            else action?.Invoke(false);
        }
        else
        {
            //Debug.LogError($"Error: {request.responseCode}, {request.error}, {request.downloadHandler.text}");
            action?.Invoke(false);
        }
    }

    private IEnumerator ReporterWorker()
    {
        mIsReporting = true;

        while (mReporterQueue.Count > 0)
        {
            string json = mReporterQueue.Dequeue();
            int attempt = 0;

            while (true)
            {
                // 存在网络接口(但不代表网络能访问)
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    mFailReporterCache.Events.Add(json);
                    break;
                }

                bool done = false;
                bool success = false;

                string ts = AnalyticsData.NowSeconds().ToString();
                string nonce = GenerateNonce(32);
                string x = BuildSignStringFromJson(json);
                string y = $"{x}|{ts}|{nonce}|{SECRET}";
                string sign = ComputeSHA256(y);

                yield return StartCoroutine(PostRequest(json, ts, nonce, sign, isSuccess =>
                {
                    success = isSuccess;
                    done = true;
                }));

                while (!done) yield return null;

                if (success) break;
                attempt++;

                if (attempt >= MAX_RETRY)
                {
                    mFailReporterCache.Events.Add(json);
                    break;
                }

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }

        PersistFailQueue();
        mIsReporting = false;
    }

    //SHA256签名
    private string ComputeSHA256(string message)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    //生成随机nonce
    private string GenerateNonce(int length)
    {
        char[] buffer = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] data = new byte[length];
            rng.GetBytes(data);
            for (int i = 0; i < length; i++)
            {
                buffer[i] = NONCECHARSET[data[i] % NONCECHARSET.Length];
            }
        }
        return new string(buffer);
    }

    //生成签名字符串
    private string BuildSignStringFromJson(string json)
    {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        var sortedItems = dict.OrderBy(kv => kv.Key);
        StringBuilder xBuilder = new();
        foreach (var kv in sortedItems)
        {
            if (xBuilder.Length > 0) xBuilder.Append("&");
            xBuilder.Append($"{kv.Key}={(kv.Value ?? "null")}");
        }
        return xBuilder.ToString();
    }

    //入盘
    private void PersistFailQueue()
    {
        AnalyticsData.WriteToJson(AnalyticsData.EventCachePath, mFailReporterCache);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}