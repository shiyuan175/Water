using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class MSARankNodePool : MonoSingleton<MSARankNodePool>
{
    private const string RankNodePrefabPath = "Prefab/MSA_RankNode";
    private Transform mMSARankNodesPar;
    private SimpleObjectPool<GameObject> mRankNodePool;

    public override void OnSingletonInit()
    {
        mMSARankNodesPar = new GameObject(name: "MSARankNodes").transform;

        mRankNodePool = new SimpleObjectPool<GameObject>(
        () =>
        {
            var obj = Resources.Load(RankNodePrefabPath);
            var node = Instantiate(obj, mMSARankNodesPar) as GameObject;
            node.Hide();
            return node;
        },
        (obj) =>
        {
            obj.GetComponent<MSANodeCtrl>().DisInit();
            obj.transform.SetParent(mMSARankNodesPar, false);
            obj.Hide();
        },
        51);
    }

    public GameObject Allocate()
    {
        var _obj = mRankNodePool.Allocate();
        _obj.Show();
        return _obj;
    }

    public void Recycle(GameObject obj)
    {
        mRankNodePool.Recycle(obj);
    }
}
