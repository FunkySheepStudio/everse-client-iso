using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class CreateStructure : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Building building, in DynamicBuffer<Points> points) =>
            {
                DynamicBuffer<Vertices> vertices = buffer.AddBuffer<Vertices>(entity);
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < points.Length; j++)
                    {
                        vertices.Add(new Vertices
                        {
                            Value = new float3
                            {
                                x = points[j].Value.x,
                                y = i * building.floorHeight,
                                z = points[j].Value.y
                            }
                        });
                    }
                }
                buffer.RemoveComponent<Points>(entity);
            })
            .WithNone<Components.Barriers.SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }

        public void Debug()
        {
            Entities.ForEach((in Components.Building building, in DynamicBuffer<Vertices> vertices) =>
            {
                int nodeCount = vertices.Length / 10;
                for (int i = 0; i < nodeCount; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        int index = (j * nodeCount) + i;
                        int nextIndex = (j * nodeCount) + (i + 1) % nodeCount;

                        UnityEngine.Debug.DrawLine(vertices[index].Value, vertices[nextIndex].Value);
                    }
                }
            })
            .WithoutBurst()
            .Run();
        }
    }
}
