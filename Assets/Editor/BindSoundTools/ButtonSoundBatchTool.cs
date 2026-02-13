using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Water;

public static class ButtonSoundBinder
{
    private static List<MethodInfo> cachedMethods;

    #region Bind

    // ���ڻ�ȡ ButtonSoundExtension ��������޲ξ�̬����
    private static List<MethodInfo> GetStaticParameterlessMethods(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var result = new List<MethodInfo>();
        foreach (var method in methods)
        {
            if (method.GetParameters().Length == 0 && method.ReturnType == typeof(void))
            {
                result.Add(method);
            }
        }
        return result;
    }

    // ���ⲿ���ã�����ѡ��󶨷���
    [MenuItem("Assets/_��ť��Ч/ѡ��󶨷������󶨰�ť��Ч��Prefab��", false, 10)]
    public static void ShowBindMethodSelector()
    {
        // ��ȡ�����޲ξ�̬����
        cachedMethods = GetStaticParameterlessMethods(typeof(ButtonSoundExtension));
        if (cachedMethods.Count == 0)
        {
            Debug.LogWarning("δ�ҵ��κη��������ľ�̬�޲η���");
            return;
        }

        // �����༭�����ڣ���ѡ��
        ButtonSoundBinderWindow.ShowWindow();
    }

    // ��֤ѡ�е���Prefab
    [MenuItem("Assets/_��ť��Ч/ѡ��󶨷������󶨰�ť��Ч��Prefab��", true)]
    public static bool ValidateShowBindMethodSelector()
    {
        return Selection.activeObject is GameObject prefab &&
               AssetDatabase.GetAssetPath(prefab).EndsWith(".prefab");
    }

    #endregion

    #region UnBind
    [MenuItem("Assets/_��ť��Ч/ѡ���󷽷����Ƴ���ť��Ч��Prefab��", false, 11)]
    public static void ShowUnbindMethodSelector()
    {
        cachedMethods = GetStaticParameterlessMethods(typeof(ButtonSoundExtension));
        if (cachedMethods.Count == 0)
        {
            Debug.LogWarning("δ�ҵ��κη��������ľ�̬�޲η���");
            return;
        }

        ButtonSoundUnbinderWindow.ShowWindow(); // ���ý�󴰿�
    }

    [MenuItem("Assets/_��ť��Ч/ѡ���󷽷����Ƴ���ť��Ч��Prefab��", true)]
    public static bool ValidateShowUnbindMethodSelector()
    {
        return Selection.activeObject is GameObject prefab &&
               AssetDatabase.GetAssetPath(prefab).EndsWith(".prefab");
    }
    /* // ���ָ�������¼�
     [MenuItem("Assets/_��ť��Ч/ѡ���󷽷����Ƴ���ť��Ч��Prefab��", false, 11)]
     public static void ShowUnbindMethodSelector()
     {
         cachedMethods = GetStaticParameterlessMethods(typeof(ButtonSoundExtension));
         if (cachedMethods.Count == 0)
         {
             Debug.LogWarning("δ�ҵ��κη��������ľ�̬�޲η���");
             return;
         }

         ButtonSoundBinderWindow.ShowWindow();
         //ButtonSoundBinderWindow.ShowWindow(true);
     }

     [MenuItem("Assets/_��ť��Ч/ѡ���󷽷����Ƴ���ť��Ч��Prefab��", true)]
     public static bool ValidateShowUnbindMethodSelector()
     {
         return Selection.activeObject is GameObject prefab &&
                AssetDatabase.GetAssetPath(prefab).EndsWith(".prefab");
     }*/
    #endregion

    // ���༭�����ڷ��ʷ���
    public static List<MethodInfo> GetCachedMethods() => cachedMethods;
}
