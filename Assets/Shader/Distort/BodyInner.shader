Shader "AShader/DistortImage"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainScale("Scale of the main texture" , float ) = 1
		_DistortTexture("Distort Texture", 2D) = "white" {}
		_DistortScale("Distort Scale" , float ) = 1
		_Color("Main COlor" , Color) = (1,1,1,1)
		_ColorD("Dark Color" , color ) = (1,1,1,1)
		_Speed("Change speed" , vector ) = (1,1,1,1)
		_DisappearRange("Disappear Range" , float ) = 0.15
	}
	SubShader
	{
		Tags { "Queue" = "Geometry" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			 Tags { "LightMode" = "Always" } 

			ZWrite On
			ZTest LEqual
			Cull front
			Blend SrcAlpha OneMinusSrcAlpha  

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 posScreen : TEXCOORD1;
				float4 posWorld : TEXCOORD2;
				float3 normal : TEXCCOORD3;
//				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex; 
			float4 _MainTex_ST;
			float _MainScale;
			float4 _Color;
			float4 _ColorD;
			sampler2D _DistortTexture;
			float4 _DistortTexture_ST;
			float _DistortScale;
			float4 _Speed;
			float _DisappearRange;


			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.posScreen = o.vertex;
				o.posScreen.xy /= o.posScreen.w;
				o.posScreen.xy = 0.5*(o.posScreen.xy+1.0) * _ScreenParams.xy;
				o.posWorld = mul(unity_ObjectToWorld,v.vertex);
				o.normal = normalize( mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
//				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			} 
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				// sample the bump
				half2 bump = UnpackNormal(tex2D( _DistortTexture, i.posScreen.xy  )).rg;
				bump = UnpackNormal(tex2D( _DistortTexture, ( i.posWorld.xy  + sin(_Time.xx * _Speed.xy ) * bump.xy ) )).rg;
				
				// sample the texture
				fixed4 col_Range = tex2D(_MainTex , ( i.posScreen.xy + i.posWorld.yy) * _MainScale * 0.001 + _Time.xx * _Speed.zw + bump.xy * _DistortScale );

				fixed4 col = lerp( _ColorD , _Color , col_Range);

				// apply fog
//				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
