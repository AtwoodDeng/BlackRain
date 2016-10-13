// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "AShader/LPWater"
{
	Properties 
	{
		[Header(Default Settings)] [Space]
		_Color("Color", Color) = (1,0,0,1)
		_SpecularColor("Specular Material Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Float) = 1.0
		_ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
		_RefractionTex ("Refraction" , 2D) = "white" {TexGen ObjectLinear }
		_InvRanges ("Inverse Alpha, Depth and Color ranges", Vector) = (1.0, 0.17, 0.17, 0.0)
		_Distrubance( "Distrubance (Refl, Refr)" , Vector) = (0.5 , 5.0 , 0 , 0 )
		_AffectByLight ( "Affet By Light " , float )  = 1
		_ReflectionAlpha( "Reflection Alpha" , float ) = 1
		_RefractionAlpha( "Refraction Alpha" , float ) = 1

        [Space][Header(Wave Settings)] [Space]
		// These parameters define the wave for the entire water mesh
		_GerstnerIntensity("Per vertex displacement", Float) = 1.0
		_GAmplitude ("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
		_GFrequency ("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSteepness ("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)
		_WaveAmplitude ("Amplitude", Float) = 1.0
//		_WaveHeight ("Height", Float) = 1.0
		_WaveFrequency ("Frequency", Float) = 1.0
		_WaveSteepness ("Steepness" , Float) = 1.0
		_WaveSpeed ("Speed" , Float ) = 1.0

		// These parameters define the wave for individual water mesh
		_InAmplitude ( "Individual Offset Amplitude" , Vector ) = ( 0 , 0.2 , 0 , 0 )
		_InSpeed ( "Individual Offset Speed" , Vector ) = ( 5 , 5 , 5 , 0 )

        [Space][Space][Header(Foam Settings)] [Space]
        _FoamTex ("Foam Texture", 2D) = "white" {}
        _FoamBumpTiling ("Foam Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
		_FoamBumpDirection ("Foam movement", Vector) = (1.0 ,1.0, -1.0, 1.0) 
		_FoamIntense("Foam Intense" , float) = 0.1
		_FoamCutoff("Foam Cutoff" , float ) = 0.35
		_InvFadeParemeter ("Auto blend parameter (Edge, Shore, Distance scale)", Vector) = (0.2 ,0.39, 0.5, 1.0)
		_FoamAlpha("Foam Alpha" , float ) = 1
		_FoamWidth("Foam Width" , float ) = 1


        [Space][Header(Shadow Settings)] [Space]
		// Shadow
		_ShadowStrength ("the strength of the shadow" , float) = 0.5

		[Space][Header(For Test)] [Space]

		[Toggle] _ShowDiffuse("Show Diffuse", Float) = 1.0
		[Toggle] _ShowSpecular("Show Specular", Float) = 1.0
		[Toggle] _ShowReflection("Show Reflection", Float) = 1.0
		[Toggle] _ShowRefraction("Show Refraction", Float) = 1.0
		[Toggle] _ShowFoam("Show Foam", Float) = 1.0
		[Toggle] _ShowDepth("Show Depth", Float) = 1.0
	}

	// Refraction & Reflection
	SubShader 
	{
	
//	    Tags { "Queue"="Geometry+400" "IgnoreProjector"="True" "RenderType"="Opaque" "DisableBatching"="True" }
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
	    LOD 400
		ZTest LEqual 
		ZWrite On
		Cull Off


    	Pass 
    	{
			Name "BASE"
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#pragma only_renderers d3d11			
			#pragma target      5.0

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight


			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase

			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			
			#define UNITY_PASS_FORWARDBASE
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "WaterInclude.cginc"  
			
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz ,float3(12.9898,78.233,45.5432))) * 43758.5453);
			}

			float rand2(float3 co)
			{
				return frac(sin(dot(co.xyz ,float3(19.9128,75.2,34.5122))) * 12765.5213);
			}

			// default 
			 float4 _Color;
			 float4 _SpecularColor;
			 float _Shininess;
//			uniform float _DColor;

			sampler2D _ReflectionTex;
			sampler2D _RefractionTex;
			sampler2D _CameraDepthTexture;
//			half _ReflectionTint;
			half4 _InvRanges;
			half4 _Distrubance;
			float _AffectByLight;
			float _ReflectionAlpha;
			float _RefractionAlpha;

			// wave 
			uniform float4 _GAmplitude;
			uniform float4 _GFrequency;
			uniform float4 _GSteepness;
			uniform float4 _GSpeed;
			uniform float4 _GDirectionAB;
			uniform float4 _GDirectionCD;

			float _WaveAmplitude;
//			float _WaveHeight;
			float _WaveFrequency;
			float _WaveSteepness;
			float _WaveSpeed;

			uniform float4 _InAmplitude;
			uniform float4 _InSpeed;

			// foam 
            uniform sampler2D _FoamTex; uniform float4 _FoamTex_ST;
            uniform float4 _FoamBumpTiling;
			uniform float4 _FoamBumpDirection;
			uniform float _FoamIntense;
			uniform float _FoamCutoff;
			uniform float4 _InvFadeParemeter;
			float _FoamAlpha;
			float _FoamWidth;

            // shadow
			uniform float _ShadowStrength;

			// test
			uniform float _ShowDiffuse;
			uniform float  _ShowSpecular;
			uniform float  _ShowReflection;
			uniform float  _ShowRefraction;
			uniform float  _ShowFoam;
			uniform float _ShowDepth;

			struct v2g 
			{
    			float4  pos      : SV_POSITION;
				float3	norm     : NORMAL;
     			float2  uv       : TEXCOORD0;
				float3  worldPos : TEXCOORD1;	// Used to calculate the texture UVs and world view vector
     			float4  proj0    : TEXCOORD2;   // Used for depth and reflection textures
     			float4  proj1    : TEXCOORD3;
			};
			
			struct g2f 
			{
    			float4  pos : SV_POSITION;
				float3  norm : NORMAL;
    			float2  uv : TEXCOORD0; 
    			float3  worldPos : TEXCOORD1;
    			float4  proj0 : TEXCOORD2;
    			float4  proj1 : TEXCOORD3;        
				float3  center : TEXCOORD4;
				LIGHTING_COORDS(5,6)
				UNITY_FOG_COORDS(7)
			};

			v2g vert(appdata_full v)
			{

				// record vertex
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				// wave
				half3 vtxForAni = (worldPos).xzz;
				half3 nrml;
				half3 offsets;

				Gerstner (
					offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
					_GAmplitude * _WaveAmplitude ,												// amplitude
					_GFrequency * _WaveFrequency ,												// frequency
					_GSteepness * _WaveSteepness ,												// steepness
					_GSpeed * _WaveSpeed ,													// speed
					_GDirectionAB,												// direction # 1, 2
					_GDirectionCD												// direction # 3, 4
				);   

				// offset for each vertex

				offsets.xyz += sin( ( _Time.xxx *  + rand( worldPos.x + worldPos.z ) ) * _InSpeed.xyz ) * _InAmplitude.xyz;

				v.vertex.xyz += offsets;
				worldPos =  mul(unity_ObjectToWorld, v.vertex).xyz;

				// eye depth
				float4 pos = mul(UNITY_MATRIX_MVP, v.vertex);
				float4 proj0 = ComputeScreenPos( pos );
				COMPUTE_EYEDEPTH(proj0.z);

				float4 proj1 = proj0;
				#if UNITY_UV_STARTS_AT_TOP
				proj1.y = (pos.w - pos.y) * 0.5;
				#endif
//				COMPUTE_EYEDEPTH(proj1.z);

    			v2g OUT;
				OUT.pos = pos;
				OUT.worldPos = worldPos;
				OUT.proj0 = proj0;
				OUT.proj1 = proj1;
				OUT.norm = v.normal;
    			OUT.uv = v.texcoord;
    			return OUT;
			}
			
			[maxvertexcount(4)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
			{
				float3 v0 = IN[0].worldPos.xyz;
				float3 v1 = IN[1].worldPos.xyz;
				float3 v2 = IN[2].worldPos.xyz;

				float3 centerPos = (v0 + v1 + v2) / 3.0;

				float3 vn = normalize(cross(v1 - v0, v2 - v0));

				g2f OUT;
				OUT.pos = IN[0].pos;
				OUT.norm = vn;
				OUT.center = centerPos;
				OUT.uv = IN[0].uv;
				OUT.worldPos = IN[0].worldPos;
				OUT.proj0 = IN[0].proj0;
				OUT.proj1 = IN[0].proj1;
				TRANSFER_GEOM_TO_FRAGMENT(OUT, OUT.pos); 
				UNITY_TRANSFER_FOG(OUT,OUT.pos);
				triStream.Append(OUT);

				OUT.pos = IN[1].pos;
				OUT.norm = vn;
				OUT.center = centerPos;
				OUT.uv = IN[1].uv;  
				OUT.worldPos = IN[1].worldPos;
				OUT.proj0 = IN[1].proj0;
				OUT.proj1 = IN[1].proj1;
				TRANSFER_GEOM_TO_FRAGMENT(OUT, OUT.pos); 
				UNITY_TRANSFER_FOG(OUT,OUT.pos);
				triStream.Append(OUT);

				OUT.pos =  IN[2].pos;
				OUT.norm = vn;
				OUT.center = centerPos;
				OUT.uv = IN[2].uv;
				OUT.worldPos = IN[2].worldPos;
				OUT.proj0 = IN[2].proj0;
				OUT.proj1 = IN[2].proj1;
				TRANSFER_GEOM_TO_FRAGMENT(OUT, OUT.pos); 
				UNITY_TRANSFER_FOG(OUT,OUT.pos);
				triStream.Append(OUT);
				
			}


			
			half4 frag(g2f IN) : COLOR
			{
				// Diffuse Color & Specular color
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(
					mul(float4(- IN.norm, 0.0), modelMatrixInverse).xyz);
//				float3 viewDirection = normalize(_WorldSpaceCameraPos
//					- mul(modelMatrix, float4(IN.center, 0.0)).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos
					- IN.center);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				if (dot(normalDirection, lightDirection) < 0.0)
					normalDirection = - normalDirection;

				// get the light color
				float4 LightColor = lerp( float4(1,1,1,1) ,  _LightColor0 , _AffectByLight);

				// get the diffuse reflection
				float3 diffuseReflection =
					LightColor.rgb * _Color.rgb
					* max(0.0, dot(normalDirection, lightDirection));


				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = fixed4( 0 , 0 , 0 , 0);
				}
				else
				{
					specularReflection =  LightColor.rgb *
						_SpecularColor.rgb * pow(max(0.0, - dot(
							reflect(lightDirection, normalDirection),
							viewDirection)), _Shininess);
				}
				specularReflection = ( _ShowSpecular == 1 ) ? specularReflection : float3(0,0,0);

				// the vector from camera to the vertex
				half3 worldView = (IN.worldPos - _WorldSpaceCameraPos);

				// Calculate the depth difference at the current pixel
				float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.proj1)).r;
				depth = LinearEyeDepth(depth);
				depth -= IN.proj1.z;
				if ( _ShowDepth == 1 )
				{
					depth = depth / 10;
					return fixed4(depth,depth,depth,1);
				}
				// Calculate the normal & world normal
				half3 worldNormal = IN.norm.xyz;
				worldNormal.z = -worldNormal.z;

				// Calculate the depth ranges (X = Alpha, Y = Color Depth)
				half3 ranges = saturate(_InvRanges.xyz * depth);
				ranges.x = _InvRanges.x;
				ranges.y = 1.0 - ranges.y;
				ranges.y = lerp(ranges.y, ranges.y * ranges.y * ranges.y, 0.5);

				// Calculate the color tint
				half4 col;
				col.rgb = (_ShowDiffuse == 1 ) ? diffuseReflection : _Color.rgb;

				// Dot product for fresnel effect
				half fresnel = Fresnel( - viewDirection , IN.norm , 0.2 , 5 );
