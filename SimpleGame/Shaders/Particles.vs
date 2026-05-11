#version 330

uniform float u_Time;

in vec3 a_Position;
in float a_Mass;
in vec2 a_Vel;
in float a_Rv;
in float a_Rv1;
in float a_Life;

out float v_Grey;

const float c_PI = 3.141592;
const float c_G = -9.8;

 float rand(float x) {
    return fract(sin(x) * 43758.5453);
}

void Sin0()
{
    float startTime = a_Rv1 * 2;
    float newTime = u_Time - startTime;
    if(newTime > 0.0)
    {
        float lifeTime = 0.5 + a_Life * 2.0;
        float t = mod(newTime * 2.f, lifeTime);
        float amp = (lifeTime - t) * 0.1f * ((a_Rv - 0.5) * 2);
        float period = 0.2f + rand(a_Rv);
        vec4 newPosition;
        newPosition.x = a_Position.x + t;
        newPosition.y = a_Position.y + amp * sin(t * 2 * c_PI * period);
        newPosition.z = 0;
        newPosition.w = 1;
        gl_Position = newPosition;  
        v_Grey = (lifeTime - t);
    }
    else{
        gl_Position = vec4(-1000 , 0 , 0 ,1);
    }
}

void Falling()
{
    float startTime = rand(a_Rv);
    float newTime = u_Time - startTime;
    if(newTime > 0.0)
    {
        float lifeScale = 2.0;
        float newlifeTime = 0.5f + a_Life * lifeScale;
        float t = newlifeTime * fract(newTime / newlifeTime );
        float vx , vy;
        float tt = t * t;
        vx = a_Vel.x;
        vy = a_Vel.y;
        float angle = a_Rv * 2.0 * c_PI;
        vec4 newPosition;
        newPosition.x = (a_Position.x * a_Rv1) + cos(angle) + vx * t;
        newPosition.y = (a_Position.y * a_Rv1) + sin(angle) + vy * t + 0.5 * c_G * tt;
        newPosition.z = 0;
        newPosition.w = 1;
        gl_Position = newPosition;  
    }
    else{
        gl_Position = vec4(-1000 , 0 , 0 ,1);
    }
    
}


void main() {
    Falling();
}


//#version 330
//
//uniform float u_Time;
//
//in vec3  a_Position;  // 쿼드 코너 오프셋 (±0.025)
//in float a_Mass;
//in vec2  a_Vel;
//in float a_Rv;
//in float a_Rv1;
//
//const float c_PI = 3.141592;
//
//void main()
//{
//    // ── 나선 궤도 ──────────────────────────────────
//    float orbitR   = 0.1 + a_Rv1 * 0.8;
//    float orbitSpd = 0.4 + a_Rv  * 1.8;
//    float armAngle = a_Rv * 2.0 * c_PI;
//
//    float twist = armAngle + orbitSpd * u_Time - orbitR * 2.5;
//
//    // ── 파동 + 맥동 ───────────────────────────────
//    float wave  = sin(u_Time * (1.2 + a_Rv * 1.5) + a_Rv1 * c_PI * 3.0) * 0.06 * a_Rv1;
//    float pulse = 1.0 + 0.07 * sin(u_Time * 2.1 + a_Rv1 * c_PI * 2.0);
//
//    // ── 쿼드 중심 위치 ────────────────────────────
//    float cx = cos(twist) * orbitR * pulse;
//    float cy = sin(twist) * orbitR * pulse + wave;
//
//    // ── 쿼드 코너 오프셋 유지 (a_Position이 이미 ±size/2) ──
//    vec4 pos;
//    pos.x = cx + a_Position.x;
//    pos.y = cy + a_Position.y;
//    pos.z = 0.0;
//    pos.w = 1.0;
//
//    gl_Position = pos;
//}
//void Sin1()
//{
//    float startTime = a_Rv1 * 2.0;
//    float newTime = u_Time - startTime;
//
//    if(newTime <= 0.0)
//    {
//        gl_Position = vec4(0.0);
//        return;
//    }
//
//    float lifeTime = 0.5 + a_Life * 3.0;
//    float t = mod(newTime, lifeTime);
//    float lifeRatio = t / lifeTime;
//
//    // 👉 오른쪽 이동 (감속 느낌)
//    float speed = 1.5;
//    float x = a_Position.x + speed * (1.0 - exp(-2.0 * t));
//
//    // 👉 퍼짐 (좌우 랜덤)
//    float spread = (rand(a_Rv) - 0.5) * 2.0;
//
//    // 👉 sin 흔들림 + 랜덤 위상
//    float phase = a_Rv * 6.2831;
//    float wave = sin(t * 10.0 + phase);
//
//    // 👉 감쇠
//    float fade = exp(-3.0 * lifeRatio);
//
//    float y = a_Position.y 
//            + spread * 0.3 * t          // 퍼짐
//            + wave * 0.1 * fade;        // 흔들림 (점점 줄어듦)
//
//    vec4 newPosition = vec4(x, y, 0.0, 1.0);
//    gl_Position = newPosition;
//}
//void RocketLaunch()
//{
//    float startTime = a_Rv1 * 2.0;
//    float newTime = u_Time - startTime;
//
//    if (newTime > 0.0)
//    {
//        // ── 수명 계산 ──────────────────────────────
//        float lifeTime = 0.5 + a_Life * 3.0;
//        float t = mod(newTime, lifeTime);
//        float tNorm = t / lifeTime; // 0~1 진행도
//
//        // ── x축: 오른쪽으로 이동 (시간에 따라 가속) ──
//        float speed = length(a_Vel) + 0.5 + a_Rv * 0.5;
//        float x = a_Position.x + t * speed;
//
//        // ── y축: 사인 곡선 진동 (진폭은 점점 감소) ──
//        float amp    = 0.15 * ((a_Rv - 0.5) * 2.0); // 파티클마다 다른 진폭
//        float period = 0.3 + rand(a_Rv) * 0.4;       // 파티클마다 다른 주기
//        float decay  = 1.0 - tNorm;                   // 끝으로 갈수록 진폭 감소
//        float y = a_Position.y + amp * sin(t * 2.0 * c_PI * period) * decay;
//
//        gl_Position = vec4(x, y, 0.0, 1.0);
//    }
//    else
//    {
//        gl_Position = vec4(-9999.0, 0.0, 0.0, 1.0);
//    }
//}