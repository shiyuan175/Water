using UnityEngine;
using QFramework;
using UnityEngine.Rendering;
using TMPro;
using JsonFileData;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class BattlePassContentPanel : ViewController,ICanGetModel
	{  
        [SerializeField] public Sprite[] boxImgs;
        [SerializeField] public Sprite[] levelImgs; // 0表示还不能，1表示能

        [SerializeField] private RewardSpriteMappingSO rewardSprite;
		private BattlePassADActivity mBattlePassADActivity;
        private BattlePassModel bPModel;

      
        public void Awake()
        {
            // 获取逻辑层，将奖励发放等功能转交给逻辑层处理
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();

            bPModel = this.GetModel<BattlePassModel>();
        }
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
     
        public void Initialize(int level)
        {
            // awake获取到了，这里为null，所以补了一个get
            bPModel = this.GetModel<BattlePassModel>();

            // 获取内容
            RewardItem[] freeData=bPModel.BPDate.Rewards[level].Free;

        }

        // 设置内容
        public void SetItemPanel(int level,bool isAlReceiew ,bool isVip =false)
        {

        }
        /// <summary>
        /// 设置按钮的点击事件
        /// </summary>
        /// <param name="freePack"></param>
        /// <param name="vipPack"></param>
        public void SetBtnOnClike(RewardItem[] freeReward, RewardItem[] vipReward)
        {
          /*  BtnFreeClaim.onClick.AddListener(() =>
            {
                mBattlePassADActivity.DistributeReward(freeReward, false);

            });
            BtnRechargeClaim.onClick.AddListener(() =>
            {
                mBattlePassADActivity.DistributeReward(vipReward, true);
            });*/
        }
        // 针对头像特殊处理
        public void SetBtnOnClike()
        {
            
        }
        public void SetCanChangeUI(int lever, bool isAnmi =false)
		{
             
		}
        
    }
}
