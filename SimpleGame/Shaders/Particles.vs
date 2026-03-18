#version 330

uniform float u_Time;

in vec3 a_Position;
in float a_Mass;
in vec2 a_Vel;
const float c_PI = 3.141592;
const float c_G = -9.8;
 
void Sin1()
{
    float t = mod(u_Time * 2.0 , 1.0);
    vec4 newPosition;
    newPosition.x = a_Position.x + t;
    newPosition.y = a_Position.y + 0.5 * sin(t * 2 * c_PI);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Sin2()
{
    float t = mod(u_Time * 2.0 , 1.0);
    vec4 newPosition;
    newPosition.x = a_Position.x + (t * 2.0 - 1.0);
    newPosition.y = a_Position.y + 0.5 * sin(t * 2.0 * c_PI);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Circle()
{
    float t = mod(u_Time * 2.0, 1.0);
    float angle = t * 2.0 * c_PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle);
    newPosition.y = a_Position.y + sin(angle);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Rose()
{
    float t = mod(u_Time , 1.0);
    float angle = t * 2.0 * c_PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle) * sin(angle * 4.0);
    newPosition.y = a_Position.y + sin(angle) * sin(angle * 4.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void LissajousCurve()
{
    float t = mod(u_Time * 2.0, 1.0);
    float angle = t * 2.0 * c_PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle) * cos(angle * 5.0);
    newPosition.y = a_Position.y + sin(angle * 3.0) * cos(angle * 5.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Butterfly()
{
    float t = mod(u_Time * 1.0, 1.0);
    float angle = t * 2.0 * c_PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle * 3.0);
    newPosition.y = a_Position.y + sin(angle * 2.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}


void Falling()
{
    float t = mod(u_Time * 2.0, 1.0);
    float vx , vy;
    float tt = t * t;
    vx = a_Vel.x;
    vy = a_Vel.y;
    vec4 newPosition;
    newPosition.x = a_Position.x + vx * t;
    newPosition.y = a_Position.y + vy * t + 0.5 * c_G * tt;
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void main()
{
    Falling();
}