#version 330

uniform float u_Time;

in vec3 a_Position;

const float c_PI = 3.141592;
out float v_Grey;
out vec2 v_TexCoord;

void Flag(){

	float tx = a_Position.x + 0.5;
	float ty = 1 - (a_Position.y + 0.5);
	v_TexCoord = vec2(tx, ty);
	float NewTime = u_Time * 3.f;
	float Value = (a_Position.x + 0.5f);
	float newX = a_Position.x;
	float newY = a_Position.y  * (1.0 - (Value *0.5)) + 0.25 * Value * sin(2 * c_PI* (newX + 0.5) - NewTime);
	

	v_Grey = (sin((2 * c_PI* ((newX + 0.5)) - NewTime))+ 1.0) / 2.0;

	vec4 final = vec4(newX,newY, 0.0, 1.0);
	gl_Position = final;
}



void main()
{
	Flag();
}
	
