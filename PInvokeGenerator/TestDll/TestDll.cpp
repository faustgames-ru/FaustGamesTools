#include "stdafx.h"

namespace Test
{

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
}