Shader "AShader/Floor"
{
	Properties
	{
		_Color("Color" , Color ) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
		_RefractionTex ("Refraction", 2D) = "white" { TexGen ObjectLinear }
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

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
				float4 proj : TEXCOORD2;
			};

			float4 _Color;
			sampler2D _MainTex;
			sampler2D _ReflectionTex;
			sampler2D _RefractionTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.proj = ComputeScreenPos( o.vertex );
				COMPUTE_EYEDEPTH(o.proj.z);
//				#if UNITY_UV_STARTS_AT_TOP
//				o.proj.y = (o.vertex.w - o.vertex.y) * 0.5;
//				#endif

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				half3 reflection = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.proj)).rgb;

				col.rgb *= reflection;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
//				return col;
				return fixed4( reflection , 1);
			}
			ENDCG
		}
	}
}
