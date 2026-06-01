#version 330

layout(location=0) out vec4 FragColor;

in float v_Grey;
in vec2 v_TexCoord;

uniform sampler2D u_YenaTex;

const float PI = 3.141592;

mat2 rotate2D(float radian)
{
	float s = sin(radian);
	float c = cos(radian);
	return mat2(c, -s, s, c);
}

void main()
{
	vec2 newTex = v_TexCoord;
	FragColor = v_Grey * texture(u_YenaTex, newTex);
}
