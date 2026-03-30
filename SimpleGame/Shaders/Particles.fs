#version 330

in float v_Grey;

layout(location=0) out vec4 FragColor;

void main()
{
	FragColor = vec4(v_Grey);
}
