#version 330

layout(location=0) out vec4 FragColor;
in vec2 v_Tpos;

void main()
{ 
	if(v_Tpos.x + v_Tpos.y + 0.5> 1)
		FragColor = vec4(0.0);
	else{
		FragColor = vec4(v_Tpos, 1.0, 0.0);
	}
}
