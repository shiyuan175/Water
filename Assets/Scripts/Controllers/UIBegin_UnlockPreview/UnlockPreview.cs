using UnityEngine;
using QFramework;
using UnityEngine.UI;
using QFramework.Example;

namespace SceneUnlock
{
    public partial class UnlockPreview : ViewController
    {
        [SerializeField] private UnlockPreviewNode[] mUnlockPreviewNode;
        [SerializeField] private Button mCloseBtn;

        private SceneUnlockModel mSceneUnlockModel;
      
        private void Awake()
        {
            mSceneUnlockModel = this.GetModel<SceneUnlockModel>();
            ScrollView.verticalNormalizedPosition = 1f;

            for (int i = 0; i < mUnlockPreviewNode.Length; i++)
            {
                mUnlockPreviewNode[i].Init(mSceneUnlockModel);
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
            mCloseBtn.onClick.AddListener(() =>
            {
                UIKit.GetPanel<UIBegin>().MenuBtnEvent(2);
            });
        }
    }
}
