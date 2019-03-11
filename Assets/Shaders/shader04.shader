// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/shader04"{
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
				#include "Lighting.cginc" 
				#pragma vertex vert	
				#pragma fragment frag 
				fixed4 _Diffuse;

				//application to vertex
				struct a2v{
					float4 vertex : POSITION;	
					float3 normal : NORMAL;
				};
				struct v2f{
					float4 position : SV_POSITION;
					fixed3 color : COLOR0;
				};

				v2f vert(a2v v){ 
					v2f f;
					f.position = UnityObjectToClipPos(v.vertex);
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb; //获取环境光
					fixed3 normalDir = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
					fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
					fixed3 diffuse = _LightColor0.rgb * max(0,dot(normalDir,lightDir)) * _Diffuse.rgb ;

					fixed3 reflectDir = reflect(-lightDir,normalDir);
					fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(v.vertex,(float3x3)unity_WorldToObject).xyz);
					fixed3 specular = _LightColor0.rgb * pow(max(dot(reflectDir,viewDir),0),10);

					f.color = diffuse + ambient + specular; 
					return f;
				}

				fixed4 frag(v2f f) : SV_Target{
					return fixed4(f.color,1);
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
