// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Community/TFHC/Force Shield"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_ShieldPatternColor("Shield Pattern Color", Color) = (0.2470588,0.7764706,0.9098039,1)
		_ShieldPattern("Shield Pattern", 2D) = "white" {}
		[IntRange]_ShieldPatternSize("Shield Pattern Size", Range( 1 , 20)) = 5
		_ShieldPatternPower("Shield Pattern Power", Range( 0 , 100)) = 5
		_ShieldRimPower("Shield Rim Power", Range( 0 , 10)) = 7
		_IntersectIntensity("Intersect Intensity", Range( 0 , 1)) = 0.2
		_IntersectColor("Intersect Color", Color) = (0.03137255,0.2588235,0.3176471,1)
		_PanSpeed("PanSpeed", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform float4 _IntersectColor;
		uniform float _ShieldRimPower;
		uniform sampler2D _ShieldPattern;
		uniform float _ShieldPatternSize;
		uniform float2 _PanSpeed;
		uniform float4 _ShieldPatternColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _IntersectIntensity;
		uniform float _ShieldPatternPower;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 Albedo279 = _Color;
			o.Albedo = Albedo279.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float ShieldRimPower32 = _ShieldRimPower;
			float fresnelNdotV8 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode8 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV8, (10.0 + (ShieldRimPower32 - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float ShieldRim23 = fresnelNode8;
			float2 appendResult130 = (float2(_ShieldPatternSize , _ShieldPatternSize));
			float2 panner285 = ( 1.0 * _Time.y * _PanSpeed + float2( 0,0 ));
			float2 uv_TexCoord41 = i.uv_texcoord * appendResult130 + panner285;
			float4 ShieldPattern17 = tex2D( _ShieldPattern, uv_TexCoord41 );
			float4 ShieldPatternColor12 = _ShieldPatternColor;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth110 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth110 = abs( ( screenDepth110 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectIntensity ) );
			float clampResult113 = clamp( distanceDepth110 , 0.0 , 1.0 );
			float4 lerpResult124 = lerp( _IntersectColor , ( ( ShieldRim23 + ShieldPattern17 ) * ShieldPatternColor12 ) , clampResult113);
			float ShieldPower15 = _ShieldPatternPower;
			float4 Emission120 = ( lerpResult124 * ShieldPower15 );
			o.Emission = Emission120.rgb;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
266;680;1040;632;3490.965;2061.909;1.48827;True;False
Node;AmplifyShaderEditor.CommentaryNode;276;-2787.563,-2113.728;Float;False;1464.822;723.0327;Comment;14;285;15;6;12;23;3;17;1;41;130;129;32;31;287;Shield Main Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2311.771,-1592.201;Float;False;Property;_ShieldRimPower;Shield Rim Power;6;0;Create;True;0;0;False;0;7;5.57;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;275;-2376.808,-1338.62;Float;False;1030.896;385.0003;Comment;3;8;30;16;Shield RIM;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-2718.754,-1864.169;Float;False;Property;_ShieldPatternSize;Shield Pattern Size;4;1;[IntRange];Create;True;0;0;False;0;5;1;1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;287;-2694.739,-1564.826;Float;False;Property;_PanSpeed;PanSpeed;10;0;Create;True;0;0;False;0;0,0;0,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-1957.864,-1593.946;Float;False;ShieldRimPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;130;-2378.754,-1840.169;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;285;-2494.045,-1613.144;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;-2313.641,-1188.566;Float;False;32;ShieldRimPower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-2031.35,-1155.62;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;10;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-2181.77,-1881.404;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;8;-1819.751,-1153.92;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1917.31,-1898.503;Float;True;Property;_ShieldPattern;Shield Pattern;3;0;Create;True;0;0;False;0;None;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;269;-3905.509,-1331.722;Float;False;1462.593;625.2556;Mix of Pattern, Wave, Rim , Impact and adding intersection highlight;13;53;125;22;122;95;120;126;124;127;113;96;110;114;Shield Mix for Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;3;-2705.615,-2062.828;Float;False;Property;_ShieldPatternColor;Shield Pattern Color;2;0;Create;True;0;0;False;0;0.2470588,0.7764706,0.9098039,1;0.01311182,0.1411765,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1577.627,-1463.315;Float;False;ShieldRim;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1543.299,-1909.361;Float;False;ShieldPattern;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-3882.703,-802.8544;Float;False;Property;_IntersectIntensity;Intersect Intensity;8;0;Create;True;0;0;False;0;0.2;0.031;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2400.514,-2063.728;Float;False;ShieldPatternColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-3821.512,-1052.12;Float;False;17;ShieldPattern;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-3811.577,-1142.976;Float;False;23;ShieldRim;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2011.914,-2017.427;Float;False;Property;_ShieldPatternPower;Shield Pattern Power;5;0;Create;True;0;0;False;0;5;17.5;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;110;-3558.163,-825.9621;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-3584.187,-1102.981;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;-3704.301,-961.2315;Float;False;12;ShieldPatternColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;122;-3413.016,-1281.722;Float;False;Property;_IntersectColor;Intersect Color;9;0;Create;True;0;0;False;0;0.03137255,0.2588235,0.3176471,1;1,0.1760982,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-1691.913,-2007.526;Float;False;ShieldPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-3390.107,-1084.528;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;113;-3300.409,-837.5335;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;284;-2393.844,-826.8639;Float;False;539.9158;261.9131;Comment;2;279;128;Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;127;-3094.816,-933.2195;Float;False;15;ShieldPower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;124;-3111.815,-1073.623;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-2869.218,-1043.621;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;128;-2316.34,-776.8638;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;267;-3554.281,-1720.395;Float;False;733.7356;329.8646;Comment;4;84;35;36;34;Animation Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;279;-2066.391,-768.0189;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;-2685.917,-1068.622;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-3188.152,-1546.982;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;34;-3430.688,-1670.396;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;280;-795.4412,-1557.674;Float;False;279;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-796.6182,-1481.73;Float;False;120;Emission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-893.5145,-1351.604;Float;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;False;0;0.5;0.36;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-3504.281,-1477.241;Float;False;Property;_ShieldAnimSpeed;Shield Anim Speed;7;0;Create;True;0;0;False;0;3;3;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;272;-846.4404,-1252.875;Float;False;-1;;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-3030.152,-1543.928;Float;False;ShieldSpeed;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-551.0419,-1550.427;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASESampleShaders/Community/TFHC/Force Shield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;31;0
WireConnection;130;0;129;0
WireConnection;130;1;129;0
WireConnection;285;2;287;0
WireConnection;30;0;16;0
WireConnection;41;0;130;0
WireConnection;41;1;285;0
WireConnection;8;3;30;0
WireConnection;1;1;41;0
WireConnection;23;0;8;0
WireConnection;17;0;1;0
WireConnection;12;0;3;0
WireConnection;110;0;114;0
WireConnection;53;0;22;0
WireConnection;53;1;125;0
WireConnection;15;0;6;0
WireConnection;95;0;53;0
WireConnection;95;1;96;0
WireConnection;113;0;110;0
WireConnection;124;0;122;0
WireConnection;124;1;95;0
WireConnection;124;2;113;0
WireConnection;126;0;124;0
WireConnection;126;1;127;0
WireConnection;279;0;128;0
WireConnection;120;0;126;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;84;0;36;0
WireConnection;0;0;280;0
WireConnection;0;2;121;0
WireConnection;0;9;28;0
ASEEND*/
//CHKSM=1167188D9DD984A78DAA2D77A260DF926E88F812