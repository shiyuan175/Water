#if UNITY_EDITOR
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Codice.Client.BaseCommands;
using Game.Water;

public enum LevelOperationType
{
    None = 0,
    RemoveOneLevelHide = 1 << 0,
    RemoveOneLevelBomb = 1 << 1,
}
public class LevelDataProcessor : EditorWindow
{
    private static Dictionary<int, List<int>> Clearlist = new();
    private LevelCreateCtrl currentLevel;
    private LevelCreateCtrl originLevel;
    private LevelCreateCtrl copyLevel;
    private LevelOperationType selectedOperations = LevelOperationType.None;
    private bool showDropdown = false;
    private Rect dropdownRect;
    private bool removeHideSelected;
    private bool removeBombSelected;
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

        /*if (GUILayout.Button("开始删除单一关卡的黑水", GUILayout.Height(40)))
        {
            RemoveOneLevelHide(currentLevel);
        }

        if (GUILayout.Button("开始删除单一关卡的普通炸弹", GUILayout.Height(40)))
        {
            RemoveOneLevelBomb(currentLevel);
        }*/
        if (GUILayout.Button("选择功能 ▼", GUILayout.Height(40)))
        {
            showDropdown = !showDropdown;
        }

        // 如果展开，显示复选框选择框
        if (showDropdown)
        {
            // 创建一个浮动窗口样式的选择框
            GUILayout.BeginVertical("window", GUILayout.Width(200));

            // 复选框选项
            removeHideSelected = (selectedOperations & LevelOperationType.RemoveOneLevelHide) != 0;
            removeBombSelected = (selectedOperations & LevelOperationType.RemoveOneLevelBomb) != 0;
            // 更新选择状态
            removeHideSelected = GUILayout.Toggle(removeHideSelected, "删除黑水");
            removeBombSelected = GUILayout.Toggle(removeBombSelected, "删除普通炸弹");
            // 更新位掩码
            selectedOperations = LevelOperationType.None;
            if (removeHideSelected) selectedOperations |= LevelOperationType.RemoveOneLevelHide;
            if (removeBombSelected) selectedOperations |= LevelOperationType.RemoveOneLevelBomb;

            // 确认按钮
            if (GUILayout.Button("确认选择"))
            {
                showDropdown = false;
            }

            GUILayout.EndVertical();
        }

        if (GUILayout.Button("执行选中功能", GUILayout.Height(40)))
        {
            ExecuteOperations(currentLevel);
        }
        /*// 创建复选框组
        removeHideSelected = (selectedOperations & LevelOperationType.RemoveOneLevelHide) != 0;
        removeBombSelected = (selectedOperations & LevelOperationType.RemoveOneLevelBomb) != 0;*/


        originLevel = (LevelCreateCtrl)EditorGUILayout.ObjectField(
            "被覆盖的的关卡",
            originLevel,
            typeof(LevelCreateCtrl),
            true);

        copyLevel = (LevelCreateCtrl)EditorGUILayout.ObjectField(
            "被复制的关卡:",
            copyLevel,
            typeof(LevelCreateCtrl),
            true);
        if (GUILayout.Button("复制整个关卡数据", GUILayout.Height(40)))
        {
            CopyOneLevelData(copyLevel, originLevel);
        }
        GUILayout.Space(20);
        GUILayout.Label("注意事项：", EditorStyles.boldLabel);
        GUILayout.Label("• 操作前请确保已保存场景");
        GUILayout.Label("• 处理完成后请手动保存资源");
        GUILayout.Label("• 建议先备份重要数据");
    }

    private static void CopyOneLevelData(LevelCreateCtrl source, LevelCreateCtrl target)
    {
        // 使用所有默认选项复制
        target.gameType = source.gameType;
        target.countDownNum = source.countDownNum;
        target.timeCountDown = source.timeCountDown;
        target.topNum = source.topNum;
        target.bottomNum = source.bottomNum;
        target.GlobalMechanismBeginSetp = source.GlobalMechanismBeginSetp;
        target.GlobalMechanismContinueSetps = source.GlobalMechanismContinueSetps;
        target.bottles = new(source.bottles);
        target.clearList = new(source.clearList);
        target.hideList = new(source.hideList);
        target.hideTypes = new(source.hideTypes);
        target.bubbleCount = new(source.bubbleCount);
        target.changeList = new(source.changeList);
        target.globalMechanism = source.globalMechanism;

        EditorUtility.SetDirty(target);
    }

    private static void RemoveOneLevelHide(LevelCreateCtrl level)
    {
        for (int bottleIndex = 0; bottleIndex < level.bottles.Count; bottleIndex++)
        {
            var bottle = level.bottles[bottleIndex];
            for (int waterLayerIndex = 0; waterLayerIndex < bottle.waterSet.Count; waterLayerIndex++)
            {
                bottle.hideTypes[waterLayerIndex] = HideWaterType.None;
            }
        }

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ExecuteOperations(LevelCreateCtrl level)
    {
        if ((selectedOperations & LevelOperationType.RemoveOneLevelHide) != 0)
        {
            RemoveOneLevelHide(level);
        }

        if ((selectedOperations & LevelOperationType.RemoveOneLevelBomb) != 0)
        {
            RemoveOneLevelBomb(level);
        }

        if (selectedOperations == LevelOperationType.None)
        {
            Debug.LogWarning("请至少选择一个功能！");
        }
    }

    private static void RemoveOneLevelBomb(LevelCreateCtrl level)
    {
        for (int bottleIndex = 0; bottleIndex < level.bottles.Count; bottleIndex++)
        {
            var bottle = level.bottles[bottleIndex];
            for (int waterLayerIndex = 0; waterLayerIndex < bottle.waterSet.Count; waterLayerIndex++)
            {
                if (bottle.bombCounts[waterLayerIndex] != 0 || bottle.waterItem[waterLayerIndex] == WaterItem.Bomb)
                {
                    bottle.bombCounts[waterLayerIndex] = 0;
                    bottle.waterItem[waterLayerIndex] = WaterItem.None;
                }
            }
        }

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
