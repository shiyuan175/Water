using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace SceneUnlock
{
    public partial class UnlockPreview : ViewController
    {
        [SerializeField] private UnlockPreviewNode[] mUnlockPreviewNode;

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
    }
}
