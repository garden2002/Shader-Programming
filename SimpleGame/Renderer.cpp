#include "stdafx.h"
#include "Renderer.h"

static std::random_device rd;
static std::default_random_engine dre(rd());
static std::uniform_real_distribution<float> urd(0.f, 1.f);
static std::uniform_real_distribution<float> velocity(-2.f, 2.f);

Renderer::Renderer(int windowSizeX, int windowSizeY)
{
	Initialize(windowSizeX, windowSizeY);
}


Renderer::~Renderer()
{
}

void Renderer::Initialize(int windowSizeX, int windowSizeY)
{
	//Set window size
	m_WindowSizeX = windowSizeX;
	m_WindowSizeY = windowSizeY;

	//Load shaders
	m_SolidRectShader = CompileShaders("./Shaders/SolidRect.vs", "./Shaders/SolidRect.fs");
	m_TriangleShader = CompileShaders("./Shaders/Triangle.vs", "./Shaders/Triangle.fs");
	m_ParticlesShader = CompileShaders("./Shaders/Particles.vs", "./Shaders/Particles.fs");
	//Create VBOs
	CreateVertexBufferObjects();

	if (m_SolidRectShader > 0 && m_TriangleShader > 0 
		&& m_VBORect > 0 && m_VBOTriangle > 0)
	{
		m_Initialized = true;
	}
}

bool Renderer::IsInitialized()
{
	return m_Initialized;
}

void Renderer::CreateTriangleVBO(int count)
{
	// 기존 VBO가 있으면 삭제
	if (m_VBOParticles > 0)
	{
		glDeleteBuffers(1, &m_VBOParticles);
		m_VBOParticles = 0;
	}

	float size = 0.01f;
	int verticesPerTriangle = 6;   // 삼각형 1개 = 정점 6개 (두 삼각형)
	std::vector<float> triangleData;
	triangleData.reserve(count * verticesPerTriangle * g_nInformationcount);

	float cx = 0;
	float cy = 0;
	for (int i = 0; i < count; i++)
	{

		float mass = 1.f;
		float vx = velocity(dre);
		float vy = velocity(dre);
		float rv = urd(dre);
		float rv1 = urd(dre);
		float life = urd(dre);
		float verts[6][g_nInformationcount] = {
			{ cx - size / 2, cy - size / 2, 0, mass, vx, vy, rv, rv1, life},
			{ cx + size / 2, cy - size / 2, 0, mass, vx, vy, rv, rv1, life},
			{ cx + size / 2, cy + size / 2, 0, mass, vx, vy, rv, rv1, life}, // triangle1
			{ cx - size / 2, cy - size / 2, 0, mass, vx, vy, rv, rv1, life},
			{ cx + size / 2, cy + size / 2, 0, mass, vx, vy, rv, rv1, life},
			{ cx - size / 2, cy + size / 2, 0, mass, vx, vy, rv, rv1, life}, // triangle2
		};

		for (int v = 0; v < 6; v++)
			for (int f = 0; f < g_nInformationcount; f++)
				triangleData.push_back(verts[v][f]);
	}

	glGenBuffers(1, &m_VBOParticles);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOParticles);
	glBufferData(GL_ARRAY_BUFFER,
		sizeof(float) * triangleData.size(),
		triangleData.data(),
		GL_STATIC_DRAW);

	m_TriangleCount = count; // 멤버 변수로 저장
}

void Renderer::CreateVertexBufferObjects()
{
	float rect[]
		=
	{
		-1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f, -1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, 1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, //Triangle1
		-1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f,  1.f / m_WindowSizeX, 1.f / m_WindowSizeY, 0.f, 1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f, //Triangle2
	};

	glGenBuffers(1, &m_VBORect);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
	glBufferData(GL_ARRAY_BUFFER, sizeof(rect), rect, GL_STATIC_DRAW);

	float centerx = 0;
	float centery = 0;
	float size = 0.1f;
	float mass = 1;
	float vx = 1.f;
	float vy = 3.f;
	float triangle[] = {
		centerx - size / 2, centery - size / 2, 0, mass,vx,vy,
		centerx + size / 2, centery - size / 2, 0, mass,vx,vy,
		centerx + size / 2, centery + size / 2, 0, mass,vx,vy,//triangle1

		centerx - size / 2, centery - size / 2, 0, mass,vx,vy,
		centerx + size / 2, centery + size / 2, 0, mass,vx,vy,
		centerx - size / 2, centery + size / 2, 0, mass,vx,vy//triangle2
	};
	glGenBuffers(1, &m_VBOTriangle);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTriangle);
	glBufferData(GL_ARRAY_BUFFER, sizeof(triangle), triangle, GL_STATIC_DRAW);
}

