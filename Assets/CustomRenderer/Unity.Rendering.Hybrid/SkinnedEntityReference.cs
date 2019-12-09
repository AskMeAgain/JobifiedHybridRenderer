using Unity.Entities;

namespace CustomRenderer.Unity.Rendering.Hybrid
{
    /// <summary>
    /// Used by skinned mesh entities to retrieve the related skinned entities.
    /// </summary>
    public struct SkinnedEntityReference : IComponentData
    {
        public Entity Value;
    }
}