//				half fresnel =  0.5 + ( 1 - 0.5 ) * pow( 1 - saturate(dot (viewDirection, IN.norm)) , 5 );
				  
				// reflection uses the dynamic reflection texture
				IN.proj0.xy += IN.norm.xy * _Distrubance.x;
				half3 reflection = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(IN.proj0)).rgb;
				reflection = lerp( col.rgb, reflection *  col.rgb, _ReflectionAlpha);
				IN.proj0.xy -= IN.norm.xy * _Distrubance.x;
				reflection.rgb = (_ShowReflection == 1) ? reflection : half3(0,0,0);


				// High-quality refraction uses the grab pass texture with grab texture
//				IN.proj1.xy += IN.norm.xy * _GrabTexture_TexelSize.xy * ( _Distrubance.y * IN.proj1.z );
//				half3 refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.proj1)).rgb;

				// refraction uses the refraction texture from the refraction texture with grab texture
//				IN.proj1.xy += IN.norm.xy * ( _Distrubance.y * IN.proj1.z );
				half3 refraction = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(IN.proj1)).rgb;

				refraction.rgb = lerp( refraction * col.rgb , refraction.rgb , ranges.y );
				refraction.rgb *= ranges.y * ranges.x; 
				refraction.rgb = lerp( col.rgb , refraction , _RefractionAlpha);
				refraction.rgb = (_ShowRefraction == 1) ? refraction : half3(0,0,0);

				// Color the refraction based on depth
