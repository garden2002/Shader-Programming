#version 330

in float v_Grey;
in vec2 v_Tex;
in vec3 v_Color;

uniform sampler2D u_ParticleTex;
uniform sampler2D u_ParticleSpriteTex;

layout(location=0) out vec4 FragColor;

void CircleShape(){
	float d = distance(v_Tex, vec2(0.5));
	if(d < 0.5){
		FragColor = vec4(v_Color, clamp(0.5 - d , 0 , 0.5) *2.0 );
		return;
	}
	else{
		FragColor = vec4(0);
		//discard;
	}
}

void SingleTexture(){
    FragColor = v_Grey * texture(u_ParticleTex , v_Tex);
}

void AnimTexture(){
	float resolX = 5;
	float resolY = 5;
	float index = floor((1.0 - v_Grey) * (resolX * resolY - 1));
	float tx = v_Tex.x / resolX;
	float ty = v_Tex.y / resolY;
	float offsetX = fract(index / resolX);
	float offsetY = floor(index / resolY) / resolY;

	float d = distance(v_Tex, vec2(0.5));
	float value = clamp(0.5 - d , 0 , 0.5) *2.0;
	vec2 newTex = vec2(tx + offsetX , ty + offsetY);
    FragColor = texture(u_ParticleSpriteTex , newTex);
	FragColor.a *= value;
}

void main()
{
	AnimTexture();
}
