using Unity.Entities;
using Unity.Mathematics;

namespace CustomRenderer.Unity.Rendering.Hybrid
{
    public struct RenderBounds : IComponentData
    {
        public AABB Value;
    }

    public struct WorldRenderBounds : IComponentData
    {
        public AABB Value;
    }
    
    public struct ChunkWorldRenderBounds : IComponentData
    {
        public AABB Value;
    }
}