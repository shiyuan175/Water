using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ResourceReferenceFinder : EditorWindow
{
    private List<Object> referencingAssets = new List<Object>();
    private Vector2 scrollPosition;

    [MenuItem("Assets/ZYT ASSETS/Find References", true)]
    private static bool ValidateSearchReference()
    {
        // ֻ��ѡ���˶����Ҳ����ļ���ʱ����ʾ�˵���
        return Selection.activeObject != null && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    [MenuItem("Assets/ZYT ASSETS/Find References")]
    private static void SearchReference()
    {
        // ����������Դ���ò��Ҵ���
        if (Selection.activeObject != null && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject)))
        {
            GetWindow<ResourceReferenceFinder>("Resource Reference Finder").ReferenceFinder(Selection.activeObject);
        }
    }

    private void OnGUI()
    {
        // ��ʾ�������
        EditorGUILayout.LabelField("Search Results:");
        // ������ͼ��ʼ
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.BeginVertical();

        // ��ʾ�������
        for (int i = 0; i < referencingAssets.Count; i++)
        {
            EditorGUILayout.ObjectField(referencingAssets[i], typeof(Object), true, GUILayout.Width(300));
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        // ������ͼ����
    }

    // ��������
    private void ReferenceFinder(Object targetResource)
    {
        referencingAssets.Clear();

        // ��ȡѡ����Դ�� GUID
        string assetPath = AssetDatabase.GetAssetPath(targetResource);
        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

        // ������Ŀ������ Prefab �ļ�
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int length = guids.Length;
        for (int i = 0; i < length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayCancelableProgressBar("Checking", filePath, (float)i / length);

            // ��ȡ Prefab �ļ����ݣ�����Ƿ����ѡ����Դ�� GUID
            string content = File.ReadAllText(filePath);
            if (content.Contains(assetGuid))
            {
                // ������������� Prefab ��ӵ�����б���
                Object referencingAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                referencingAssets.Add(referencingAsset);
            }
        }

        // ���������
        EditorUtility.ClearProgressBar();
    }
}