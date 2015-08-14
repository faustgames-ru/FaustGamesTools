#ifndef LLGE_H
#define LLGE_H

#ifdef __ANDROID__
#include <jni.h>
#define API_CALL JNICALL
#define DLLEXPORT JNIEXPORT
#else 
#ifdef __APPLE__
#define API_CALL
#define DLLEXPORT
#else
#define API_CALL __stdcall
#define DLLEXPORT __declspec( dllexport )
#endif
#endif

typedef unsigned short ushort;
typedef unsigned int uint;
typedef const char * String;
typedef void * IntPtr;

namespace llge
{
	/// graphics enums

	enum BlendMode
	{
		None = 0x0,
		Alpha = 0x1,
		Additive = 0x2,
	};

	enum GraphicsEffects
	{
		EffectTextureColor = 0x0,
		EffectTextureLightmapColor = 0x1,
		EffectWater = 0x2,
		EffectSolid = 0x3,
		EffectRenderDepth = 0x4,
		EffectSolidColor = 0x5,
		EffectBloomFilter = 0x6,
	};

	enum GraphicsVertexFormats
	{
		FormatPositionTextureColor = 0x0,
		FormatPositionNormal = 0x1,
		FormatPositionTexture = 0x2,
		FormatPositionColor = 0x3,
	};
		
	enum CubemapPlane
	{
		PositiveX = 0x0,
		NegativeX = 0x1,
		PositiveY = 0x2,
		NegativeY = 0x3,
		PositiveZ = 0x4,
		NegativeZ = 0x5,
	};

	enum TextureImage2dFormat
	{
		Rgba = 0x0,
		Rgb = 0x1,
	};


	/// entities enums
	enum ComponentsTypes
	{
		Aadd2d = 0x1,
		Transform2d = 0x2,
		Render2d = 0x4,
		MatrixTransform = 0x8,
	};

	/// graphics
	class ITexture
	{
	public:
		virtual uint API_CALL getId() = 0;
		virtual IntPtr API_CALL getTextureInstance() = 0;
	};

