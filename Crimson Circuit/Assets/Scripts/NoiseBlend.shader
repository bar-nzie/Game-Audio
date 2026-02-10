Shader "Custom/NoiseBlend"
{
    Properties
    {
        _TextureA ("Texture A", 2D) = "white" {}
        _TextureB ("Texture B", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _BlendStrength ("Blend Strength", Range(0,1)) = 0.001
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _TextureA;
        sampler2D _TextureB;
        sampler2D _NoiseTex;
        float _BlendStrength;

        struct Input
        {
            float2 uv_TextureA;
            float2 uv_TextureB;
            float2 uv_NoiseTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_TextureA;

            fixed4 texA = tex2D(_TextureA, uv);
            fixed4 texB = tex2D(_TextureB, uv);
            float noise = tex2D(_NoiseTex, uv).r;

            float blend = saturate(noise);

            fixed4 finalColor = lerp(texA, texB, blend);
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}