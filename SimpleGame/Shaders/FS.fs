#version 330

uniform float u_Time;

layout(location=0) out vec4 FragColor;
in vec2 v_Tpos;

const float c_PI = 3.141592;

void simple()
{
	if(v_Tpos.x + v_Tpos.y + 0.5 > 1)
		FragColor = vec4(0.0);
	else{
		FragColor = vec4(v_Tpos, 1.0, 0.0);
	}
}

void LinePattern()
{
	float linecountH = 30;
	float linecountV = 30;
	float linewidth = 1;
	float per =  -0.5 * c_PI;
	linecountH = linecountH / 2;
	linecountV = linecountV / 2;
	linewidth = 50 / linewidth;
	float grey = pow(abs(sin((v_Tpos.y * 2 * c_PI + per)* linecountH)), linewidth);
	float grey2 = pow(abs(sin((v_Tpos.x * 2 * c_PI + per) * linecountV)), linewidth);
	FragColor = vec4(grey + grey2); 
}

void Circle()
{
	vec2 center = vec2(0.5,0.5);
	vec2 currpos = v_Tpos;
	float linewidth = 0.01;
	float radius = 0.1;
	float dist = distance(center,currpos);
	if(dist >= radius - linewidth && dist <= radius) {
		FragColor = vec4(v_Tpos,1.0,0.0);
	}
	else{
		FragColor = vec4(0.0);
	}
}

void CircleSin()
{
	float t = u_Time * 20;
	vec2 center = vec2(0.5,0.5);
	vec2 currpos = v_Tpos;
	float dist = distance(center,currpos);
	float value = abs(sin(dist * c_PI * 16 - t));
	FragColor = vec4(pow(value,16));
}

void main()
{ 
	CircleSin();
}