	class ITextureImage2d
	{
	public:
		virtual IntPtr API_CALL getTextureImageInstance() = 0;
		virtual ITexture* API_CALL getTexture() = 0;
		virtual void API_CALL LoadPixels(int width, int height, TextureImage2dFormat format, void *pixels) = 0;
		virtual void API_CALL create() = 0;
		virtual void API_CALL cleanup() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IRenderTarget2d
	{
	public:
		virtual IntPtr API_CALL getRenderTargetInstance() = 0;
		virtual ITexture* API_CALL getTexture() = 0;
		virtual void API_CALL create(int width, int height) = 0;
		virtual void API_CALL cleanup() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IRenderTargetDepth2d
	{
	public:
		virtual IntPtr API_CALL getRenderTargetInstance() = 0;
		virtual ITexture* API_CALL getTexture() = 0;
		virtual ITexture* API_CALL getDepthTexture() = 0;
		virtual void API_CALL create(int width, int height) = 0;
		virtual void API_CALL cleanup() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IUniformsFacade
	{
	public:
		virtual void API_CALL resetSamplers() = 0;
		virtual void API_CALL setTime(float value) = 0;
		virtual void API_CALL setNormalmap(ITexture *texture) = 0;
		virtual void API_CALL setEnvironment(ITexture *texture) = 0;
		virtual void API_CALL setDepthmap(ITexture *texture) = 0;
		virtual void API_CALL setTexture(ITexture *texture) = 0;
		virtual void API_CALL setLightMap(ITexture *texture) = 0;
		virtual void API_CALL setProjection(void *floatMatrix) = 0;
	};

	class IVertexBuffer
	{
	public:
		virtual int API_CALL getId() = 0;
		virtual void API_CALL create() = 0;
		virtual void API_CALL setData(void *data, int count) = 0;
		virtual void API_CALL cleanup() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IGraphicsFacade
	{
	public:
		virtual IUniformsFacade * API_CALL getUniforms() = 0;

		virtual void API_CALL viewport(int width, int height) = 0;
		virtual void API_CALL setRenderTarget(void *renderTargetInstance) = 0;
		virtual void API_CALL setClearState(uint color, float depth) = 0;
		virtual void API_CALL setBlendMode(BlendMode blendMode) = 0;
		virtual void API_CALL clear() = 0;
		virtual void API_CALL clearDepth() = 0;

		virtual void API_CALL resetDrawCallsCounter() = 0;
		virtual int API_CALL getDrawCallsCounterValue() = 0;

		virtual void API_CALL drawEdges(GraphicsEffects effect, GraphicsVertexFormats vertexFormat, void *vertices, int primitivesCount) = 0;
		virtual void API_CALL draw(GraphicsEffects effect, GraphicsVertexFormats vertexFormat, void *vertices, int primitivesCount) = 0;
		virtual void API_CALL drawElements(GraphicsEffects effect, GraphicsVertexFormats vertexFormat, void *vertices, void *indices, int primitivesCount) = 0;
		virtual void API_CALL drawVertexBuffer(GraphicsEffects effect, GraphicsVertexFormats vertexFormat, IVertexBuffer *vertexBuffer, void *indices, int primitivesCount) = 0;
		virtual void API_CALL setEffectConstantFloat(GraphicsEffects effect, char *name, float value) = 0;
		virtual void API_CALL setEffectConstantColor(GraphicsEffects effect, char *name, uint value) = 0;

        virtual void API_CALL create() = 0;
		virtual void API_CALL grabDefaultRenderTarget() = 0;
		virtual void API_CALL cleanup() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IGraphicsFactory
	{
	public:
		virtual IGraphicsFacade * API_CALL createGraphicsFacade() = 0;
		virtual ITextureImage2d * API_CALL createTextureImage2d(bool generateMipmaps, bool useFilter) = 0;
		virtual IRenderTarget2d * API_CALL createRenderTarget2d() = 0;
		virtual IRenderTargetDepth2d * API_CALL createRenderTargetDepth2d() = 0;
		virtual IVertexBuffer * API_CALL createVertexBuffer() = 0;
		
		virtual void API_CALL dispose() = 0;
	};

	/// geometry

	class IQuadTree
	{
	public:
		virtual int API_CALL insert(float minX, float minY, float maxX, float maxY, int userData) = 0;
		virtual void API_CALL remove(int id) = 0;
		virtual void API_CALL query(float minX, float minY, float maxX, float maxY) = 0;
		virtual void API_CALL getQueryResults(void *intBuffer) = 0;
		virtual int API_CALL getQueryResultsCount() = 0;
		virtual int API_CALL getIterationsCount() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class IGeometryFactory
	{
	public:
		virtual IQuadTree * API_CALL createQuadTree() = 0;
		virtual void API_CALL dispose() = 0;
	};

	/// entities

	class IAabb2d
	{
	public:
		virtual void API_CALL update(float minX, float minY, float maxX, float maxY, float zOrder) = 0;
	};

	class IRender2d
	{
	public:
		virtual void API_CALL setMeshesCount(int meshesCount) = 0;
		virtual void API_CALL setMesh(int meshIndex, ITexture *texture, void* vertices, int verticesCount, void* indices, int indicesCount) = 0;
	};

	class IMatrixTransform
	{
	public:
		virtual void API_CALL setTransform(void *floatMatrix) = 0;
	};

	class ITransform2d
	{
	public:
		virtual void API_CALL setWorldPosition(float x, float y, float z) = 0;
		virtual void API_CALL setWorldRotation(float value) = 0;
		virtual void API_CALL setWorldScale(float value) = 0;
		virtual void API_CALL setLocalPivot(float x, float y, float z) = 0;
		virtual void API_CALL setLocalPosition(float x, float y, float z) = 0;
		virtual void API_CALL setLocalRotation(float value) = 0;
		virtual void API_CALL setLocalScale(float value) = 0;
	};

	class IEntity
	{
	public:
		virtual IntPtr getSelfInstance() = 0;		
		virtual void setComponents(ComponentsTypes types) = 0;
		virtual IAabb2d* API_CALL getAabb2d() = 0;
		virtual IRender2d* API_CALL getRender2d() = 0;
		virtual ITransform2d* API_CALL getTransform2d() = 0;
		virtual IMatrixTransform* API_CALL getMatrixTransform() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class ICamera
	{
	public:
		virtual void API_CALL setPosition(float x, float y, float z) = 0;
		virtual void API_CALL setFov(float fov) = 0;
		virtual void API_CALL setAspect(float aspect) = 0;
		virtual void API_CALL setRotation(float rotationZ) = 0;
		virtual void API_CALL setPlanes(float zn, float zf) = 0;
	};

	class IEntitiesWorld
	{
	public:
		virtual ICamera * API_CALL getCamera() = 0;
		virtual void API_CALL setUnpdateBounds(float minX, float minY, float maxX, float maxY) = 0;
		virtual void API_CALL setRenderBounds(float minX, float minY, float maxX, float maxY) = 0;
		
		virtual IEntity * API_CALL createEntity() = 0;
				
		virtual int API_CALL update(float elapsed) = 0;

		virtual void API_CALL updateEntity(IEntity *entity, ComponentsTypes types) = 0;
		virtual void API_CALL addEntity(IEntity *entity) = 0;
		virtual void API_CALL removeEntity(IEntity *entity) = 0;

		virtual void API_CALL dispose() = 0;
		virtual void API_CALL clear() = 0;
	};

	class IEntitiesFactory
	{
	public:
		virtual IEntitiesWorld * API_CALL createEntitiesWorld() = 0;
		virtual void API_CALL dispose() = 0;
	};

	/// drawing
	class IBatch2d
	{
	public:
		virtual IntPtr API_CALL getNativeInstance() = 0;
		virtual void API_CALL addProjection(void *floatMatrix) = 0;
		virtual void API_CALL startBatch() = 0;
		virtual void API_CALL finishBatch() = 0;
		virtual void API_CALL setToneMap(uint tonemapId) = 0;
		virtual void API_CALL draw(GraphicsEffects effect, BlendMode blendMode, ITexture* textureId, uint lightmapId, void *vertices, int verticesCount, void *indices, int indicesCount) = 0;
		virtual void API_CALL execute(bool usePostProcess) = 0;
	};

	/// spine
	
	class ISpineSkeleton
	{
	public:
		virtual void API_CALL setTransform(void *floatMatrix) = 0;
		virtual void API_CALL setColor(uint color) = 0;
		virtual float API_CALL getMinX() = 0;
		virtual float API_CALL getMinY() = 0;
		virtual float API_CALL getMaxX() = 0;
		virtual float API_CALL getMaxY() = 0;
		virtual float API_CALL getZ() = 0;

		virtual void API_CALL render(IBatch2d * batch, int lightmapId) = 0;
		virtual int API_CALL getGeometry(void *vertices, int verticeLimit, void *indices, int indicesLimit) = 0;
		virtual IntPtr API_CALL getNativeInstance() = 0;
		virtual void API_CALL updateWorldTransform() = 0;
		virtual void API_CALL setBonesToSetupPose() = 0;
		virtual void API_CALL setSlotsToSetupPose() = 0;
		virtual void API_CALL dispose() = 0;
	};


	class ISpineAnimation
	{
	public:
		virtual IntPtr API_CALL getNativeInstance() = 0;
		virtual IntPtr API_CALL getName() = 0;
		virtual float API_CALL getDuration() = 0;
	};

	class ISpineAnimationState
	{
	public:
		virtual void API_CALL update(float delta) = 0;
		virtual void API_CALL apply(ISpineSkeleton *skeleton) = 0;
		virtual void API_CALL setAnimation(ISpineAnimation* animation, bool loop, bool normalize) = 0;
		virtual void API_CALL addAnimation(ISpineAnimation* animation, bool loop, float delay) = 0;
		virtual int API_CALL getSpineEventIndices(IntPtr indices, int limit) = 0;
		virtual void API_CALL dispose() = 0;
	};

	class ISpineAnimationStateData
	{
	public:
		virtual void API_CALL setMix(ISpineAnimation *from, ISpineAnimation *to, float time) = 0;
		virtual ISpineAnimationState* API_CALL createState() = 0;
		virtual void API_CALL dispose() = 0;
	};

	class ISpineEvent
	{
	public:
		virtual IntPtr API_CALL getName() = 0;
	};

	class ISpineResource
	{
	public:
		virtual void API_CALL load(String atlasText, String jsonText, String dir) = 0;
		virtual void API_CALL unLoad() = 0;
		virtual ISpineAnimation* API_CALL getSpineAnimation(int i) = 0;
		virtual int API_CALL getSpineAnimationsCount() = 0;
		virtual ISpineEvent* API_CALL getSpineEvent(int i) = 0;
		virtual int API_CALL getSpineEventsCount() = 0;
		virtual ISpineSkeleton* API_CALL createSkeleton(void *floatMatrix) = 0;
		virtual ISpineAnimationStateData* API_CALL createStateData() = 0;
		virtual void API_CALL dispose() = 0;
	};

	/// profile

	class INativeMemoryProfiler
	{
	public:
		virtual int API_CALL getTexturesSize() = 0;
		virtual int API_CALL getAllocationsSize() = 0;
		virtual int API_CALL getHeapSize() = 0;
	};

	/// content
	class ITextureBuffer2d
	{
	public:
		virtual TextureImage2dFormat API_CALL getFormat() = 0;
		virtual int API_CALL getWidth() = 0;
		virtual int API_CALL getHeight() = 0;
		virtual IntPtr API_CALL getPixels() = 0;
	};

	class IContentAtlasMap
	{
	public:
		virtual void API_CALL resetMap() = 0;
		virtual void API_CALL addRect(char *name, int pageIndex, int x, int y, int width, int height) = 0;
		virtual void API_CALL loadTextures() = 0;
	};

	class IContentManager
	{
	public:
		virtual IContentAtlasMap * API_CALL getContentAtlasMap() = 0;
		virtual void API_CALL replaceSeparator(bool value) = 0;
		virtual int API_CALL registerImage(char * name) = 0;
		virtual void API_CALL startLoad() = 0;
		virtual void API_CALL loadImage(int id, ITextureImage2d *textureImage) = 0;
		virtual void API_CALL reloadImages() = 0;
		virtual ITextureBuffer2d * API_CALL loadBuffer(int id) = 0;
		virtual void API_CALL finishLoad() = 0;
		virtual bool API_CALL update() = 0;
		virtual void API_CALL dispose() = 0;
	};
		
	class IObbContentProvider
	{
	public:
		virtual void API_CALL openObbFile(const char *obbFile) = 0;
		virtual void API_CALL closeObbFile() = 0;
		virtual bool API_CALL existsContent(const char *name) = 0;
		virtual void API_CALL openContent(const char *name) = 0;
		virtual int API_CALL read(void *buffer, int bytesLimit) = 0;
		virtual int API_CALL getContentSize() = 0;
		virtual void API_CALL closeContent() = 0;
	};
	
	extern "C" DLLEXPORT IBatch2d * API_CALL createBatch2d();
    
	extern "C" DLLEXPORT ITexture * API_CALL createTextureByID(uint id);
	extern "C" DLLEXPORT IContentManager * API_CALL createContentManager();
	extern "C" DLLEXPORT IObbContentProvider * API_CALL createContentProvider();
	extern "C" DLLEXPORT IEntitiesFactory * API_CALL createEntitiesFactory();
	extern "C" DLLEXPORT IGraphicsFactory * API_CALL createGraphicsFactory();
	extern "C" DLLEXPORT IGeometryFactory * API_CALL createGeometryFactory();

	extern "C" DLLEXPORT void API_CALL initRenderContext();
	extern "C" DLLEXPORT INativeMemoryProfiler * API_CALL createNativeMemoryProfiler();
	extern "C" DLLEXPORT ISpineResource * API_CALL createSpineResource();
    
}

#endif /*LLGE_H*/