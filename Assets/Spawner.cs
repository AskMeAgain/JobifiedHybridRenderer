using System.Collections;
using System.Collections.Generic;
using CustomRenderer.Unity.Rendering.Hybrid;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnerSystem : ComponentSystem
{

    public GameObject Cube;
    public Material material;

    protected override void OnCreate()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Cube = Resources.Load<GameObject>("Cube");
        
        var _cube = GameObjectConversionUtility.ConvertGameObjectHierarchy(Cube,settings);
        
        var mesh = Cube.GetComponent<MeshFilter>().sharedMesh;

        var rMesh = new RenderMesh()
        {
            mesh = mesh,
            material = Resources.Load<Material>("Material"),
            receiveShadows = true,
            castShadows = UnityEngine.Rendering.ShadowCastingMode.On
        };


        for (int i = 0; i < 100; i++)
        {
            for (int ii = 0; ii < 100; ii++)
            {
                var entity = manager.Instantiate(_cube);
                manager.AddComponentData(entity, new Translation() {Value = new float3(i * 2, 0, ii * 2)});
                manager.AddComponentData(entity, new LocalToWorld());
                manager.AddSharedComponentData(entity, rMesh);

            }
        }
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation position) =>
        {
            position.Value.y += 1 * Time.DeltaTime;

            if (position.Value.y > 10)
            {
                position.Value.y = -10;
            }
        });
    }
}