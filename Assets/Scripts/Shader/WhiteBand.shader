Shader "UI/WhiteBandWithColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // UI����
        _BandPosition ("Band Position", Range(0, 10)) = 0 // ���λ��
        _BandWidth ("Band Width", Range(0, 1)) = 0.1 // ������
        _BandColor ("Band Color", Color) = (1,1,1,1) // �����ɫ
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // ����Image����ɫ
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR; // ����Image����ɫ��Ƭ����ɫ��
            };

            sampler2D _MainTex;
            float _BandPosition;
            float _BandWidth;
            fixed4 _BandColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // ����Image����ɫ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); // ԭʼ������ɫ

                 // ����б��Ĺ�����򣬻��� i.uv.x �� i.uv.y
                float angle = 45.0; // б��Ƕȣ����� 45 ��
                float slope = tan(radians(angle)); // ���ݽǶȼ���б��
                float uv_value = i.uv.x + slope * i.uv.y; // б��� UV ����ֵ
                // float band = smoothstep(_BandPosition - _BandWidth, _BandPosition, i.uv.x) - 
                //              smoothstep(_BandPosition, _BandPosition + _BandWidth, i.uv.x); // �������
                float band = smoothstep(_BandPosition - _BandWidth, _BandPosition, uv_value) - 
                             smoothstep(_BandPosition, _BandPosition + _BandWidth, uv_value); // �������
                col.rgb = lerp(i.color, _BandColor.rgb, band); // ��Ϲ����ɫ
                return col;
            }
            ENDCG
        }
    }
}