using UnityEngine;
using QFramework;
using UnityEngine.UI;
using QFramework.Example;

namespace SceneUnlock
{
    public partial class UnlockPreview : ViewController
    {
        [SerializeField] private UnlockPreviewNode[] mUnlockPreviewNode;
        [SerializeField] private Button mPlot2Guide;
        [SerializeField] private Button mCloseBtn;

        private SceneUnlockModel mSceneUnlockModel;
        private GameGlobalModel mGameGlobalModel;

        private void Awake()
        {
            mSceneUnlockModel = this.GetModel<SceneUnlockModel>();
            mGameGlobalModel = this.GetModel<GameGlobalModel>();

            ScrollView.verticalNormalizedPosition = 1f;

            foreach (var plotNode in mUnlockPreviewNode)
            {
                plotNode.Init(mSceneUnlockModel, mGameGlobalModel);
            }
        }

        private void OnEnable()
        {
            ScrollView.verticalNormalizedPosition = 1f;
            foreach (var item in mUnlockPreviewNode)
            {
                item.CheckUnlockFinish();
            }
        }

        private void Start()
        {
            mCloseBtn.onClick.AddListener(() => {
                UIKit.GetPanel<UIBegin>().MenuBtnEvent(2);
            });

            //仅在首套场景注册该事件
            if (mSceneUnlockModel.SceneIndex <= 0)
                StringEventSystem.Global.Register(GameDefine.GameConst.SCENE_UNLOCK_GUIDE_STEP2, PlotGuideEvent);
        }

        private void PlotGuideEvent()
        {
            mPlot2Guide.onClick?.Invoke();
            StringEventSystem.Global.UnRegister(GameDefine.GameConst.SCENE_UNLOCK_GUIDE_STEP2, PlotGuideEvent);
        }
    }
}
