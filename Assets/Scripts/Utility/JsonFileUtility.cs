using System;
using System.Collections.Generic;
using System.IO;
using JsonFileData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QFramework;
using UnityEngine;

namespace JsonFileData
{
    /// <summary>
    /// 用于版本对比
    /// </summary>
    public class VersionWrapper
    {
        public int Version;
    }

    /// <summary>
    /// 用于声明文件信息和版本
    /// </summary>
    public class JsonFileInfo
    {
        public string FileName;
        public int TargetVersion;
    }

    #region 通用配置Json

    /// <summary>
    /// 每日奖励领取记录
    /// 后续可拓展每日增益效果
    /// </summary>
    public class DailyReward
    {
        public int Version;
        public long NextResetTicks;
        //第二套剧情奖励
        public bool IsClaim_UnlockScene2Reward;
        public bool IsClaim_UnlockScene4Reward;
        //特权礼包3每日奖励
        public bool DailyReward_ByGiftPack3;
    }

    public class GameGlobalData
    {
        public int Version;

        //场景解锁1、3的增益领取记录(只领一次)
        public bool IsClaim_UnlockScene1Reward;
        public bool IsClaim_UnlockScene3Reward;
        //永久去广/双倍奖励/双倍金币/永久进关去黑/永久进关加一格瓶/永久双倍金币
        public bool ForeverRemoveAds;
        public bool ForeverDoubleBuff;
        public bool ForeverRemoveHide;
        public bool ForeverAddHalfBottle;
        public bool ForeverDoubleCoinBuff;
        //永久每日可领礼包(购买礼包3)
        public bool ForeverDailyReward_ByGiftPack3;

        //体力上限
        public int MaxHp;
        public int HpRecoverTimer;

        public TimedBuffData TimedBuffData;
        public PurchasedGiftPacks GiftPackPurchases;
    }

    /// <summary>
    /// 严格与特殊奖励类型字段相等
    /// 用于反射写入计时器到Json
    /// </summary>
    public class TimedBuffData
    {
        public long RemoveAds;
        public long DoubleCoin;
        //public long UnlimitedHp;
        public long DoubleBuff;
        public long Unlimited_S_AddOneBottle;
        public long Unlimited_S_RemoveOneBottleHideWater;
        public long Unlimited_S_RemoveOneDebuffBottle;
    }

    //特权礼包购买情况记录
    public class PurchasedGiftPacks
    {
        public bool gift_1;
        public bool gift_2;
        public bool gift_3;
        public bool gift_4;
        public bool gift_5;
        public bool gift_6;
    }

    #endregion

    #region Magic Streak Activity Data
    public class MSActivityData
    {
        public int Version;
        public MSAPlayer Player;
        public List<MSARobotsData> MSARobots;
    }

    public class MSAPlayer
    {
        public string PlayerName;
        public int Score;
    }

    public class MSARobotsData
    {
        public int ID;
        public string Name;
        public int Avatar;
        public int AvatarFrame;
        public int MinInitScore;
        public int MaxInitScore;
        public int LimitScore;
        public int Score;
    }

    #endregion

    #region Tier Rank Activity Data

    public class TRActivityData
    {
        public int Version;
        public TRAPlayer Player;
        public List<TRARobotsData> TRARobots;
    }

    public class TRAPlayer
    {
        public string PlayerName;
        public int StreamWinNum;
        public bool IsRewardSettled;
    }

    public class TRARobotsData
    {
        public int ID;
        public string Name;
        public int Avatar;
        public int AvatarFrame;
        public int StreamWinNum;
    }
    #endregion

    #region BattlePass Data

    public class BattlePassData
    {
        public int BattlePassVersion;
        public BPReward[] Rewards;
    }

    public class BPReward
    {
        public int GetConditions;
        public RewardItem[] Free;
        public RewardItem[] Vip;
        public int FreeIsBox; // -1 表示false 0-4表示箱子，5表示特俗图
        public int VipIsBox;
    }

    public class RewardItem
    {
        public string itemType;
        public int itemQuantity;
    }
    #endregion

    #region PrograssGiftADActivityModel
    public class PGData
    {
        public int PGVersion;
        public PGReward[] Rewards;

    }
    public class PGReward
    {
        public float Price;
        public RewardItem[] RewardItem;
    }

    #endregion
}

public class JsonFileUtility : IUtility
{
    /// 注意事项：
    /// 每个Json文件应有 Version 字段
    /// StreamingAssets 下的 Json 应为最新默认版本

    /// <summary>
    /// 从 JSON 文件读取对象
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="action"></param>
    public void LoadFromJson(string filePath, Action<string> action)
    {
        if (!File.Exists(filePath))
        {
            //Debug.Log($"文件不存在: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        action?.Invoke(json);
    }

    /// <summary>
    /// 保存对象为 JSON 文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">沙盒路径</param>
    /// <param name="data"></param>
    public void SaveToJson<T>(string filePath, T data)
    {
        //确保路径存在
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        string _json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, _json);
        //Debug.Log($"数据已保存: {filePath}");
    }

