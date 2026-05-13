Shader "UI/Waveform"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _DarkAlpha ("Dark Alpha", Range(0,1)) = 0.4
        _Thickness ("Thickness", Range(0,0.5)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float _DarkAlpha;
            float _Thickness;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float alpha = 0;
                
                // VERTICAL NEIGHBORS (Split View)
                if (uv.y > 0.5)
                {
                    // TOP HALF: LEFT CHANNEL (Row 0)
                    float4 data = tex2D(_MainTex, float2(uv.x, 0.25));
                    float minVal = (data.r * 2.0 - 1.0) * 1.07 - _Thickness;
                    float maxVal = (data.g * 2.0 - 1.0) * 1.07 + _Thickness;
                    float rmsVal = data.b * 1.07;
                    
                    float yLocal = (uv.y - 0.5) * 4.0 - 1.0; // Remap 0.5..1.0 to -1..1
                    alpha = (yLocal >= minVal && yLocal <= maxVal) ? ((abs(yLocal) < rmsVal) ? 0.8 : _DarkAlpha) : 0;
                }
                else
                {
                    // BOTTOM HALF: RIGHT CHANNEL (Row 1)
                    float4 data = tex2D(_MainTex, float2(uv.x, 0.75));
                    float minVal = (data.r * 2.0 - 1.0) * 1.07 - _Thickness;
                    float maxVal = (data.g * 2.0 - 1.0) * 1.07 + _Thickness;
                    float rmsVal = data.b * 1.07;
                    
                    float yLocal = uv.y * 4.0 - 1.0; // Remap 0.0..0.5 to -1..1
                    alpha = (yLocal >= minVal && yLocal <= maxVal) ? ((abs(yLocal) < rmsVal) ? 0.8 : _DarkAlpha) : 0;
                }

                fixed4 color = IN.color;
                color.a *= alpha;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                return color;
            }
            ENDCG
        }
    }
}
