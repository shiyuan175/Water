// Generate Id:7799010a-0de1-4775-b555-0b1ce0f01c05
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class SettingPanelCtrl : QFramework.IController
	{

		public UnityEngine.UI.Button BtnSelect;

		public UnityEngine.UI.Image ImgSelected;

		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>GameMainArc.Interface;
	}
}
