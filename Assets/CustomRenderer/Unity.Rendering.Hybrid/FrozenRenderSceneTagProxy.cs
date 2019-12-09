using System;
using Unity.Entities;

namespace CustomRenderer.Unity.Rendering.Hybrid
{
    [Serializable]
    public struct FrozenRenderSceneTag : ISharedComponentData, IEquatable<FrozenRenderSceneTag>
    {
        public Hash128          SceneGUID;
        public int              SectionIndex;
        public int              HasStreamedLOD;

        public bool Equals(FrozenRenderSceneTag other)
        {
            return SceneGUID == other.SceneGUID && SectionIndex == other.SectionIndex;
        }

        public override int GetHashCode()
        {
            return SceneGUID.GetHashCode() ^ SectionIndex;
        }
    }

    [UnityEngine.AddComponentMenu("")]
    public class FrozenRenderSceneTagProxy : SharedComponentDataProxy<FrozenRenderSceneTag>
    {
    }
}
