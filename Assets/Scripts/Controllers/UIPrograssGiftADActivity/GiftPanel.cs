using UnityEngine;
using QFramework;
using System.Collections.Generic;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
	public partial class GiftPanel : ViewController
	{
        [SerializeField]
        List<Transform> items;
        private void Awake()
        {
            Initialize();
        }
        private void OnEnable()
        {
         
        }

        private void Initialize()
        {
            if(items.Count ==0)
            {
                Transform _transform = transform.Find("item").Find("GiftItem");
                for (int i =0;i< _transform.childCount;i++)
                    items.Add(_transform.GetChild(i));
            }


        }

        private void SetBtn()
        {

        }
      
	}
}
