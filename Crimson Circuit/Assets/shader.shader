Shader "Custom/shader"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)

        _FresnelColor ("Fresnel Color", Color) = (0,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 2.0

        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _CrackColor ("Crack Glow", Color) = (1, 0.5, 0, 1)
        _CrackStrength ("Crack Intensity", Range(0, 2)) = 1.0

        _NoiseTiling ("Noise Tiling", Vector) = (1,1,0,0)
        _Alpha ("Base Transparency", Range(0,1)) = 1.0

        _HeightDarkenAmount ("Height Darken Amount", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        float4 _Color;
        float _Alpha;

        float4 _FresnelColor;
        float _FresnelPower;

        float4 _CrackColor;
        float _CrackStrength;

        float4 _NoiseTiling;

        float _HeightDarkenAmount;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Scale noise UVs
            float2 noiseUV = IN.uv_NoiseTex * _NoiseTiling.xy;

            // Sample noise texture
            float noise = tex2D(_NoiseTex, noiseUV).r;

            // Fresnel effect (opaque)
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);
            float3 fresnelEffect = fresnel * _FresnelColor.rgb;

            // Crack glow emission
            float3 crack = noise * _CrackColor.rgb * _CrackStrength;

            // Darken base color by height
            float darken = lerp(1.0, 1.0 - _HeightDarkenAmount, noise);
            float3 baseColorDarkened = baseTex.rgb * darken;

            // Apply transparency to base color only
            o.Albedo = baseColorDarkened;
            o.Alpha = _Alpha * baseTex.a;

            // Emission is fresnel + crack glow, fully visible regardless of transparency
            o.Emission = fresnelEffect + crack;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
