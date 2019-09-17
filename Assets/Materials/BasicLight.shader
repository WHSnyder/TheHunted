//Simplified lightshaft shader cut out from https://medium.com/@avseoul/unity3d-how-to-create-fake-volumetric-light-using-shader-and-geometry-cf3885991720

Shader "Custom/BasicLight" {
    Properties {
        _Fresnel("Fresnel", Range (0., 10.)) = 1.
        _AlphaOffset("Alpha Offset", Range(0., 1.)) = 1.
        _Ambient("Ambient", Range(0., 1.)) = .3
        _Intensity("Intensity", Range(0., 1.5)) = .2
        _Fade("Fade", Range(0., 10.)) = 1.
        _Wind("Wind", Range(0., 1.)) = .1
    }
 
    SubShader {
        // set render type for transparency
        // transparent will draw after all the opaque geometry drawn 
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"} 
        LOD 100 // set level of detail minimum

        ZWrite Off // we don't need depth buffer, we're gonana use transparency and blending mode
        Blend SrcAlpha One // blend mode - additive with transparency
     
        Pass {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct v_data {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
 
            struct f_data {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
            };
 
            float _Fresnel;
            float _AlphaOffset;
            float _Ambient;
            float _Intensity;
            float _Fade;
            float _Wind;
             
            f_data vert (v_data v){
                
                f_data o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);    
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }
             
            fixed4 frag (f_data i) : SV_Target{

                half3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                float4 col = float4(1.0,1.0,1.0,1.0);

                // get raycast between vertice's dir and normal
                // discard vertices facing opposite way with view direction 
                // if the value is closer to 1 then that means the vertex is facing more towards the camera
                float raycast = saturate(dot(viewDir, i.normal));
                // make extreme distribution
                float fresnel = pow(raycast, _Fresnel);

                // fade out
                //float fade = saturate(pow(1. - i.uv.y, _Fade));

                col.a *= fresnel * _AlphaOffset;// * fade;

                return col;
            }
            ENDCG
        }
    }
}