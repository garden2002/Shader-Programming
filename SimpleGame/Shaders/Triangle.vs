#version 330

uniform float u_Time;

in vec3 a_Position;

#define PI 3.141592

void Sin1()
{
    float t = mod(u_Time * 2.0 , 1.0);
    vec4 newPosition;
    newPosition.x = a_Position.x + t;
    newPosition.y = a_Position.y + 0.5 * sin(t * 2 * PI);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Sin2()
{
    float t = mod(u_Time * 2.0 , 1.0);
    vec4 newPosition;
    newPosition.x = a_Position.x + (t * 2.0 - 1.0);
    newPosition.y = a_Position.y + 0.5 * sin(t * 2.0 * PI);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Circle()
{
    float t = mod(u_Time * 2.0, 1.0);
    float angle = t * 2.0 * PI;
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
    float angle = t * 2.0 * PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle) * sin(angle * 4.0);
    newPosition.y = a_Position.y + sin(angle) * sin(angle * 4.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Circle1()
{
    float t = mod(u_Time * 2.0, 1.0);
    float angle = t * 2.0 * PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle) * cos(angle * 5.0);
    newPosition.y = a_Position.y + sin(angle * 3.0) * cos(angle * 5.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void Circle2()
{
    float t = mod(u_Time * 2.0, 1.0);
    float angle = t * 2.0 * PI;
    vec4 newPosition;
    newPosition.x = a_Position.x + cos(angle * 3.0);
    newPosition.y = a_Position.y + sin(angle * 2.0);
    newPosition.z = 0;
    newPosition.w = 1;
    gl_Position = newPosition;  
}

void main()
{
    Circle2();
}