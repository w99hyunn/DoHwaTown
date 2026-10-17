Shader "DoHwa/UI/FieldEdgeShade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0.06,0.07,0.1,0.9)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct Varying { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            fixed4 _Color;
            Varying vert(Input v) { Varying o; o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o; }
            fixed4 frag(Varying i):SV_Target
            {
                float side=pow(1-i.uv.x,5)*smoothstep(.35,.70,i.uv.y);
                float edge=pow(abs(i.uv.y-.5)*2,5)*.78;
                i.color.a*=max(side,edge);
                return i.color;
            }
            ENDCG
        }
    }
}
