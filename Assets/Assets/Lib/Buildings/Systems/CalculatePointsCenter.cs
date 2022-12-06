using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(Osm.Systems.OsmGroup))]
    public partial class CalculatePointsCenter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Components.Building building, in DynamicBuffer<Points> points) =>
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
