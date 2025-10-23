using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;

namespace QFramework.Example
{
	public class testAcitionKitData : UIPanelData
	{
	}
	public partial class testAcitionKit : UIPanel
	{
		[SerializeField] Image img;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as testAcitionKitData ?? new testAcitionKitData();
			// please add init code here
		}
	
		protected override void OnOpen(IUIData uiData = null)
		{
			
		}
		
		 void OnEnable()
		{
			Debug.Log("Sb");
			ActionKit.Sequence()
				.Custom(a =>
                {
					A();
				})
				.Callback(C)
                .Start(this, _ => { Debug.Log("Sequence Finish:" + Time.time); });
        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		void A()
		{
			Debug.Log("A");
			Tween tween = img.DOFillAmount(1, 1)
				.OnComplete(() =>
				{
					Debug.Log("dsa");					
				});
		}
        void B()
        {
            Debug.Log("B");

        }
        void C()
        {
            Debug.Log("C");
        }
    }
}
