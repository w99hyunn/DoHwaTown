Shader "DoHwa/Peach Forest Water"
{
    Properties
    {
        _BaseColor("Deep jade", Color) = (.15,.32,.36,1)
        _ShallowColor("Shallow water", Color) = (.32,.46,.39,1)
        _ReflectionColor("Warm sky", Color) = (.70,.64,.70,1)
        _WaveScale("Ripple scale",Float)=2.2
        _WaveSpeed("Current speed",Float)=.22
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _ShallowColor;
                half4 _ReflectionColor;
                float _WaveScale;
                float _WaveSpeed;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; };
            struct Varyings { float4 positionCS:SV_POSITION; float3 positionWS:TEXCOORD0; half fog:TEXCOORD1; };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS=TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS=TransformWorldToHClip(output.positionWS);
                output.fog=ComputeFogFactor(output.positionCS.z);
                return output;
            }
            half4 Frag(Varyings input):SV_Target
            {
                float2 uv=GetNormalizedScreenSpaceUV(input.positionCS);
                float raw=SampleSceneDepth(uv);
                float sceneDepth=IsPerspectiveProjection()?LinearEyeDepth(raw,_ZBufferParams):LinearDepthToEyeDepth(raw);
                float surfaceDepth=-TransformWorldToView(input.positionWS).z;
                float depth=max(0,sceneDepth-surfaceDepth);
                clip(depth-.006);
                float2 p=input.positionWS.xz*_WaveScale;
                float t=_Time.y*_WaveSpeed;
                float a=p.x*.7+p.y*1.2+t;
                float b=p.x*1.8-p.y*.6-t*1.4;
                half3 normal=normalize(half3(cos(a)*.05+cos(b)*.025,1,cos(a)*.06-cos(b)*.02));
                half3 view=GetWorldSpaceNormalizeViewDir(input.positionWS);
                half fresnel=pow(1-saturate(dot(view,normal)),2);
                Light light=GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 water=lerp(_ShallowColor.rgb,_BaseColor.rgb,smoothstep(.08,1.15,depth));
                half3 bed=SampleSceneColor(uv+normal.xz*.0018);
                water=lerp(bed*half3(.73,.87,.83),water,.65);
                water=lerp(water,_ReflectionColor.rgb,fresnel*.45);
                half glint=pow(saturate(dot(normal,normalize(view+light.direction))),110);
                half wave=smoothstep(.92,.995,sin(a*.65+sin(b)*.55))*.015;
                half shore=(1-smoothstep(.02,.17,depth))*smoothstep(.2,.7,sin(a*1.7+b)*.5+.5);
                water*=.78+.22*light.shadowAttenuation;
                water+=glint*light.color*light.shadowAttenuation*.22+wave;
                water=lerp(water,half3(.65,.72,.63),shore*.12);
                return half4(MixFog(water,input.fog),lerp(.68,.94,smoothstep(.015,.38,depth)));
            }
            ENDHLSL
        }
    }
}