void Renderer::AddShader(GLuint ShaderProgram, const char* pShaderText, GLenum ShaderType)
{
	//쉐이더 오브젝트 생성
	GLuint ShaderObj = glCreateShader(ShaderType);

	if (ShaderObj == 0) {
		fprintf(stderr, "Error creating shader type %d\n", ShaderType);
	}

	const GLchar* p[1];
	p[0] = pShaderText;
	GLint Lengths[1];
	Lengths[0] = strlen(pShaderText);
	//쉐이더 코드를 쉐이더 오브젝트에 할당
	glShaderSource(ShaderObj, 1, p, Lengths);

	//할당된 쉐이더 코드를 컴파일
	glCompileShader(ShaderObj);

	GLint success;
	// ShaderObj 가 성공적으로 컴파일 되었는지 확인
	glGetShaderiv(ShaderObj, GL_COMPILE_STATUS, &success);
	if (!success) {
		GLchar InfoLog[1024];

		//OpenGL 의 shader log 데이터를 가져옴
		glGetShaderInfoLog(ShaderObj, 1024, NULL, InfoLog);
		fprintf(stderr, "Error compiling shader type %d: '%s'\n", ShaderType, InfoLog);
		printf("%s \n", pShaderText);
	}

	// ShaderProgram 에 attach!!
	glAttachShader(ShaderProgram, ShaderObj);
}

bool Renderer::ReadFile(char* filename, std::string *target)
{
	std::ifstream file(filename);
	if (file.fail())
	{
		std::cout << filename << " file loading failed.. \n";
		file.close();
		return false;
	}
	std::string line;
	while (getline(file, line)) {
		target->append(line.c_str());
		target->append("\n");
	}
	return true;
}

GLuint Renderer::CompileShaders(char* filenameVS, char* filenameFS)
{
	GLuint ShaderProgram = glCreateProgram(); //빈 쉐이더 프로그램 생성

	if (ShaderProgram == 0) { //쉐이더 프로그램이 만들어졌는지 확인
		fprintf(stderr, "Error creating shader program\n");
	}

	std::string vs, fs;

	//shader.vs 가 vs 안으로 로딩됨
	if (!ReadFile(filenameVS, &vs)) {
		printf("Error compiling vertex shader\n");
		return -1;
	};

	//shader.fs 가 fs 안으로 로딩됨
	if (!ReadFile(filenameFS, &fs)) {
		printf("Error compiling fragment shader\n");
		return -1;
	};

	// ShaderProgram 에 vs.c_str() 버텍스 쉐이더를 컴파일한 결과를 attach함
	AddShader(ShaderProgram, vs.c_str(), GL_VERTEX_SHADER);

	// ShaderProgram 에 fs.c_str() 프레그먼트 쉐이더를 컴파일한 결과를 attach함
	AddShader(ShaderProgram, fs.c_str(), GL_FRAGMENT_SHADER);

	GLint Success = 0;
	GLchar ErrorLog[1024] = { 0 };

	//Attach 완료된 shaderProgram 을 링킹함
	glLinkProgram(ShaderProgram);

	//링크가 성공했는지 확인
	glGetProgramiv(ShaderProgram, GL_LINK_STATUS, &Success);

	if (Success == 0) {
		// shader program 로그를 받아옴
		glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
		std::cout << filenameVS << ", " << filenameFS << " Error linking shader program\n" << ErrorLog;
		return -1;
	}

	glValidateProgram(ShaderProgram);
	glGetProgramiv(ShaderProgram, GL_VALIDATE_STATUS, &Success);
	if (!Success) {
		glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
		std::cout << filenameVS << ", " << filenameFS << " Error validating shader program\n" << ErrorLog;
		return -1;
	}

	glUseProgram(ShaderProgram);
	std::cout << filenameVS << ", " << filenameFS << " Shader compiling is done.";

	return ShaderProgram;
}

