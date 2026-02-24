using QFramework;

namespace Game.Water
{
    /// <summary>
    /// ͨ���༭����ӵİ�ť��չ�࣬���ڲ��Ű�ť�����Ч
    /// </summary>
    /// ͨ�������ȡ��̬��������
    /// ��ͬ��ť����ʹ�ò�ͬ����Ч
    public static class ButtonSoundExtension
    {
        public static void PlayButtonClickSound()
        {
            AudioKit.PlaySound("resources://Sound/WaterSound/BtnSound");
        }

        //������Ӹ������Ч����
        //public static void PlayButtonClickSound2()
        //{
        //    AudioKit.PlaySound($"resources://Audio/btnClick2");
        //}
    }
}
