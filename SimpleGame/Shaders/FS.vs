#version 330

in vec3 a_Pos;
in vec2 a_TPos;
out vec2 v_Tpos;

void main()
{
    v_Tpos = a_TPos;
    gl_Position = vec4(a_Pos , 1);
}