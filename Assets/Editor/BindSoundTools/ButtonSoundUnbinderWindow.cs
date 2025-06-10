using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ButtonSoundUnbinderWindow : EditorWindow
{
    private static GameObject selectedPrefab;
    private static GameObject prefabRoot;
    private static List<Button> buttons;
    private static List<MethodInfo> soundMethods;
    private static string[] methodNames;
    private static string[] buttonBindings; // ��¼ÿ����ť��ǰ�󶨵ķ�������û��Ϊnull���

    public static void ShowWindow()
    {
        if (!(Selection.activeObject is GameObject prefab))
        {
            Debug.LogWarning("��ѡ��һ��Prefab");
            return;
        }

        selectedPrefab = prefab;
        string prefabPath = AssetDatabase.GetAssetPath(selectedPrefab);
        prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        buttons = new List<Button>(prefabRoot.GetComponentsInChildren<Button>(true));

        soundMethods = ButtonSoundBinder.GetCachedMethods();
        if (soundMethods == null || soundMethods.Count == 0)
        {
            Debug.LogWarning("δ�ҵ���̬�޲���Ч����������ִ�а󶨴����Ի��淽��");
            return;
        }

        methodNames = new string[soundMethods.Count];
        for (int i = 0; i < soundMethods.Count; i++)
        {
            methodNames[i] = soundMethods[i].Name;
        }

        buttonBindings = new string[buttons.Count];
        // ��ȡÿ����ť��ǰ�󶨵���Ч�������ƣ����һ����
        for (int i = 0; i < buttons.Count; i++)
        {
            buttonBindings[i] = GetButtonBoundMethodName(buttons[i]);
        }

        GetWindow<ButtonSoundUnbinderWindow>("��ť��Ч��󹤾�").Show();
    }

    private void OnGUI()
    {
        if (buttons == null || buttons.Count == 0)
        {
            EditorGUILayout.LabelField("δ�ҵ��κ� Button");
            return;
        }

        EditorGUILayout.LabelField("��ǰ���а�ť�󶨵���Ч����", EditorStyles.boldLabel);

        for (int i = 0; i < buttons.Count; i++)
        {
            var btn = buttons[i];
            if (btn == null) continue;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(btn.name, GUILayout.Width(200));
            EditorGUILayout.LabelField(string.IsNullOrEmpty(buttonBindings[i]) ? "<�ް�>" : buttonBindings[i], GUILayout.Width(150));

            if (!string.IsNullOrEmpty(buttonBindings[i]))
            {
                if (GUILayout.Button("���", GUILayout.Width(80)))
                {
                    UnbindMethodFromButton(btn, buttonBindings[i]);
                    buttonBindings[i] = null; // ������ʾ
                    SavePrefab();
                }
            }
            else
            {
                GUILayout.Space(80); // ���ְ�ť���ռλ
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("����������а���Ч"))
        {
            int unbindCount = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (!string.IsNullOrEmpty(buttonBindings[i]))
                {
                    UnbindMethodFromButton(buttons[i], buttonBindings[i]);
                    buttonBindings[i] = null;
                    unbindCount++;
                }
            }
            if (unbindCount > 0)
            {
                SavePrefab();
                Debug.Log($"���������ɣ������ {unbindCount} ����ť");
            }
            else
            {
                Debug.Log("û�м�⵽�󶨵İ�ť���������");
            }
        }
    }

    private static string GetButtonBoundMethodName(Button button)
    {
        int count = button.onClick.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            var target = button.onClick.GetPersistentTarget(i);
            var methodName = button.onClick.GetPersistentMethodName(i);
            // ֻ���target==null����������Ч�����б��еİ�
            if (target == null && soundMethods.Exists(m => m.Name == methodName))
            {
                return methodName;
            }
        }
        return null;
    }

    private void UnbindMethodFromButton(Button button, string methodName)
    {
        int count = button.onClick.GetPersistentEventCount();
        for (int i = count - 1; i >= 0; i--)
        {
            var m = button.onClick.GetPersistentMethodName(i);
            var target = button.onClick.GetPersistentTarget(i);
            if (target == null && m == methodName)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            }
        }
        Debug.Log($"�ѽ�󷽷� {methodName} �Ӱ�ť {button.name}");
    }

    private void SavePrefab()
    {
        if (prefabRoot != null && selectedPrefab != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedPrefab);
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            Debug.Log("Prefab �������");
        }
    }

    private void OnDestroy()
    {
        if (prefabRoot != null)
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            prefabRoot = null;
            Debug.Log("Prefab ��Դ���ͷ�");
        }
    }
}
