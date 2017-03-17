// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//created by colin leung 2015-4-19 - c4cat entertainment limited.
//This shader is free for personal use.

//This shader can add a planar projection shadow to your object.

//but it only works on y=0 plane, and the whole object must above the y=0 plane
//*a directional light is needed

Shader "C4Cat/SimplePlanarShadow(Shadow Only)" {
	Properties {
      _MainTex ("Texture Image", 2D) = "grey" {} 
      _GlobalDynamicShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,1) //this line will allow you to control shadow color in material inspector
      _GroundY("Ground Y",float) = 0
   }
   SubShader {
   	
   		//Your original custom shader here inside this pass, do your object's original shading in this pass (first pass)
   		//In this example, I assume your original shading is just a basic unlit texture.
//	      Pass {	
//	         CGPROGRAM
//	 
//	         #pragma vertex vert  
//	         #pragma fragment frag 
//	 
//	         uniform sampler2D _MainTex;	
//	 
//	         struct vertexInput {
//	            float4 vertex : POSITION;
//	            float4 texcoord : TEXCOORD0;
//	         };
//	         struct vertexOutput {
//	            float4 pos : SV_POSITION;
//	            float4 tex : TEXCOORD0;
//	         };
//	 
//	         vertexOutput vert(vertexInput input) 
//	         {
//	            vertexOutput output;
//	 
//	            output.tex = input.texcoord;
//	            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
//	            return output;
//	         }
//	         float4 frag(vertexOutput input) : COLOR
//	         {
//	            return tex2D(_MainTex, input.tex.xy);		 
//	         }
//	 
//	         ENDCG
//	      }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    
	   //Planar Shadow  Pass (Projection transparent Mesh)
	   //This additional pass will render a planar shadow on y=0 plane (second pass).
	   
		Pass
		{
			Tags
            {
                //"RenderType"="Transparent"
                //"Queue" = "Transparent-10"  
				//"LightMode"="ForwardBase"
				//"IgnoreProjector"="True"      
            }
               
            ZTest on             
            Blend OneMinusSrcAlpha SrcAlpha 
            Cull off          
                          
         	//use Stencil Buffer to prevent multi Blending
         	//you can try to command out this Stencil code,you should see some part of the shadow are render twice if you object is not convex.             
            Stencil
             {
                Ref 1
                Comp NotEqual
                Pass Replace
                Fail Keep
                ZFail Keep
                ReadMask 1
                WriteMask 1
            }           
             
            CGPROGRAM
 			
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
			
			//You can edit this global shadow color in C#.
			//Shader.SetGlobalColor("_GlobalDynamicShadowColor",GlobalDynamicShadowColor);
            uniform fixed4 _GlobalDynamicShadowColor;
	         uniform float _GroundY;
            
            // Structure from vertex shader to fragment shader
			struct v2f
			{
				half4 Pos : SV_POSITION;
				fixed4 Color : COLOR0;
			};
             
            //in vertex shader project the model onto y=0 ground according to directional light direction 
            v2f vert(appdata_base v)
            {
            	v2f o;
				//convert to world space for calculation
                float4 positionInWorldSpace = mul(unity_ObjectToWorld, v.vertex);
                float3 lightDirectionInWorldSpace = normalize(_WorldSpaceLightPos0.xyz);
                float distanceToOrigin = length(v.vertex);
                
                //project the world space vertex to the y=0 ground
                positionInWorldSpace.x -= positionInWorldSpace.y/lightDirectionInWorldSpace.y*lightDirectionInWorldSpace.x;
                positionInWorldSpace.z -= positionInWorldSpace.y/lightDirectionInWorldSpace.y*lightDirectionInWorldSpace.z;
                
                float originalWorldY = positionInWorldSpace.y; //save a copy of original world space Y (height),use to calculate shadow fading
                positionInWorldSpace.y = _GroundY;//force the vertex's world space Y = 0 (on the ground)
                
                float4 result = mul (UNITY_MATRIX_VP, positionInWorldSpace); //complete to MVP matrix transform (we already change from local->world before, so this line only do VP)
                result.z-=0.0001; //push depth towards camera,above ground a bit. prevent Z-fighting to ground
                
                //pack up for output to fragment shader(pos & color)
                o.Pos = result; //screen space Pos
                o.Color = _GlobalDynamicShadowColor; //the color we pick in material
                 
                //the higher the original vertex, the more transparent it will be.
                //you can edit this line,changing the number...remove it...
//                o.Color.a *= originalWorldY*0.15 + 0.45; 
				 o.Color.a *= 0.8;
                
                
                return o;
            }
 			
 			//simple outputing blending color from color calculated in vertex shader (above)
            fixed4 frag(v2f i) : COLOR {                
                return i.Color;
            }
 
            ENDCG
		}
   }
}