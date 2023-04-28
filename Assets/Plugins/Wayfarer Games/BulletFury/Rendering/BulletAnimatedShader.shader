Shader "BulletFury/Animated Bullet" {
  //show values to edit in inspector
  Properties{
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cols ("Cols Count", Int) = 5
		_Rows ("Rows Count", Int) = 3
		_Frame ("Per Frame Length", Float) = 0.5
  }

  SubShader
  {
    Tags{ "RenderType"="Transparent" "Queue"="Transparent"}
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    
    Pass{
      CGPROGRAM
      //allow instancing
      #pragma multi_compile_instancing

      //shader functions
      #pragma vertex vert
      #pragma fragment frag

      //use unity shader library
      #include "UnityCG.cginc"

      //per vertex data that comes from the model/parameters
      struct appdata{
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      //per vertex data that gets passed from the vertex to the fragment function
      struct v2f{
        float4 position : SV_POSITION;
        half2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        
      };

      UNITY_INSTANCING_BUFFER_START(Props)
      UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
      UNITY_INSTANCING_BUFFER_END(Props)
      
      sampler2D _MainTex;
      float4 _MainTex_ST;

      uint _Cols;
			uint _Rows;

			float _Frame;

      fixed4 shot (sampler2D tex, float2 uv, float dx, float dy, int frame)
      {
        return tex2D(tex, float2(
            (uv.x * dx) + fmod(frame, _Cols) * dx,
            1.0 - ((uv.y * dy) + (frame / _Cols) * dy)
          ));
      }

      v2f vert(appdata v){
        v2f o;

        //setup instance id
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);

        //calculate the position in clip space to render the object
        o.position = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
      }

      fixed4 frag(v2f i) : SV_TARGET{
          //setup instance id
          UNITY_SETUP_INSTANCE_ID(i);
          //get _Color Property from buffer
          fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

          int frames = _Rows * _Cols;
				  float frame = fmod(_Time.y / _Frame, frames);
				  int current = floor(frame);
				  float dx = 1.0 / _Cols;
				  float dy = 1.0 / _Rows;
        
          //Return the color the Object is rendered in
          return shot(_MainTex, i.texcoord, dx, dy, current) * color;
      }

      ENDCG
    }
  }
}