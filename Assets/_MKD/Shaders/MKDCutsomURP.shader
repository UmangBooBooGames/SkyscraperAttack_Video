Shader "MKD/CustomURP"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _Hue("Hue (deg)", Range(-180,180)) = 0
        _Saturation("Saturation (%)", Range(0,200)) = 100
        _Lightness("Lightness (%)", Range(-100,100)) = 0
    }

    SubShader
    {
        Tags{ "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Forward"
            Tags{ "LightMode" = "UniversalForward" }

            Blend One Zero
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // Include URP core libraries only
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Declare texture and sampler
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float _Hue;
            float _Saturation;
            float _Lightness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
                return OUT;
            }

            float3 RGBtoHSL(float3 c) 
            {
                float M = max(c.r, max(c.g, c.b));
                float m = min(c.r, min(c.g, c.b));
                float C = M - m;

                float H = 0.0;
                if (C != 0.0)
                {
                    if (M == c.r) H = ((c.g - c.b) / C) % 6.0;
                    else if (M == c.g) H = ((c.b - c.r) / C) + 2.0;
                    else H = ((c.r - c.g) / C) + 4.0;
                }
                H = H / 6.0; 
                if (H < 0.0) H += 1.0;

                float L = (M + m) * 0.5;
                float S = (C == 0.0) ? 0.0 : C / (1.0 - abs(2.0*L - 1.0));
                return float3(H, S, L);
            }

            float hue2rgb(float p, float q, float t)
            {
                if (t < 0.0) t += 1.0;
                if (t > 1.0) t -= 1.0;
                if (t < 1.0/6.0) return p + (q - p)*6.0*t;
                if (t < 1.0/2.0) return q;
                if (t < 2.0/3.0) return p + (q - p)*(2.0/3.0 - t)*6.0;
                return p;
            }

            float3 HSLtoRGB(float3 hsl)
            {
                float H = hsl.x;
                float S = hsl.y;
                float L = hsl.z;

                if (S == 0.0) return float3(L,L,L);

                float q = (L < 0.5) ? (L*(1.0+S)) : (L+S - L*S);
                float p = 2.0*L - q;
                float r = hue2rgb(p,q,H + 1.0/3.0);
                float g = hue2rgb(p,q,H);
                float b = hue2rgb(p,q,H - 1.0/3.0);
                return float3(r,g,b);
            }

            float3 AdjustHSL(float3 colorRGB, float hueDeg, float saturationPCT, float lightnessPCT)
            {
                float hueShift = hueDeg / 360.0; 
                float sFactor = saturationPCT / 100.0; 
                float lShift = lightnessPCT / 100.0;

                float3 hsl = RGBtoHSL(colorRGB);
                hsl.x = frac(hsl.x + hueShift);
                hsl.y = clamp(hsl.y * sFactor, 0.0, 1.0);
                hsl.z = clamp(hsl.z + lShift, 0.0, 1.0);

                return HSLtoRGB(hsl);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float3 adjusted = AdjustHSL(col.rgb, _Hue, _Saturation, _Lightness);
                return float4(adjusted, col.a);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}