Shader "Custom/ChessPieceOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "OUTLINE"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                // حرکت نورمال‌ها به سمت بیرون برای ایجاد حاشیه
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex + float4(v.normal * _OutlineWidth, 0));
                o.color = _OutlineColor;
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return i.color;
            }
            ENDCG
        }

        // Render the original model without outline
        Pass
        {
            Name "BASE"
            Tags { "LightMode"="ForwardBase" }
            Cull Back
            ZWrite On
            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf (Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
    }
}
