using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

[MonoSingletonPath("[Share]/ShareManager")]
public class ShareManager : MonoSingleton<ShareManager>, ICanGetUtility, ICanSendEvent
{
    TextManager textManager;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public void ShareImg(string path)
    {
        StartCoroutine(TakeShareImg(path));
    }

    public void ShareUrl(string url = "")
    {
        //StartCoroutine(TakeNormalShare(url));      
        StartCoroutine(TakeNormalShare(url));
    }

    public void ShareScreen()
    {
        //StartCoroutine(TakeNormalShare(url));      
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeShareImg(string path, string url = "")
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(path);

        if (textManager == null)
        {
            textManager = TextManager.Instance;
        }

        Sprite img = Resources.Load<Sprite>(path);

        string filePath = Path.Combine(Application.temporaryCachePath, "result.png");
        File.WriteAllBytes(filePath, img.texture.EncodeToPNG());

        // To avoid memory leaks
        //Destroy(ss);

        //NativeShare share = 
        string text1 = textManager.GetConvertText("Text_ShareLook1") + "\r\n" + "https://www.baidu.com" + "\r\n";
        string text2 = textManager.GetConvertText("Text_ShareLook2" ) + "\r\n";
        string text3 = textManager.GetConvertText("Text_ShareLook3" ) + "\r\n" + "https://www.baidu.com";

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText(text1 + text2 + text3)
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        //if(url != "")
        //{
        //    share = share.SetUrl(url);

        //}
        //share.Share();
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }

    private IEnumerator TakeNormalShare(string url)
    {
        yield return new WaitForEndOfFrame();

        // To avoid memory leaks
        //Destroy(ss);

        NativeShare share =
        new NativeShare();
        if (url != "")
        {
            share = share.SetUrl(url);
            share.Share();
        }
        else
        {
            share = share.SetUrl("https://www.baidu.com");
            //.SetUrl("https://play.google.com/store/apps/details?id=");
            share.Share();
        }
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Debug.Log("00000000000000");
        Texture2D ss = new Texture2D((int)Screen.width, (int)Screen.height, TextureFormat.RGB24, false);

        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
    
        string filePath = Path.Combine(Application.temporaryCachePath, "shared11img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        //new NativeShare().AddFile(filePath)
        //    .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
        //    .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        //    .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
