using FunkySheep.Roads.Components;
using FunkySheep.Roads.Components.Tags;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Roads.Systems
{
    [UpdateInGroup(typeof(SetRoadsCoordinatesGroup))]
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    public partial class CheckPlayerPosition : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void CheckRoadsNearPlayer(float2 position)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Road road, in DynamicBuffer<Points> points) =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (math.distance(position, points[i].Value) < 500)
                    {
                        buffer.AddComponent<Spawn>(entity);
                        return;
                    }
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithNone<Spawn>()
            .ScheduleParallel();
        }
    }

}
