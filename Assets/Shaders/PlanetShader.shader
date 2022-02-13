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
        _Emission("Emission", Color) = (0,0,0,1)
        _Height("Height", Range(-1,1)) = 0
        _Seed("Seed", Range(0,10000)) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd noshadow vertex:vert
        #pragma target 3.0
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color: COLOR;
        };

        float _FirstThreshold;
        float _SecondThreshold;

        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        float4 _Emission;
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

        void vert(inout appdata_full v)
        {
            float height = noise(v.texcoord, 5) * 0.75 + noise(v.texcoord, 30) * 0.125 +
                noise(v.texcoord, 50) * 0.125;
            v.color.r = height + _Height;

            const float3 normal = frac(v.normal);
            
            const float layHeight = .01;
            if (height >= _SecondThreshold)
            {
                v.vertex.xyz += normal * layHeight;
            } else if (height < _FirstThreshold)
            {
                v.vertex.xyz -= normal * layHeight;
            }
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 color = tex2D(_MainTex, IN.uv_MainTex);;
            float height = IN.color.r;
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
            o.Albedo = color.rgb;
            o.Emission = _Emission.xyz;
            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}