    /// <summary>
    /// 获取Json版本号
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns> -1 表示不存在 </returns>
    public int GetFileVersion(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var versionData = JsonConvert.DeserializeObject<VersionWrapper>(json);
            return versionData.Version;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// 当版本不一致时，使用该方法插入新字段到当前Json中(不覆盖已有字段)
    /// </summary>
    /// <param name="curPath"></param>
    /// <param name="delPath"></param>
    /// 如果默认值是类时间这种(非固定值的),那就需要在代码中匹配-1之类的初始值,然后为字段更新值
    public void AutoFixFields(string curPath, string delPath)
    {
        JObject cur = JObject.Parse(File.ReadAllText(curPath));
        JObject def = JObject.Parse(File.ReadAllText(delPath));

        MergeMissingFields(cur, def);
        // 补完字段后同步更新版本号
        if (def.TryGetValue("Version", out var newVersion))
            cur["Version"] = newVersion;

        File.WriteAllText(curPath, cur.ToString());
    }

    #region 拷贝文件到持久化路径

    private readonly JsonFileInfo[] mJsonFileData = new JsonFileInfo[]
    {
        GameDefine.GameConst.MSADefaultJson,
        GameDefine.GameConst.TRADefaultJson,
        GameDefine.GameConst.BPDefaultJson,
        GameDefine.GameConst.PGDefaultJson,
        GameDefine.GameConst.GAME_GLOBAL_DEFAULT_JSON
    };

    /*public IEnumerator UpdateJsonFiles()
    {
        bool _needUpdate;

        for (int i = 0; i < mJsonFileData.Length; i++)
        {
            _needUpdate = true;

            var _perFilePath = Path.Combine(Application.persistentDataPath, mJsonFileData[i].FileName);
            if (File.Exists(_perFilePath))
            {
                Debug.Log($"文件:{mJsonFileData[i].FileName} 已存在");
                int _localVersion = GetFileVersion(_perFilePath);
                Debug.Log("当前Json版本：" + _localVersion);

                if (_localVersion >= mJsonFileData[i].TargetVersion)
                    _needUpdate = false;

                yield return null;
            }

            if (_needUpdate)
            {
                Debug.Log($"文件:{mJsonFileData[i]} 不存在或版本过低，更新中...");
#if UNITY_ANDROID && !UNITY_EDITOR
                var streamingAssetsFilePath = Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName);
                UnityWebRequest request = UnityWebRequest.Get(streamingAssetsFilePath);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(_perFilePath, request.downloadHandler.data);
                }
                else
                {
                    Debug.LogError("拷贝失败: " + request.error);
                }
#else
                //非安卓
                File.Copy(Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName), _perFilePath, true);
                yield return null;
#endif
            }
        }
    }*/

    public void UpdateJsonFiles()
    {
        bool _needUpdate;
        for (int i = 0; i < mJsonFileData.Length; i++)
        {
            _needUpdate = true;

            var _perFilePath = Path.Combine(Application.persistentDataPath, mJsonFileData[i].FileName);
            if (File.Exists(_perFilePath))
            {
                //Debug.Log($"文件:{mJsonFileData[i].FileName} 已存在");
                int _localVersion = GetFileVersion(_perFilePath);
                //Debug.Log($"{mJsonFileData[i].FileName} 当前版本：" + _localVersion);
                // 读取默认的json比对json的version和targeversion
                if (_localVersion >= mJsonFileData[i].TargetVersion)
                    _needUpdate = false;
            }

            if (_needUpdate)
            {
                //Debug.Log($"文件:{mJsonFileData[i]} 不存在或版本过低，更新中...");
#if UNITY_ANDROID && !UNITY_EDITOR
            var streamingAssetsFilePath = Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName);
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(streamingAssetsFilePath))
            {
               var operation = request.SendWebRequest();
               while (!operation.isDone){ }

               if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
               {
                   File.WriteAllBytes(_perFilePath, request.downloadHandler.data);
               }
               else
               {
                   //Debug.LogError("拷贝失败: " + request.error);
               }
            }
#else
                File.Copy(Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName), _perFilePath, overwrite: true);
#endif
            }
        }
    }

    #endregion

    private void MergeMissingFields(JObject target, JObject template)
    {
        foreach (var prop in template.Properties())
        {
            if (!target.TryGetValue(prop.Name, out var value))
            {
                // 补字段
                target[prop.Name] = prop.Value.DeepClone();
            }
            else
            {
                // 如果是对象则递归
                if (prop.Value.Type == JTokenType.Object)
                {
                    MergeMissingFields(
                        (JObject)value,
                        (JObject)prop.Value
                    );
                }
            }
        }
    }
}
