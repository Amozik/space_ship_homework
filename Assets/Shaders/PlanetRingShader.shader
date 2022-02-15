Shader "Custom/PlanetRingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _InnerRadius ("InnerRadius", Float) = 1.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0;
                float4 vertex : SV_POSITION;
            };
            fixed4 _Color;
            float _InnerRadius;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                //light
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.diff.rgb += ShadeSH9(half4(worldNormal,1));
                o.diff += _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 position = float2((0.5 - i.uv.x) * 2, (0.5 - i.uv.y) * 2);
                const float ringDistanceFromCenter = sqrt(pow(position.x, 2) + pow(position.y, 2));
                clip(ringDistanceFromCenter - _InnerRadius * 2);
                clip(1 - ringDistanceFromCenter);
                
                fixed4 color = tex2D(_MainTex, i.uv) * _Color * i.diff;
                                
                return color;
            }
           
            ENDCG
        }
    }
    FallBack "Diffuse"
}