//				refraction = lerp(refraction, refraction * col.rgb * ranges.x, ranges.z);
//				refraction = lerp( lerp(col.rgb, col.rgb * refraction, ranges.y), refraction, ranges.y) * ranges.x;

				// foam color	
				half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
//				half depthF = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.proj0));
//					depthF = LinearEyeDepth(depthF);
				edgeBlendFactors = saturate(_InvFadeParemeter * (depth / _FoamWidth));
				edgeBlendFactors.y = 1.0-edgeBlendFactors.y; // distance

				half2 tileableUv = IN.worldPos.xz;
				half4 coords = (tileableUv.xyxy + _Time.xxxx * _FoamBumpDirection.xyzw) * _FoamBumpTiling.xyzw;
				half4 foam = tex2D(_FoamTex, coords.xy)  * tex2D(_FoamTex,coords.zw);
				foam.rgb = foam.rgb * _FoamIntense * (edgeBlendFactors.y + saturate(_FoamCutoff));
				foam.rgb += ( 1.0-edgeBlendFactors.x ) * 0.5;
				foam.rgb = (_ShowFoam == 1) ? foam : half3(0,0,0);
				foam.rgb *= _FoamAlpha;

				// add all color together
				float4 outColor;
				outColor.rgb = lerp(refraction, reflection, fresnel) + specularReflection + foam;
				UNITY_APPLY_FOG(IN.fogCoord, outColor);
