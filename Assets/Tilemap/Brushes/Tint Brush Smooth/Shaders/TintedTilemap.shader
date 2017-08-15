 Shader "Custom/TintedTilemap" 
 {
		Properties
		{
			[PerRendererData]_MainTex ("Albedo (RGB)", 2D) = "white" {}
		}
  
		SubShader 
		{
			Tags { "Queue"="Transparent" "Render"="Transparent" "IgnoreProjector"="True"}
			LOD 200
          
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
  
			Pass{
			CGPROGRAM
  
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
  
				#include "UnityCG.cginc"
  
				struct appdata {
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};
  
				sampler2D _MainTex;
				sampler2D _TintMap;
				float _TintMapSize;

				struct v2f {
					float4 vertex : SV_POSITION;
					float4 uv : TEXCOORD0;
					float3 worldPos : float3;
				};            
  
				v2f vert(appdata v) {
					v2f o;
  
					o.worldPos = mul (unity_ObjectToWorld, v.vertex);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = float4(v.texcoord.xy, 0, 0);
  
					return o;
				}
  
				fixed4 frag(v2f i) : SV_Target {
					fixed4 col = tex2D (_MainTex, i.uv);
					fixed4 tint = tex2D(_TintMap, (i.worldPos.xy / _TintMapSize)  + .5);
					return tint * col;
			}
			ENDCG
		}
  
      
	}
	FallBack "Diffuse"
}
