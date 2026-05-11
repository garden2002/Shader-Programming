#version 330



uniform float u_Time;
uniform sampler2D u_RGBTex;
uniform sampler2D u_NumsTex;
uniform sampler2D u_CurrNumTex;
uniform int u_InputNum;

layout(location=0) out vec4 FragColor;
in vec2 v_Tpos;

const float c_PI = 3.141592;

uniform vec4 u_DropInfo[1000]; // x,y: center, z: startTime, w: lifeTime

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
	float t = u_Time * 100;
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

 //==================== 원본 RainDrop (버그 보존) ====================
 void RainDrop()
 {
 	float accum = 0;
 	for(int i = 0; i < 1000; ++i){
 		float lifeTime = u_DropInfo[i].z; // [버그] z는 startTime인데 lifeTime으로 읽음
 		float startTime = u_DropInfo[i].w; // [버그] w는 lifeTime인데 startTime으로 읽음

 		float newtime = u_Time - startTime;

 		if (newtime > 0){
 			newtime = fract(newtime * 20 / lifeTime); // [문제] * 20 의도 불명확, fract 후 * lifeTime 하면 의미 없음
 			float oneMinus = 1 - newtime;
 			float t = newtime * lifeTime;

 			vec2 center = u_DropInfo[i].xy;
 			vec2 currpos = v_Tpos;

 			float range = t / 30; // [문제] 30이 하드코딩, t 스케일이 lifeTime에 따라 달라짐
 			float dist = distance(center,currpos);

 			float fade = 20 * clamp(range - dist,0,1); // [문제] edge가 날카롭고 range 밖은 전부 0

 			float value = pow(abs(sin(dist * c_PI * 4 - t)),32); // t 단위가 맞지 않아 파형 속도 불안정
 			accum += value * fade * oneMinus;
 		}
 		else{ }
 		FragColor = vec4(vec3(accum), 1.0); // [버그] 루프 안에 있어서 매 iteration마다 덮어씌워짐 -> accum 누적 의미 없음
 	}
 }
 //===================================================================

//void RainDrop()
//{
//	float accum = 0.0;
//
//	for(int i = 0; i < 1000; ++i){
//		float startTime = u_DropInfo[i].z; // z: startTime (주석 기준 수정)
//		float lifeTime  = u_DropInfo[i].w; // w: lifeTime  (주석 기준 수정)
//
//		if(lifeTime <= 0.0) continue;
//
//		float elapsed = u_Time - startTime; // 빗방울 생성 후 경과 시간
//
//		if(elapsed < 0.0 || elapsed > lifeTime) continue; // 수명 범위 밖이면 건너뜀
//
//		float progress = elapsed / lifeTime; // 0.0(시작) ~ 1.0(소멸), 정규화된 진행도
//		float fadeOut  = 1.0 - progress;     // 시간이 지날수록 투명해짐
//
//		vec2  center = u_DropInfo[i].xy;
//		float dist   = distance(center, v_Tpos);
//
//		float maxRadius = 0.35;             // 최대 확산 반경 (UV 공간)
//		float radius    = progress * maxRadius; // 현재 파문 반경 (시간에 따라 증가)
//
//		// 파문 링: 현재 반경 근처 픽셀만 밝아지는 Gaussian ring
//		float ringWidth = 0.015 + progress * 0.01; // 시간이 지날수록 링이 약간 퍼짐
//		float wave = exp(-pow((dist - radius) / ringWidth, 2.0));
//
//		// 중심에서 멀어질수록 추가로 감쇠
//		float distFade = exp(-dist * 3.0);
//
//		accum += wave * fadeOut * distFade;
//	}
//
//	accum = clamp(accum, 0.0, 1.0);
//	FragColor = vec4(vec3(accum), 1.0); // [수정] 루프 밖에서 한 번만 출력
//}


void Flag()
{
    float speed = 10.0;
    float time = u_Time * speed; // 시간에 따라 깃발이 흔들리는 속도 조절
    float oneMinus = 1 - v_Tpos.x;
    float fWidth = 0;
    float mix = mix(1, fWidth, v_Tpos.x); 
    float amp = 0.3;
    float sinInput = v_Tpos.x * c_PI * 2 - time;
    float sinValue = v_Tpos.x * amp * (((sin(sinInput) + 1) / 2) - 0.5) + 0.5;
    float width = 0.5 * mix;
    float grey = 0;

    if(v_Tpos.y < sinValue + width / 2 && v_Tpos.y > sinValue - width/2){
       grey = 1.0;
    }
    else{
       grey = 0.0;
    }
    FragColor = vec4(grey);
}

