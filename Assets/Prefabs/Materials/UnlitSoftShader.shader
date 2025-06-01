Shader "Custom/UnlitSoftTrail"
{
    Properties
    {
        _Color ("Trail Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Softness ("Softness", Range(0,5)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Suavidade adicional com base na UV e _Softness
                float edgeFade = smoothstep(0.0, _Softness, i.uv.y) * smoothstep(0.0, _Softness, 1.0 - i.uv.y);
                col.a *= edgeFade;

                return col;
            }
            ENDCG
        }
    }
}
