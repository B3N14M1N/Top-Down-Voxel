Shader "Universal Render Pipeline/Custom/VertexLit"
{
    Properties
    {
        
		_MainTex ("Texture", 2D) = "white" {}
		 _GradientLUT ("Gradient LUT", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
        [Toggle] _RemoveDiag("Remove diagonals", Float) = 0.
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            
            Name "StandardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma glsl
			#pragma vertex vert
			#pragma geometry geom
			
			#pragma fragment frag
			//#pragma multi_compile __ _REMOVEDIAG_ON
			
            //#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			
            //#include "UnityLightingCommon.cginc" 
            //#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			// for _LightColor0
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float3 normal     : NORMAL;
                float4 tangent    : TANGENT;
			};

			struct v2g
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : POSITION1;
                float4 color : COLOR;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 bary : TEXCOORD1;
				float3 normal : POSITION1;
                float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Gain;
			float4 _Color;
			float4 _EdgeColor;
			
			static const float3 MyNormals[6]  = 
			{
				float3(0.0, 0.0, -1.0),
				float3(1.0, 0.0, 0.0),
				float3(0.0, 0.0, 1.0),
				float3(-1.0, 0.0, 0.0),
				float3(0.0, 1.0, 0.0),
				float3(0.0, -1.0, 0.0)
			};

			static const float2 MyUVs[4] = 
			{
				float2(0.0, 0.0),
				float2(1.0, 0.0),
				float2(1.0, 1.0),
				float2(0.0, 1.0)
			};

			v2g vert (appdata v)
			{
				v2g o;
				
				int data = asint(v.vertex.x);
				float4 position = float4(data & 0xff, (data >> 8) & 0xff,(data >> 16) & 0xff, 1.0);
				float3 normal = float3(MyNormals[floor(asint(v.vertex.z) & 0xff)]);
				float2 uv = float2(MyUVs[(asint(v.vertex.z) >> 8) & 0xff]);
				float4 color = float4(float(v.vertex.y), 0.0, 0.0, 1.0);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(position);
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(normal, v.tangent);
                
				o.vertex = vertexInput.positionCS;
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				o.color = color;
				o.normal = vertexNormalInput.normalWS.xyz;
				return o;
			}
			
			[maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
                float3 param = float3(0.0, 0.0, 0.0);
 

                g2f o;
                o.vertex = IN[0].vertex;
                o.normal = IN[0].normal;
				o.uv = IN[0].uv;
				o.color = IN[0].color;
                o.bary = float3(1.0, 0.0, 0.0) + param;
                triStream.Append(o);

                o.vertex = IN[1].vertex;
                o.normal = IN[1].normal;
                o.uv = IN[1].uv;
				o.color = IN[1].color;
				o.bary = float3(0.0, 0.0, 1.0) + param;
                triStream.Append(o);
                
				o.vertex = IN[2].vertex;
				o.normal = IN[2].normal;
                o.uv = IN[2].uv;
				o.color = IN[2].color;
				o.bary = float3(0.0, 1.0, 0.0) + param;
                triStream.Append(o);
            }
			
			
			float edgeFactor(float3 bary)
			{
				float3 d = fwidth(bary);
				float3 a3 = smoothstep(float3(0, 0, 0), _Gain * d, bary);
				return min(min(a3.x, a3.y), a3.z);
			}

			
			half4 frag (g2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv) * i.color;// * _Color;
				return col;
				//float t = edgeFactor(i.bary);

                //Light mainLight = GetMainLight();
                
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                //half nl = max(0.3, dot(i.normal, mainLight.direction));
                
                // factor in the light color
                //half4 diff = nl * half4(mainLight.color, 1);
                
                //return col * diff;
				//return lerp(col, _EdgeColor, t) * diff;
			}

            ENDHLSL
        }

    }

}