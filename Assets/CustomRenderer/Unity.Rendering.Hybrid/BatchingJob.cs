using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace CustomRenderer.Unity.Rendering.Hybrid
{
    [BurstCompile]
    public struct BatchingJob : IJob, IDisposable
    {
        [WriteOnly] public NativeList<bool> NativeFlipped;
        [WriteOnly] public NativeList<int> NativeEditorRenderDataIndex;
        [WriteOnly] public NativeList<int4> NativeDataArray1;
        
        [ReadOnly] public NativeArray<int> SortedChunkIndices;
        [ReadOnly] public int SharedRenderCount;
        [ReadOnly] public ArchetypeChunkComponentType<RenderMeshFlippedWindingTag> MeshInstanceFlippedTagType;
        [ReadOnly] public ArchetypeChunkSharedComponentType<EditorRenderData> EditorRenderDataType;
        [ReadOnly] public NativeArray<int> SharedRendererCounts;
        [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
        [ReadOnly] public ArchetypeChunkSharedComponentType<RenderMesh> RenderMeshType;

        public void Execute()
        {
            var sortedChunkIndex = 0;

            for (int i = 0; i < SharedRenderCount; i++)
            {
                var startSortedChunkIndex = sortedChunkIndex;
                var endSortedChunkIndex = startSortedChunkIndex + SharedRendererCounts[i];

                while (sortedChunkIndex < endSortedChunkIndex)
                {
                    var chunkIndex = SortedChunkIndices[sortedChunkIndex];
                    var chunk = Chunks[chunkIndex];
                    var rendererSharedComponentIndex = chunk.GetSharedComponentIndex(RenderMeshType);

                    var editorRenderDataIndex = chunk.GetSharedComponentIndex(EditorRenderDataType);

                    var remainingEntitySlots = 1023;
                    var flippedWinding = chunk.Has(MeshInstanceFlippedTagType);

                    int instanceCount = chunk.Count;
                    int startSortedIndex = sortedChunkIndex;
                    int batchChunkCount = 1;

                    remainingEntitySlots -= chunk.Count;
                    sortedChunkIndex++;

                    while (remainingEntitySlots > 0)
                    {
                        if (sortedChunkIndex >= endSortedChunkIndex) break;

                        var nextChunkIndex = SortedChunkIndices[sortedChunkIndex];
                        var nextChunk = Chunks[nextChunkIndex];
                        if (nextChunk.Count > remainingEntitySlots) break;

                        var nextFlippedWinding = nextChunk.Has(MeshInstanceFlippedTagType);
                        if (nextFlippedWinding != flippedWinding) break;

                        #if UNITY_EDITOR
                        if (editorRenderDataIndex !=
                            nextChunk.GetSharedComponentIndex(EditorRenderDataType))
                            break;
                        #endif

                        remainingEntitySlots -= nextChunk.Count;
                        instanceCount += nextChunk.Count;
                        batchChunkCount++;
                        sortedChunkIndex++;
                    }

                    NativeFlipped.Add(flippedWinding);

                    NativeDataArray1.Add(new int4(rendererSharedComponentIndex,
                        startSortedIndex, instanceCount, batchChunkCount));

                    NativeEditorRenderDataIndex.Add(editorRenderDataIndex);
                }
            }
        }

        public void Dispose()
        {
            NativeFlipped.Dispose();
            NativeEditorRenderDataIndex.Dispose();
            NativeDataArray1.Dispose();
        }
    }
}