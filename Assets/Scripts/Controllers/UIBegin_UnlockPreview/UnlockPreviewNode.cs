using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPreviewNode : MonoBehaviour
{
    [SerializeField] private Image mImgPreview;
    [SerializeField] private Image mBlueFrame;
    [SerializeField] private Image mLock;
    [SerializeField] private Button mGreyFrame;

    [SerializeField] private Sprite mNewSprite;

    [Header("场景索引(从0开始)、当前场景部件数、上个场景部件数")]
    [SerializeField] private int mSceneIdx;
    [SerializeField] private int mThisScenePartTotal;
    [SerializeField] private int mPrevScenePartTotal;
    [SerializeField] private bool mRegisterEvent;

    private SceneUnlockModel mSceneUnlockModel;

    public void Init(SceneUnlockModel sceneUnlockModel)
    {
        mSceneUnlockModel = sceneUnlockModel;
        if (sceneUnlockModel.SceneIndex >= mSceneIdx)
        {
            mBlueFrame.DOFade(1, 0f);
            mGreyFrame.Hide();
            mLock.Hide();
        }
    }

    public void CheckUnlockFinish()
    {
        if (mImgPreview.sprite == mNewSprite)
            return;
        if (mSceneUnlockModel.GetSceneUnitIndex(mSceneIdx) >= mThisScenePartTotal)
            mImgPreview.sprite = mNewSprite;
    }
   
    private void Start()
    {
        if (mSceneUnlockModel.SceneIndex < mSceneIdx && mSceneIdx > 0)
            mGreyFrame.onClick.AddListener(UnlockEvent);

        if (mRegisterEvent)
        {
            StringEventSystem.Global.Register(GameDefine.GameConst.SCENE_UNLOCK_GUIDE_STEP2, () =>
            {
                mGreyFrame.onClick.Invoke();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }

    void UnlockEvent()
    {
        if (mSceneUnlockModel.GetSceneUnitIndex(mSceneIdx - 1) >= mPrevScenePartTotal)
        {
            UIKit.OpenPanel<UIMask>();
            mGreyFrame.onClick.RemoveListener(UnlockEvent);
            mGreyFrame.interactable = false;
            mSceneUnlockModel.UpdateSceneIdx(mSceneIdx);
            mBlueFrame.DOFade(1, 1.5f);
            mGreyFrame.image.DOFade(0, 1.2f)
                .OnComplete(() =>
                {
                    mGreyFrame.Hide();
                    mLock.Hide();
                    UIKit.ClosePanel<UIMask>();
                });
        }
    }
}