//				UNITY_OPAQUE_ALPHA(outColor.a);
				outColor.a = 1;
				 
				return outColor;
			}
			
			ENDCG

    	}



		// SHADOW CASTER PASS	
		// Pass to render object as a shadow caster
//		Pass 
//		{
//			Name "ShadowCaster"
//			Tags { "LightMode" = "ShadowCaster" }
//			
//			Fog {Mode Off}
//			ZWrite On 
//			ZTest LEqual 
////			Cull Off  
//			 Cull Off
//			 Offset 1, 1
////			Offset 2, 1 // Prevents bias shadowing errors that shadow bias cannot fix - not sure why this happens, would zTest Less be better? or is that slower than LEqual?
//
//			CGPROGRAM
//			#pragma target      5.0
//			
//			#pragma vertex 		vert
//			#pragma geometry    geom
//			#pragma fragment 	frag
//			
//			#pragma multi_compile_shadowcaster
//			#pragma only_renderers d3d11
//			
//			#include "UnityCG.cginc"
//			#include "HLSLSupport.cginc"
//			
//			float       _Explode;
//
//			struct v2f 
//			{ 
//				V2F_SHADOW_CASTER;
//			};
//			
//			struct v2g 
//			{ 
//				float4 pos : SV_POSITION;
//			};
//			  
//
//			v2g vert( appdata_base v )
//			{
//				v2g o;
//				o.pos = v.vertex;
//				return o;
//			}	
//	
//			[maxvertexcount(12)]
//			void geom( triangle v2g input[3], inout TriangleStream<v2f> outStream )
//			{
//				v2f output;
//
//
//				for( int looper=0; looper<3; looper++ )
//				{
//					float clamped;
//					TRANSFER_GEOM_SHADOW_CASTER(output, input[looper].pos)		
//				}
//			}
//			     
//			
//			float4 frag( v2f i ) : COLOR
//			{
//				SHADOW_CASTER_FRAGMENT(i)
//			}
//			ENDCG
//		}
		// END SHADOW CASTER PASS	
	

	
//		// SHADOW COLLECTOR PASS
		// Pass to render object as a shadow collector
//		Pass 
//		{
//			Name "ShadowCollector"
//			Tags { "LightMode" = "ShadowCollector" }
//			
//			Fog {Mode Off}
//			ZWrite On ZTest LEqual
//			// Offset 2, 1
//
//			CGPROGRAM
//			#pragma target      5.0
//			
//			#pragma vertex 		vert
//			#pragma geometry    geom
//			#pragma fragment 	frag
//			
//			#pragma multi_compile_shadowcollector 
//			#pragma only_renderers d3d11
//			
//			#define SHADOW_COLLECTOR_PASS
//			
//			#include "UnityCG.cginc"
//			#include "HLSLSupport.cginc"
//			
//			float       _Explode;
//			
//			struct appdata 
//			{
//				float4 vertex : POSITION;
//			};
//
//			struct v2f 
//			{
//				V2F_SHADOW_COLLECTOR;
//			};
//			
//			struct v2g
//			{
//				float4 pos : SV_POSITION;
//			};
//			
//			
//			v2g vert (appdata v)
//			{
//				v2g o;
//				o.pos = v.vertex;
//				return o;
//			}
//
//			[maxvertexcount(12)]
//			void geom( triangle v2g input[3], inout TriangleStream<v2f> outStream )
//			{
//				v2f output;
//				for( int looper=0; looper<3; looper++ )
//				{
//					float4 wpos;
//					TRANSFER_GEOM_SHADOW_COLLECTOR(output, input[looper].pos)		
//				}
//
//			}
//			
//			fixed4 frag (v2f i) : COLOR
//			{
//				SHADOW_COLLECTOR_FRAGMENT(i)
//			}
//			ENDCG
//		}
//		// END SHADOW COLLECTOR

	}

}