using FunkySheep.Buildings.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class SetWallsMesh : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                DynamicBuffer<Vertices> vertices = buffer.AddBuffer<Vertices>(entity);
                DynamicBuffer<Triangles> triangles = buffer.AddBuffer<Triangles>(entity);
                DynamicBuffer<Uvs> uvs = buffer.AddBuffer<Uvs>(entity);

                for (int i = 0; i < points.Length; i++)
                {
                    vertices.Add(new Vertices { Value = points[i].ToFloat3() });
                    vertices.Add(new Vertices { Value = points[i].ToFloat3() + new Unity.Mathematics.float3(0, 5, 0) });

                    triangles.Add(new Triangles { Value = i * 2 });
                    triangles.Add(new Triangles { Value = i * 2 + 1 });
                    triangles.Add(new Triangles { Value = i * 2 - 1 });

                    triangles.Add(new Triangles { Value = i * 2 + 1 });
                    triangles.Add(new Triangles { Value = i * 2 - 1 });
                    triangles.Add(new Triangles { Value = i * 2 });
                }
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
