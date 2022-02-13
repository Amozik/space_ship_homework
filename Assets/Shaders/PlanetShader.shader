Shader "Custom/PlanetShader"
{
    Properties
    {
        [HideInInspector] 
        _FirstThreshold ("FirstThreshhold", float) = 0.45
        [HideInInspector] 
        _SecondThreshold ("SecondThreshhold", float) = 0.75
        
        _Color1 ("Color1", Color) = (.1, .3, .5, 1)
        _Color2 ("Color2", Color) = (.1, .6, .3, 1)
        _Color3 ("Color3", Color) = (.6, .3, .3, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Height("Height", Range(-1,1)) = 0
        _Seed("Seed", Range(0,10000)) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COlOR;
            };

            float _FirstThreshold;
            float _SecondThreshold;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            float _Height;
            float _Seed;

            float hash(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float noise(float2 p, float size)
            {
                float result = 0;
                p *= size;
                float2 i = floor(p + _Seed);
                float2 f = frac(p + _Seed / 739);
                float2 e = float2(0, 1);
                float z0 = hash((i + e.xx) % size);
                float z1 = hash((i + e.yx) % size);
                float z2 = hash((i + e.xy) % size);
                float z3 = hash((i + e.yy) % size);
                float2 u = smoothstep(0, 1, f);
                result = lerp(z0, z1, u.x) + (z2 - z0) * u.y * (1.0 - u.x) + (z3 - z1) * u.x * u.y;
                return result;
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                float height = noise(v.texcoord, 5) * 0.75 + noise(v.texcoord, 30) * 0.125 +
                    noise(v.texcoord, 50) * 0.125;
                o.color.r = height + _Height;

                float3 normal = frac(v.normal);
                
                const float layHeight = .01;
                if (height >= _SecondThreshold)
                {
                    o.vertex.xyz += normal * layHeight;
                } else if (height < _FirstThreshold)
                {
                    o.vertex.xyz -= normal * layHeight;
                }
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                float height = i.color.r;
                if (height < _FirstThreshold)
                {
                    color = _Color1;
                }
                else if (height < _SecondThreshold)
                {
                    color = _Color2;
                }
                else
                {
                    color = _Color3;
                }
                
                return color;
            }
            ENDCG
        }
    }
}