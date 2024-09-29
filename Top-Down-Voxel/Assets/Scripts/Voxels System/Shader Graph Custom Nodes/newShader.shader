Shader "Custom/LambertLighting"
{
    Properties
    {
        
		_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
			sampler2D _MainTex;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv: TEXCOORD0;
            };
            struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float3 normal     : NORMAL;
                float4 tangent    : TANGENT;
			};

            struct Varyings
            {
				float4 vertex : SV_POSITION;
                float2 uv: TEXCOORD0;
				float3 normal : POSITION1;
                float4 color : COLOR;
                half3 lightAmount : TEXCOORD2;
            };
            
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

            Varyings vert(Attributes v)
            {
                Varyings OUT;
                
				int data = asint(v.positionOS.x);
				float4 position = float4(data & 0xff, (data >> 8) & 0xff,(data >> 16) & 0xff, 1.0);
				float3 normal = float3(MyNormals[floor(asint(v.positionOS.z) & 0xff)]);
				float2 uv = float2(MyUVs[(asint(v.positionOS.z) >> 8) & 0xff]);
				float4 color = float4(float(v.positionOS.y), 0.0, 0.0, 1.0);

                OUT.vertex = TransformObjectToHClip(position.xyz);
                OUT.normal = normal;
                OUT.uv = uv;
                OUT.color = color;
                // Get the VertexNormalInputs of the vertex, which contains the normal in world space
                VertexNormalInputs positions = GetVertexNormalInputs(position);

                // Get the properties of the main light
                Light light = GetMainLight();

                // Calculate the amount of light the vertex receives
                OUT.lightAmount = LightingLambert(light.color, light.direction, positions.normalWS.xyz);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
				float4 col = tex2D(_MainTex, IN.uv); //* IN.color;// * float4(IN.lightAmount, 1);
                return col;
                // Set the fragment color to the interpolated amount of light
                //return float4(IN.lightAmount, 1);
            }
            ENDHLSL
        }
    }
}