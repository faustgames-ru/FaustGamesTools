#ifndef TESTHEADER_H
#define TESTHEADER_H

#define API_CALL __stdcall
#define DLLEXPORT __declspec( dllexport )

#define ushort unsigned short

namespace Test
{
	enum Shaders
	{
		SolidColor = 0x1,
		TextureColor = 0x2,
		TextureLighmapColor = 0x3,
	};

	struct MeshVertex
	{
	};

	class IUpdateArgs
	{
	public:
		virtual void API_CALL setElapsedTime(Shaders shader, void * value) = 0;
	};

	extern "C" DLLEXPORT IUpdateArgs * API_CALL createUpdateArgs();
}

#endif /*TESTHEADER_H*/