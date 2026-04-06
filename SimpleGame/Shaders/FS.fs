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

void Fractal()
{
    // 시간에 따라 지수적으로 줌인
    float cycle = mod(u_Time * 10.0, 10.0); // 10초마다 리셋
    float zoom = pow(2.0, cycle);

    // 흥미로운 경계 지점으로 줌인 (Mandelbrot 유명한 좌표)
    vec2 target = vec2(-0.7269, 0.1889);

    vec2 c = (v_Tpos - vec2(0.5, 0.5)) * (3.0 / zoom) + target;

    vec2 z = vec2(0.0);
    float iter = 0.0;
    bool escaped = false;

    for(int n = 0; n < 128; n++)
    {
        z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
        if(dot(z, z) > 4.0)
        {
            iter = float(n);
            escaped = true;
            break;
        }
    }

    if(!escaped)
    {
        FragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    // Smooth coloring - 계단 현상 제거
    float smooth_iter = iter - log2(log2(dot(z, z))) + 4.0;
    float t = smooth_iter / 128.0;

    float phase = u_Time * 1.5;
    vec3 col = vec3(
        0.5 + 0.5 * sin(t * c_PI * 8.0 + phase),
        0.5 + 0.5 * sin(t * c_PI * 8.0 + phase + 2.094),
        0.5 + 0.5 * sin(t * c_PI * 8.0 + phase + 4.189)
    );

    FragColor = vec4(col, 1.0);
}

void main()
{ 
	Fractal();
}
