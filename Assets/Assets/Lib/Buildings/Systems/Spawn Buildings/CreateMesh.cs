using FunkySheep.Buildings.Components.Tags;
using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Geometry.Components;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class CreateMesh : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Building building, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                DynamicBuffer<Vertices> vertices = buffer.AddBuffer<Vertices>(entity);
                DynamicBuffer<Triangles> triangles = buffer.AddBuffer<Triangles>(entity);
                DynamicBuffer<Uvs> uvs = buffer.AddBuffer<Uvs>(entity);
                float startUv = 0;

                for (int i = 0; i < points.Length; i++)
                {
                    vertices.Add(new Vertices { Value = points[i].ToFloat3() });
                    vertices.Add(new Vertices { Value = points[i].ToFloat3() + new float3(0, building.floorHeight, 0) });

                    if (i != points.Length - 1)
                    {
                        int vertice0 = (i * 2);
                        int vertice1 = ((i * 2) + 2);
                        int vertice2 = ((i * 2) + 3);
                        int vertice3 = ((i * 2) + 1);

                        triangles.Add(new Triangles { Value = vertice0 });
                        triangles.Add(new Triangles { Value = vertice2 });
                        triangles.Add(new Triangles { Value = vertice1 });

                        triangles.Add(new Triangles { Value = vertice0 });
                        triangles.Add(new Triangles { Value = vertice3 });
                        triangles.Add(new Triangles { Value = vertice2 });
                    }

                    float uvRatio = 1 / (building.perimeter / math.distance(points[i].Value, points[(i + 1) % points.Length].Value));

                    uvs.Add(new Uvs { Value = new float2 { x = startUv, y = 0 } });
                    uvs.Add(new Uvs { Value = new float2 { x = startUv, y = 1 } });

                    startUv += uvRatio;
                }

                // Last wall
                vertices.Add(new Vertices { Value = points[0].ToFloat3() });
                vertices.Add(new Vertices { Value = points[0].ToFloat3() + new float3(0, building.floorHeight, 0) });

                triangles.Add(new Triangles { Value = vertices.Length - 4 });
                triangles.Add(new Triangles { Value = vertices.Length - 1 });
                triangles.Add(new Triangles { Value = vertices.Length - 2 });

                triangles.Add(new Triangles { Value = vertices.Length - 4 });
                triangles.Add(new Triangles { Value = vertices.Length - 3 });
                triangles.Add(new Triangles { Value = vertices.Length - 1 });

                uvs.Add(new Uvs { Value = new float2 { x = 1, y = 0 } });
                uvs.Add(new Uvs { Value = new float2 { x = 1, y = 1 } });



            }).WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
