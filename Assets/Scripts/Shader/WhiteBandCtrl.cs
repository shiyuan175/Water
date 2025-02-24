using UnityEngine;
using UnityEngine.UI;

public class WhiteBandCtrl : MonoBehaviour
{
    public Material uiMaterial; // ����
    public float speed = 1.0f; // ����ƶ��ٶ�

    private void Update()
    {
        // ���¹��λ��
        float bandPosition = Mathf.Repeat(Time.time * speed, 1.0f); // ѭ����0��1
        uiMaterial.SetFloat("_BandPosition", bandPosition);
    }
}