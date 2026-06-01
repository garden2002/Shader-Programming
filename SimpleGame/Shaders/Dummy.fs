#version 330

layout(location=0) out vec4 FragColor;

in float v_Grey;

void main()
{
	FragColor = v_Grey * vec4(1);
}
