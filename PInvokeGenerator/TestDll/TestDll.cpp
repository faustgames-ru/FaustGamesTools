#include "stdafx.h"

namespace Test
{
	class CTestSystem : public ITestSystem
	{
	public:
		virtual void API_CALL update(const IUpdateArgs * args) 
		{
		}
	};

	class CUpdateArgs : public IUpdateArgs
	{
	private:
		float _elapsedTime;
	public:
		virtual float API_CALL getElapsedTime()
		{
			return _elapsedTime;
		}
		virtual void API_CALL setElapsedTime(float value)
		{
			_elapsedTime = value;
		}
	};

	class CTestFactory : public ITestFactory
	{
	public:
		virtual ITestSystem * API_CALL createTestSystem()
		{
			return new CTestSystem();
		}
		virtual IUpdateArgs * API_CALL createUpdateArgs()
		{
			return new CUpdateArgs();
		}
	};

	extern "C" DLLEXPORT ITestFactory * API_CALL createTestFactory()
	{
		return new CTestFactory();
	}
}