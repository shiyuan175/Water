using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    private ResLoader mResLoader = null;

    private void Start()
    {
        ResKit.Init();

        mResLoader = ResLoader.Allocate();
        // 同步加载 
        ActionKit.Delay(10, () => { mResLoader.LoadSceneSync("SampleScene"); });
    }

    // Update is called once per frame
    private void Update()
    {
    }
}