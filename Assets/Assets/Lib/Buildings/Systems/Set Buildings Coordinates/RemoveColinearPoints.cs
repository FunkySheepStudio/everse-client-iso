using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components.Barriers;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SetBuildingCoordinatesGroup))]
    public partial class RemoveCollinearPoints : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building) =>
            {
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (Geometry.utils.IsCollinear(
                        gPSCoordinates[i].Value,
                        gPSCoordinates[(i + 1) % gPSCoordinates.Length].Value,
                        gPSCoordinates[(i + 2) % gPSCoordinates.Length].Value
                        ))
                    {
                        gPSCoordinates.RemoveAt((i + 1) % gPSCoordinates.Length);
                    }
                }

                if (gPSCoordinates.Length < 3)
                {
                    buffer.DestroyEntity(entity);
                    return;
                }
            })
            .WithNone<SetBuildingCoordinatesOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
