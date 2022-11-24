using FunkySheep.Buildings.Components;
using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Components.Barriers;
using System;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SetBuildingCoordinatesGroup))]
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    public partial class CalculatePointsCenter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points) =>
            {
                float2 center = new float2();
                for (int i = 0; i < points.Length; i++)
                {
                    center += points[i].Value;
                }

                center /= points.Length;

                building.center = center;
                buffer.AddComponent<SetBuildingCoordinatesOver>(entity);
            })
            .WithNone<SetBuildingCoordinatesOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
