using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[MonoSingletonPath("[Resource]/ResourceManager")]
public class ResourceManager : MonoSingleton<ResourceManager>, ICanGetUtility, ICanSendEvent
{
    private ResLoader mResLoader = ResLoader.Allocate();
    private SpriteAtlas mtttt;
    //private List<Sprite> mRankLevelSprites;

    public override void OnSingletonInit()
    {
        mResLoader.Add2Load<SpriteAtlas>("RankLevelAtlas", (success, atlas) =>
        {
            if (success)
            {
                mtttt = atlas.Asset as SpriteAtlas;
                Debug.Log("图集加载完成，共 " + mtttt.spriteCount + " 个 Sprite");
                Debug.Log(mtttt.name);
                Debug.Log(mtttt.GetSprite($"{mtttt.name}_1"));
            }
            else
            {
                Debug.LogError("图集加载失败！");
            }
        });


        mResLoader.LoadAsync();
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
