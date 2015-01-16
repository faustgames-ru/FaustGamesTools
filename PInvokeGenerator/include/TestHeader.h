#ifndef TESTHEADER_H
#define TESTHEADER_H

#define API_CALL _stdcall

struct Vector2
{
	float x;
	float y;
};

struct Vector3
{
	float x;
	float y;
	float z;
};

struct Vector4
{
	float x;
	float y;
	float z;
	float w;
};

class IUpdateArgs
{
public:
	virtual float API_CALL getElapsedTime() = 0;
	virtual void API_CALL setElapsedTime(float value) = 0;
};

class ITestSystem
{
public:
	virtual void API_CALL update(const IUpdateArgs *args) = 0;
};

class ITestFactory
{
public:
	virtual ITestSystem * API_CALL ñreateTestSystem() = 0;
	virtual IUpdateArgs * API_CALL ñreateUpdateArgs() = 0;
};

ITestFactory * API_CALL CreateTestFactory();

#endif /*TESTHEADER_H*/