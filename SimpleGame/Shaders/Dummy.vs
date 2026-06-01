#version 330

uniform float u_Time;

in vec3 a_Position;

const float c_PI = 3.141592;
out float v_Grey;

void main()
{
	float NewTime = u_Time * 3.f;
	float OneMinus = (a_Position.x + 0.5f);
	float newX = a_Position.x;
	float newY = a_Position.y + 0.25 * OneMinus * sin(2 * c_PI* (newX + 0.5) - NewTime);
	

	v_Grey = (sin((2 * c_PI* ((newX + 0.5)) - NewTime))+ 1.0) / 2.0;

	vec4 final = vec4(newX,newY, 0.0, 1.0);
	gl_Position = final;
}
