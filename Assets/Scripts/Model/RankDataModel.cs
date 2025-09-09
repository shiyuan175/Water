using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using A_PlayerRankData;
using QFramework;
using UnityEngine;

namespace A_PlayerRankData
{
    public struct PlayerRankData
    {
        public readonly string playerName;
        public readonly int trophyCount;

        public PlayerRankData(string name, int trophies)
        {
            playerName = name;
            trophyCount = trophies;
        }
    }
}

//排行榜数据
public class RankDataModel : AbstractModel
{
    private readonly string FILE_PATH_RELATIVE = "RankExcel/Rank.csv";

    //可以使用索引来做排名
    private List<PlayerRankData> mRankListData;
    public IReadOnlyList<PlayerRankData> RankDataList => mRankListData;

    //还需记录一个排行榜宝箱是否开启的状态
    //启动时，排行榜活动结束且宝箱未开启
    //打开排行榜面板，做一个领取奖励动画，领取完关闭，标记已领取

    protected async override void OnInit()
    {
        mRankListData = new List<PlayerRankData>();
        await LoadRankDataAsync();
        //Debug.Log("已加载" + mRankListData.Count + "条排名数据。");
    }

    #region LoadRankCSV

    private async Task LoadRankDataAsync()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, FILE_PATH_RELATIVE);

        string fileContent = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(filePath))
        {
            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone)
                await Task.Yield();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                //Debug.LogError("读取文件失败: " + www.error);
                return;
            }
            else
            {
                fileContent = www.downloadHandler.text;
            }
        }
#else
        if (File.Exists(filePath))
        {
            fileContent = await Task.Run(() => File.ReadAllText(filePath));
        }
        else
        {
            //Debug.LogError("文件不存在：" + filePath);
            return;
        }
#endif

        ParseCSV(fileContent);
    }

    private void ParseCSV(string content)
    {
        mRankListData.Clear();
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            var parts = trimmedLine.Split(',');
            if (parts.Length < 2)
                continue;

            string name = parts[0].Trim();
            if (int.TryParse(parts[1].Trim(), out int trophies))
            {
                mRankListData.Add(new PlayerRankData(name, trophies));
            }
            else
            {
                //Debug.LogWarning($"数据解析失败，行内容：{line}");
            }
        }
    }

    #endregion

    /// <summary>
    /// 获取score在排行榜中的实际排名（从1开始）
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public int GetPlayerRank(int score)
    {
        int left = 0;
        int right = mRankListData.Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (mRankListData[mid].trophyCount >= score)
                left = mid + 1;
            else
                right = mid - 1;
        }
        return left + 1;
    }
}