void Renderer::DrawSolidRect(float x, float y, float z, float size, float r, float g, float b, float a)
{
	float newX, newY;

	GetGLPosition(x, y, &newX, &newY);

	//Program select
	glUseProgram(m_SolidRectShader);

	glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Trans"), newX, newY, 0, size);
	glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Color"), r, g, b, a);

	int attribPosition = glGetAttribLocation(m_SolidRectShader, "a_Position");
	glEnableVertexAttribArray(attribPosition);
	glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
	glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, 0);

	glDrawArrays(GL_TRIANGLES, 0, 6);

	glDisableVertexAttribArray(attribPosition);

	glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

float g_time = 0.f;
void Renderer::DrawTriangle()
{
	//Program select
	g_time += 0.001f;
	glUseProgram(m_TriangleShader);

	int uTime = glGetUniformLocation(m_TriangleShader, "u_Time");
	glUniform1f(uTime, g_time);

	int attribPosition = glGetAttribLocation(m_TriangleShader, "a_Position");
	glEnableVertexAttribArray(attribPosition);
	int attribMass = glGetAttribLocation(m_TriangleShader, "a_Mass");
	glEnableVertexAttribArray(attribMass);
	int attribVel = glGetAttribLocation(m_TriangleShader, "a_Vel");
	glEnableVertexAttribArray(attribVel);

	glBindBuffer(GL_ARRAY_BUFFER, m_VBOTriangle);
	glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 6, 0);
	glVertexAttribPointer(attribMass, 1, GL_FLOAT, GL_FALSE, sizeof(float) * 6, (GLvoid*)(sizeof(float) * 3));
	glVertexAttribPointer(attribVel, 2, GL_FLOAT, GL_FALSE, sizeof(float) * 6, (GLvoid*)(sizeof(float) * 4));

	glDrawArrays(GL_TRIANGLES, 0, 6);
}

void Renderer::DrawParticles(int count)
{
	// count가 바뀌었으면 VBO 재생성
	if (count != m_TriangleCount)
	{
		CreateTriangleVBO(count);
	}

	g_time += 0.001f;
	glUseProgram(m_ParticlesShader);

	int uTime = glGetUniformLocation(m_ParticlesShader, "u_Time");
	glUniform1f(uTime, g_time);

	int attribPosition = glGetAttribLocation(m_ParticlesShader, "a_Position");
	glEnableVertexAttribArray(attribPosition);
	int attribMass = glGetAttribLocation(m_ParticlesShader, "a_Mass");
	glEnableVertexAttribArray(attribMass);
	int attribVel = glGetAttribLocation(m_ParticlesShader, "a_Vel");
	glEnableVertexAttribArray(attribVel);
	int attribRv = glGetAttribLocation(m_ParticlesShader, "a_Rv");
	glEnableVertexAttribArray(attribRv);
	int attribRv1 = glGetAttribLocation(m_ParticlesShader, "a_Rv1");
	glEnableVertexAttribArray(attribRv1);
	int attribLife = glGetAttribLocation(m_ParticlesShader, "a_Life");
	glEnableVertexAttribArray(attribLife);





	glBindBuffer(GL_ARRAY_BUFFER, m_VBOParticles);
	glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 0));
	glVertexAttribPointer(attribMass, 1, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 3));
	glVertexAttribPointer(attribVel, 2, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 4));
	glVertexAttribPointer(attribRv, 1, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 6));
	glVertexAttribPointer(attribRv1, 1, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 7));
	glVertexAttribPointer(attribLife, 1, GL_FLOAT, GL_FALSE, sizeof(float) * g_nInformationcount, (GLvoid*)(sizeof(float) * 8));

	glDrawArrays(GL_TRIANGLES, 0, 6 * count); // ← count만큼 정점 수 확장

	glDisableVertexAttribArray(attribPosition);
	glDisableVertexAttribArray(attribMass);
	glDisableVertexAttribArray(attribVel);
	glDisableVertexAttribArray(attribRv);
	glDisableVertexAttribArray(attribRv1);
	glDisableVertexAttribArray(attribLife);
}

void Renderer::GetGLPosition(float x, float y, float *newX, float *newY)
{
	*newX = x * 2.f / m_WindowSizeX;
	*newY = y * 2.f / m_WindowSizeY;
}