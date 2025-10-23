// Generate Id:9a0faef9-657f-4d8d-9d68-3b8b22f62310
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class PotionActivityNode : QFramework.IController
	{
		public UnityEngine.UI.Image ImgProgressBar;
		
		public TMPro.TextMeshProUGUI TextProgress;
		
		public UnityEngine.RectTransform Selected;
		
		public TMPro.TextMeshProUGUI TxtCurLevel;
		
		public UnityEngine.UI.Image ImgRewardIcon;
		
		public TMPro.TextMeshProUGUI TextTimer;
		
		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>GameMainArc.Interface;
	}
}
