
// -----------------------------------------------------------------------------
// This source file has been developed within the scope of the
// Technical Director course at Filmakademie Baden-Wuerttemberg.
// http://technicaldirector.de
//
// Written by Daniel Schmucker
// Copyright (c) 2020 Animationsinstitut of Filmakademie Baden-Wuerttemberg
// -----------------------------------------------------------------------------

Shader "Unlit/HDRP_Cryptomatte_preview"
{
    Properties
    {
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog


            #include "UnityCG.cginc"

			UNITY_INSTANCING_BUFFER_START(PerInstance)
				UNITY_DEFINE_INSTANCED_PROP(float4, _ColorCrypto) // CRYPTOMATTE PREVIEW COLOR INSTANCED PROPERTY
			UNITY_INSTANCING_BUFFER_END(PerInstance)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 linearToSRGB(float4 c)
            {
                return pow(c, 1.0 / 1.8);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                //CRYPTOMATTE INSTANCED PROPERTIES
                float4 crypto_preview = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _ColorCrypto);
                float4 color;
                color = linearToSRGB(crypto_preview); // changing color values to make it better match with nuke cryptomatte preview

			    return color;

            }
            ENDCG
        }
    }
}
