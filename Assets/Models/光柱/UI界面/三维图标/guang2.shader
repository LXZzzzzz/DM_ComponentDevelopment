// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/guang2"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_scale("scale", Float) = 0
		_Float0("Float 0", Float) = 0
		_Color("Color", Color) = (0,0.7372549,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		CGINCLUDE
		#pragma target 4.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform float _scale;
			uniform float _Float0;
			uniform float4 _Color;
			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float temp_output_13_0 = ( _scale * _Float0 );
				float3 objectToViewPos = UnityObjectToViewPos(v.vertex.xyz);
				float eyeDepth = -objectToViewPos.z;
				float cameraDepthFade9 = (( eyeDepth -_ProjectionParams.y - temp_output_13_0 ) / temp_output_13_0);
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue = ( cameraDepthFade9 * v.vertex.xyz );
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv_TextureSample0 = i.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				
				
				finalColor = ( _Color * tex2D( _TextureSample0, uv_TextureSample0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16201
1927;1;1906;1010;975.4672;206.3938;1.027416;True;True
Node;AmplifyShaderEditor.RangedFloatNode;14;-756.2131,435.3916;Float;False;Property;_scale;scale;1;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-749.2131,560.3916;Float;False;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;3061.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-417.2131,412.3916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;9;-125.7844,343.5638;Float;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;11;-147.2131,550.3916;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-383.2131,100.3916;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;6063fb5b2dbb039419a7cda7cf8db14b;6063fb5b2dbb039419a7cda7cf8db14b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-385.6005,-58.88687;Float;False;Property;_Color;Color;3;0;Create;True;0;0;False;0;0,0.7372549,1,0;0,0.737255,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;195.7869,431.3916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-21.60046,29.11313;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;19;461.3604,189.1401;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/guang2;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;4;0;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;13;0;14;0
WireConnection;13;1;15;0
WireConnection;9;0;13;0
WireConnection;9;1;13;0
WireConnection;12;0;9;0
WireConnection;12;1;11;0
WireConnection;18;0;17;0
WireConnection;18;1;10;0
WireConnection;19;0;18;0
WireConnection;19;1;12;0
ASEEND*/
//CHKSM=678F7C44B90C3C567EBA76BEB0745E6C59CAB188