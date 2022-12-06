using Unity.Entities;
using Unity.Collections;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsOrder))]
    public partial class CalculatePointsCounterClockwise : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Points> points, in Components.Building building) =>
            {
                bool result = Geometry.Utils.IsTriangleOrientedClockwise(
                    points[points.Length - 1].Value,
                    points[0].Value,
                    points[1].Value
                );

                if (result)
                {
                    NativeArray<Points> tempPoints = new NativeArray<Points>(points.Length, Allocator.Temp);
                    tempPoints.CopyFrom(points.AsNativeArray());
                    points.Clear();

                    for (int i = tempPoints.Length - 1; i >= 0; i--)
                    {
                        points.Add(tempPoints[i]);
                    }
                }
            })
            .WithNone<Components.Barriers.SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
