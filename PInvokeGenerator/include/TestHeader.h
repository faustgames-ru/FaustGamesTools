#ifndef TESTHEADER_H
#define TESTHEADER_H

#define API_CALL __stdcall
#define DLLEXPORT __declspec( dllexport )

#define ushort unsigned short

namespace Test
{
	struct MeshVertex
	{
		ushort
	};

	class IUpdateArgs
	{
	public:
		virtual void API_CALL setElapsedTime(void * value) = 0;
	};

	extern "C" DLLEXPORT IUpdateArgs * API_CALL createUpdateArgs();
}

#endif /*TESTHEADER_H*/