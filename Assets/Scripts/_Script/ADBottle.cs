using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ADBottle : MonoBehaviour
{
    private Transform mParTras;
    private Button mADBottle;

    private void Awake()
    {
        mADBottle = GetComponent<Button>();
        mParTras = transform.parent;
    }

    private void Start()
    {
        mADBottle.onClick.AddListener(() =>
        {
            TopOnADManager.Instance.ShowVideoAd(
                () =>
                {
                    ActionKit.DelayFrame(5, () =>
                    {
                        LevelManager.Instance.AddBottle(false, UpdateADBottle);
                    }).Start(this);
                }, null);
        });
    }

    public void UpdateADBottle()
    {
        //16个瓶子为上限
        if (LevelManager.Instance.nowBottles.Count >= 16
            || LevelManager.Instance.levelId <= 2 ||
            LevelManager.Instance.levelId == 11)
        {
            mParTras.Hide();
            LevelManager.Instance.UpdateButtomLayoutSpcing();
            return;
        }

        mParTras.Show();
        int topAc = LevelManager.Instance.topBottle.Count(b => b.gameObject.activeSelf);
        int bomAc = LevelManager.Instance.bottomBottle.Count(b => b.gameObject.activeSelf);

        if (topAc > bomAc)  
        {
            var bottomNode = LevelManager.Instance.bottomBottle[0].transform.parent;
            bottomNode.Show();
            mParTras.SetParent(bottomNode);
        }
        else
        {
            var topNode = LevelManager.Instance.topBottle[0].transform.parent;
            mParTras.SetParent(topNode);
        }

        LevelManager.Instance.UpdateTopLayoutSpcing();
        LevelManager.Instance.UpdateButtomLayoutSpcing();
    }
}
