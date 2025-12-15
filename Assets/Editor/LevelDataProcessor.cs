#if UNITY_EDITOR
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using GameDefine;
using System.IO;
using System.Linq;

public class LevelDataProcessor : EditorWindow
{
    private static Dictionary<int, List<int>> Clearlist = new();
    private LevelCreateCtrl currentLevel;

    [MenuItem("Tools/关卡数据处理工具")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataProcessor>("关卡数据");
    }

    private void OnGUI()
    {
        GUILayout.Label(" 批量处理工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("功能说明：", EditorStyles.boldLabel);
        GUILayout.Label("1. 遍历项目中所有 LevelCreateCtrl SO 文件");
        GUILayout.Space(20);

        if (GUILayout.Button("开始处理所有关卡", GUILayout.Height(40)))
        {
            ProcessAllLevels();
        }

        currentLevel = (LevelCreateCtrl)EditorGUILayout.ObjectField(
            "Level:",
            currentLevel,
            typeof(LevelCreateCtrl),
            true);
        if (GUILayout.Button("开始处理单一关卡", GUILayout.Height(40)))
        {
            ProcessOneLevels(currentLevel);
        }

        GUILayout.Space(20);
        GUILayout.Label("注意事项：", EditorStyles.boldLabel);
        GUILayout.Label("• 操作前请确保已保存场景");
        GUILayout.Label("• 处理完成后请手动保存资源");
        GUILayout.Label("• 建议先备份重要数据");
    }

    private static void ProcessOneLevels(LevelCreateCtrl level)
    {
        // 处理每个瓶子
        for (int bottleIndex = 0; bottleIndex < level.bottles.Count; bottleIndex++)
        {
            var bottle = level.bottles[bottleIndex];

            // 处理water
            bottle.waterSet.RemoveAll(x => x == 0);
            if (bottle.numCake == 0)
            {
                bottle.numCake = 4;
            }

            while (bottle.bombCounts.Count < bottle.waterSet.Count)
            {
                bottle.bombCounts.Add(0);
            }

            while (bottle.bombCounts.Count > bottle.waterSet.Count)
            {
                bottle.bombCounts.RemoveAt(bottle.bombCounts.Count - 1);
            }

            // 2. 确保 waterItem 与 waterSet 长度相同
            while (bottle.waterItem.Count < bottle.waterSet.Count)
            {
                bottle.waterItem.Add(WaterItem.None);
            }

            while (bottle.waterItem.Count > bottle.waterSet.Count)
            {
                bottle.waterItem.RemoveAt(bottle.waterItem.Count - 1);
            }

            // 3. 确保 hideTypes 与 waterSet 长度相同
            while (bottle.hideTypes.Count < bottle.waterSet.Count)
            {
                bottle.hideTypes.Add(HideWaterType.None);
            }

            while (bottle.hideTypes.Count > bottle.waterSet.Count)
            {
                bottle.hideTypes.RemoveAt(bottle.hideTypes.Count - 1);
            }

            while (bottle.BlackBottleList.Count < bottle.waterSet.Count)
            {
                bottle.BlackBottleList.Add(false);
            }

            // 处理每个水层
            for (int waterLayerIndex = 0; waterLayerIndex < bottle.waterSet.Count; waterLayerIndex++)
            {
                int waterValue = bottle.waterSet[waterLayerIndex];
                if (waterValue == 5002)
                    bottle.waterItem[waterLayerIndex] = WaterItem.FlyBomb;
                if ((waterValue < 1000 && waterValue != 0) ||
                    (waterValue == 4001 || waterValue == 4002))
                {
                    if (!Clearlist.ContainsKey(waterValue))
                    {
                        Clearlist.Add(waterValue, new List<int>()); // 改为存储 int 列表
                        Clearlist[waterValue].Add(bottleIndex); // 添加 bottle 索引
                    }
                    else
                    {
                        Clearlist[waterValue].Add(bottleIndex); // 添加 bottle 索引
                    }
                }

                if (bottle.bombCounts[waterLayerIndex] != 0 &&
                    bottle.waterItem[waterLayerIndex] == WaterItem.None &&
                    waterValue < 1000)
                {
                    bottle.waterItem[waterLayerIndex] = WaterItem.Bomb;
                }

                if (waterValue == 5002)
                {
                    bottle.waterItem[waterLayerIndex] = WaterItem.FlyBomb;
                }
            }
        }

        level.clearList.Clear();
        level.clearList = Clearlist.Keys.ToList();
        // 检查水的数量
        foreach (var data in Clearlist)
        {
            if (data.Value.Count != 4)
            {
                Debug.Log($"处理关卡: {level.name}");
                Debug.Log($"水层数量错误: {data.Key}");
                foreach (var bottle in data.Value)
                {
                    Debug.Log(bottle);
                }
            }
        }

        Clearlist.Clear();
        // 保存所有修改
        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ProcessAllLevels()
    {
        // 查找所有 LevelCreateCtrl 类型的 ScriptableObject
        string[] guids = AssetDatabase.FindAssets("t:LevelCreateCtrl");

        if (guids.Length == 0)
        {
            Debug.LogWarning("未找到任何 LevelCreateCtrl 类型的资源文件");
            return;
        }

        int processedCount = 0;
        int totalBottles = 0;
        int totalUpdates = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            LevelCreateCtrl level = AssetDatabase.LoadAssetAtPath<LevelCreateCtrl>(assetPath);

            if (level == null) continue;
            ProcessOneLevels(level);
        }


        Debug.Log($"处理完成！");
        Debug.Log($"总关卡数: {guids.Length}");
        Debug.Log($"已修改关卡: {processedCount}");
        Debug.Log($"总瓶子数: {totalBottles}");
        Debug.Log($"总更新字段数: {totalUpdates}");

        EditorUtility.DisplayDialog("处理完成",
            $"已处理 {processedCount}/{guids.Length} 个关卡文件\n" +
            $"更新了 {totalUpdates} 个字段", "确定");
    }
}
#endif