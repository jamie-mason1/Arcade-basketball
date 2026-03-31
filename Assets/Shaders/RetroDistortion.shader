Shader "Unlit/RetroDistortion"
{
    Properties
    {
        _Distortion ("Distortion Strength", Range(0,0.2)) = 0.05
        _ScanlineIntensity ("Scanline Intensity", Range(0,2)) = 0.5
        _PixelSize ("Pixel Size", Range(64,512)) = 240
        _RGBOffset ("RGB Offset", Range(0,0.01)) = 0.002
        _NoiseAmount ("Noise Amount", Range(0,1)) = 0.05
        _TimeScale ("Flicker Speed", Range(0,10)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }

        GrabPass { "_GrabTexture" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;

            float _Distortion;
            float _ScanlineIntensity;
            float _PixelSize;
            float _RGBOffset;
            float _NoiseAmount;
            float _TimeScale;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;

                // CRT distortion
                float2 centered = uv * 2.0 - 1.0;
                centered *= 1.0 + _Distortion * dot(centered, centered);
                uv = centered * 0.5 + 0.5;

                // Pixelation
                float2 pixelUV = floor(uv * _PixelSize) / _PixelSize;

                // RGB split
                float r = tex2D(_GrabTexture, pixelUV + float2(_RGBOffset, 0)).r;
                float g = tex2D(_GrabTexture, pixelUV).g;
                float b = tex2D(_GrabTexture, pixelUV - float2(_RGBOffset, 0)).b;

                float3 col = float3(r, g, b);

                // Scanlines
                float scan = sin(uv.y * _PixelSize * 3.1415);
                col *= 1.0 - _ScanlineIntensity * (0.5 + 0.5 * scan);

                // Noise
                float noise = rand(uv + _Time.y * _TimeScale);
                col += (noise - 0.5) * _NoiseAmount;

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}