#if UNITY_EDITOR
using QFramework.Example;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.MaterialProperty;

public class AddPrefabTool : EditorWindow
{
    private string _filePath = "YourFilePath";
    private string _parent = "YourParent";
    private GameObject _prefab;

    [MenuItem("Tools/AddPrefabTool")]
    public static void ShowWindow()
    {
        GetWindow<AddPrefabTool>();
    }

    private void OnGUI()
    {
        GUILayout.Label("������ģ���������", EditorStyles.boldLabel);
        _filePath = EditorGUILayout.TextField("����ӵ�����·��", _filePath);
        _prefab = (GameObject)EditorGUILayout.ObjectField("��ӵ�����", _prefab, typeof(GameObject), true);
        if (GUILayout.Button("���"))
        {
            AddPrefab();
        }
    }

    private void AddPrefab()
    {
        // ����ָ���ļ����е�����Ԥ����
        string[] guids = AssetDatabase.FindAssets("UIRankClear t:Prefab", new[] { _filePath });
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        // ��Ԥ������б༭
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(assetPath);

        // ʵ����Ԥ����
        GameObject instantiate = PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
        UIRankClear panel = instantiate.GetComponent<UIRankClear>();


        List<string> nameList = new List<string>();
        List<string> starList = new List<string>();
        TextAsset Rank = Resources.Load<TextAsset>("Text/Rank");
        string[] textList = Rank.text.Split("\r\n");

        foreach (var item in textList)
        {
            item.Replace(" ", "");
            //Debug.Log(item);
            string[] textPair = item.Split("\t", 2);
            nameList.Add(textPair[0].Trim());
            starList.Add(textPair[1].Trim());
        }
        foreach (var item in panel.rankItemCtrls)
        {

            //item.GetText();
        }
        //for(int i = 0; i < panel.rankNode.childCount; i++)
        //{
        //    var item = panel.rankNode.GetChild(i).GetComponent<RankItemCtrl>();
        //    panel.rankItemCtrls.Add(item);
        //    //item.GetText(i, nameList[i], starList[i]);
        //}
        //prefabInstance = instantiate;
        // �����Ԥ������޸�
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, assetPath);
        // ж��Ԥ��������
        PrefabUtility.UnloadPrefabContents(prefabInstance);
        AssetDatabase.Refresh();

        // ˢ���ʲ����ݿ�
        AssetDatabase.Refresh();
    }
}
#endif