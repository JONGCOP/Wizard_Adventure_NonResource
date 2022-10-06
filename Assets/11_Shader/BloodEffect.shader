// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BloodEffect"
{
	Properties
	{
		_MainTex ("Base", 2D) = "" {}
		_BlendTex ("Image", 2D) = "" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}

		_BlendAmount ("Blend Amount", Range(0,1)) = 0.5
		_EdgeSharpness ("Edge Sharpness", float) = 3
		_Distortion ("Distortion", Range(0,1)) = 0.5
	}
	Category
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Tags{ "RenderType" = "Tranparent" "IgnoreProjector" = "True" "Queue" = "Transparent"}
		ColorMask RGB
		LOD 200

		SubShader {
			Pass{
				CGPROGRAM

				#pragma fragmentoption ARB_precision_hint_fastest 
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ LINEAR_COLORSPACE
				#pragma multi_compile_instancing
				#pragma target 3.0

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;

					UNITY_VERTEX_INPUT_INSTANCE_ID //Insert	
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					half2 uv : TEXCOORD0;

					UNITY_VERTEX_OUTPUT_STEREO //Insert
				};

				UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
				UNITY_DECLARE_SCREENSPACE_TEXTURE(_BlendTex);
				sampler2D _BumpMap;

				float _BlendAmount;
				float _EdgeSharpness;
				float _Distortion;

				v2f vert(appdata_t v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v); //Insert
					UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert

					float4 blendColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BlendTex, i.uv);

					blendColor.a = blendColor.a + (_BlendAmount * 2 - 1);
					blendColor.a = saturate(blendColor.a * _EdgeSharpness - (_EdgeSharpness - 1) * 0.5);

					if (blendColor.a < 0.2f)
					{
						discard;
					}

					//Distortion:
					half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv)).rg;
					float4 mainColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv + bump * blendColor.a * _Distortion);

					#ifdef LINEAR_COLORSPACE
					mainColor = sqrt(mainColor);
					#endif

					float4 overlayColor = blendColor;
					overlayColor.rgb = mainColor.rgb * (blendColor.rgb + 0.5) * 1.2; //overlay

					blendColor = lerp(blendColor, overlayColor, 0.3);

					mainColor.rgb *= 1 - blendColor.a * 0.5; //inner-shadow border

					//return lerp(mainColor, blendColor, blendColor.a);
					mainColor = lerp(mainColor, blendColor, blendColor.a);
					mainColor.a = blendColor.a;

					#ifdef LINEAR_COLORSPACE
					mainColor *= mainColor;
					#endif
					return mainColor;
				}

				half4 frag2(v2f i) : COLOR //linear
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert

					float4 blendColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BlendTex, i.uv);
					blendColor.rgb *= 1.2;

					blendColor.a = blendColor.a + (_BlendAmount * 2 - 1);
					blendColor.a = saturate(blendColor.a * _EdgeSharpness - (_EdgeSharpness - 1) * 0.5);

					if (blendColor.a < 0.2f)
					{
						discard;
					}

					//Distortion:
					half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv)).rg;
					float4 mainColor = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv + bump * blendColor.a * _Distortion);

					float4 overlayColor = blendColor;
					overlayColor.rgb = mainColor.rgb * (blendColor.rgb + 0.05) * 2.64; //overlay

					blendColor = lerp(blendColor,overlayColor,0.3);

					mainColor.rgb *= 1 - blendColor.a;//*0.995; //inner-shadow border

					lerp(mainColor, blendColor, blendColor.a);
					mainColor.a = blendColor.a;

					return mainColor;
				}

				ENDCG
			}
		}

	}
} 