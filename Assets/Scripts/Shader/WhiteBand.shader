Shader "UI/WhiteBandWithColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // UI����
        _BandPosition ("Band Position", Range(0, 1)) = 0 // ���λ��
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
        ZTest Always
          Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); //Ӧ����������ƫ��
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. ��������Ӧ�ö�����ɫ
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * i.color; //������ɫ��Image�����ɫ���

                // 2. ����б��������
                float angle = 45.0;
                float slope = tan(radians(angle));
                float uv_value = i.uv.x + slope * i.uv.y;
                float band = smoothstep(_BandPosition - _BandWidth, _BandPosition, uv_value) - 
                             smoothstep(_BandPosition, _BandPosition + _BandWidth, uv_value);

                // 3. ���ӹ����ɫ������͸���ȣ�
                finalColor.rgb = lerp(finalColor.rgb, _BandColor.rgb, band * _BandColor.a);
                finalColor.a = texColor.a * i.color.a; // ����͸����

                return finalColor;
            }
            ENDCG
        }
    }
}