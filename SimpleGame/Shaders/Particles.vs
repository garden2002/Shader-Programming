//#version 330
//
//uniform float u_Time;
//
//in vec3 a_Position;
//in float a_Mass;
//in vec2 a_Vel;
//in float a_Rv;
//in float a_Rv1;

//const float c_PI = 3.141592;
//const float c_G = -9.8;
 
// float rand(float x) {
//    return fract(sin(x) * 43758.5453);
//}


//void Falling()
//{
//   float startTime = rand(a_Rv);
//    float newTime = u_Time - startTime;
//    if(newTime > 0.0)
//    {
//        float t = mod(newTime * 2.f, 1.0);
//        float vx , vy;
//        float tt = t * t;
//        vx = a_Vel.x;
//       vy = a_Vel.y;
//        float angle = a_Rv * 2.0 * c_PI;
//        vec4 newPosition;
//        newPosition.x = (a_Position.x * a_Rv1) + cos(angle) + vx * t;
//        newPosition.y = (a_Position.y * a_Rv1) + sin(angle) + vy * t + 0.5 * c_G * tt;
//        newPosition.z = 0;
//        newPosition.w = 1;
//        gl_Position = newPosition;  
//    }
//    else{
//        gl_Position = vec4(-1000 , 0 , 0 ,1);
//    }
//    
//}
//
//void main()
//{
//    Falling();
//}


//#version 330
//
//uniform float u_Time;
//
//in vec3 a_Position;
//in float a_Mass;
//in vec2 a_Vel;
//in float a_Rv;
//in float a_Rv1;
//
//const float c_PI = 3.141592;
//const float c_G = -9.8;
//
//float rand(float x) {
//    return fract(sin(x) * 43758.5453);
//}
//
//// 개선1: smoothstep 기반 fade (0~1 → 부드러운 0→1→0)
//float fadeAlpha(float t) {
//    float fadeIn  = smoothstep(0.0, 0.1, t);
//    float fadeOut = 1.0 - smoothstep(0.6, 1.0, t);
//    return fadeIn * fadeOut;
//}
//
//void Falling()
//{
//    // 개선2: 두 rv를 조합해서 startTime 분산 → 파티클들이 고르게 퍼짐
//    float startTime = rand(a_Rv + a_Rv1 * 31.7);
//
//    float newTime = u_Time - startTime * 4.0;
//    if(newTime > 0.0)
//    {
//        // 개선3: 파티클마다 다른 루프 길이 (0.6~1.4초)
//        float loopDur = 0.6 + rand(a_Rv1 * 7.3) * 0.8;
//        float raw = mod(newTime, loopDur);
//        float t   = raw / loopDur;
//
//        // 개선4: 수평 공기저항 (지수 감쇠)
//        float drag = exp(-raw * 1.2);
//
//        float angle = a_Rv * 2.0 * c_PI;
//
//        vec4 newPosition;
//        newPosition.x = (a_Position.x * a_Rv1) + cos(angle)
//                        + a_Vel.x * raw * drag;          // drag 적용
//        newPosition.y = (a_Position.y * a_Rv1) + sin(angle)
//                        + a_Vel.y * raw
//                        + 0.5 * c_G * raw * raw;         // raw 사용 (루프 내 실제 시간)
//        newPosition.z = 0.0;
//        newPosition.w = 1.0;
//
//        gl_Position = newPosition;
//    }
//    else
//    {
//        gl_Position = vec4(-1000.0, 0.0, 0.0, 1.0);
//    }
//}
//
//void main()
//{
//    Falling();
//}


#version 330

uniform float u_Time;

in vec3  a_Position;  // 쿼드 코너 오프셋 (±0.025)
in float a_Mass;
in vec2  a_Vel;
in float a_Rv;
in float a_Rv1;

const float c_PI = 3.141592;

void main()
{
    // ── 나선 궤도 ──────────────────────────────────
    float orbitR   = 0.1 + a_Rv1 * 0.8;
    float orbitSpd = 0.4 + a_Rv  * 1.8;
    float armAngle = a_Rv * 2.0 * c_PI;

    float twist = armAngle + orbitSpd * u_Time - orbitR * 2.5;

    // ── 파동 + 맥동 ───────────────────────────────
    float wave  = sin(u_Time * (1.2 + a_Rv * 1.5) + a_Rv1 * c_PI * 3.0) * 0.06 * a_Rv1;
    float pulse = 1.0 + 0.07 * sin(u_Time * 2.1 + a_Rv1 * c_PI * 2.0);

    // ── 쿼드 중심 위치 ────────────────────────────
    float cx = cos(twist) * orbitR * pulse;
    float cy = sin(twist) * orbitR * pulse + wave;

    // ── 쿼드 코너 오프셋 유지 (a_Position이 이미 ±size/2) ──
    vec4 pos;
    pos.x = cx + a_Position.x;
    pos.y = cy + a_Position.y;
    pos.z = 0.0;
    pos.w = 1.0;

    gl_Position = pos;
}