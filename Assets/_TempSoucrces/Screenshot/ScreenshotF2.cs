using UnityEngine;
using System.IO;
using System;

public class ScreenshotF2 : MonoBehaviour
{
    private void Update()
    {
        // F10 截图
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Capture();
        }
    }

    private void Capture()
    {
        string folder = Path.Combine(Application.dataPath, "_TempSoucrces/Screenshot");

        // 没有就创建
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        // 用当前 ticks 作为文件名
        string fileName = DateTime.Now.Ticks + ".png";

        string fullPath = Path.Combine(folder, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);

        Debug.Log($"截图已保存: {fullPath}");
    }
}