void Flame()
{
    float speed = 10.0;
    float NewY = 1 - v_Tpos.y; // y축을 뒤집어서 아래에서 위로 움직이도록
    float time = u_Time * speed; // 시간에 따라 깃발이 흔들리는 속도 조절
    float oneMinus = 1 - NewY;
    float fWidth = 0;
    float mix = mix(fWidth, 1, NewY); 
    float amp = 0.3;
    float sinInput = NewY * c_PI * 2 - time;
    float sinValue = NewY * amp * (((sin(sinInput) + 1) / 2) - 0.5) + 0.5;
    float width = 0.5 * mix;
    float grey = 0;

    if(v_Tpos.x < sinValue + width / 2 && v_Tpos.x > sinValue - width/2){
       grey = 1.0;
    }
    else{
       grey = 0.0;
    }
    FragColor = vec4(grey);
}


void TextureSampling()
{
    vec4 c0;
    vec4 c1;
    vec4 c2;
    vec4 c3;
    vec4 c4;

    float offsetX = 0.01;
    c0 = texture(u_RGBTex, vec2(v_Tpos.x - offsetX * 2 ,v_Tpos.y));
    c1 = texture(u_RGBTex, vec2(v_Tpos.x - offsetX * 1 ,v_Tpos.y));
    c2 = texture(u_RGBTex, vec2(v_Tpos.x - offsetX * 0 ,v_Tpos.y));
    c3 = texture(u_RGBTex, vec2(v_Tpos.x + offsetX * 1 ,v_Tpos.y));
    c4 = texture(u_RGBTex, vec2(v_Tpos.x + offsetX * 2 ,v_Tpos.y));

    vec4 sum = c0 + c1 + c2 + c3 + c4;
    sum = sum / 5.0; 
    FragColor = sum;
}

void TextureQ1()
{
    float tx = v_Tpos.x;
    float ty = 1 - (2 * abs(v_Tpos.y - 0.5f));
    vec2 newtexcoord = vec2(tx, ty);
    FragColor = texture(u_RGBTex, newtexcoord);
}

void TextureQ2()
{
    
    float tx = fract(v_Tpos.x * 3);
    float ty = v_Tpos.y / 3;
    float offsetX = 0;
    float offsetY = (2 - floor(v_Tpos.x * 3)) / 3;
    vec2 newtexcoord = vec2(tx, ty + offsetY);
    FragColor = texture(u_RGBTex, newtexcoord);
}


void TextureQ3()
{
    
    float tx = fract(v_Tpos.x * 3);
    float ty = v_Tpos.y / 3;
    float offsetX = 0;
    float offsetY = floor(v_Tpos.x * 3) / 3;
    vec2 newtexcoord = vec2(tx, ty + offsetY);
    FragColor = texture(u_RGBTex, newtexcoord);
}

void TextureQ4()
{

    float resolX = 3;
    float resolY = 5;
    float shear = 0.5 * u_Time;
    float offsetX =  fract(ceil(v_Tpos.y *resolY) * shear) ;
    float offsetY = 0;
    float tx = fract(offsetX + v_Tpos.x * resolX);
    float ty = fract(offsetY + v_Tpos.y * resolY);


    vec2 newtexcoord = vec2(tx, ty);
    FragColor = texture(u_RGBTex, newtexcoord);
}


void Num()
{
    
    float tx = v_Tpos.x;
    float ty = v_Tpos.y;
    float offsetX = 0;
    float offsetY = 0;
    vec2 newtexcoord = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_CurrNumTex, newtexcoord);
}

void Nums()
{
    float index = float(u_InputNum);
    float tx = v_Tpos.x / 5;
    float ty = v_Tpos.y / 2;
    float offsetX = fract(index / 5.0);
    float offsetY = floor(index / 5) / 2.0;
    vec2 newtexcoord = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_NumsTex, newtexcoord);
}


void main()
{ 
	TextureSampling();
}
