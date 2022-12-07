using Unity.Entities;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components.Barriers;

namespace FunkySheep.Buildings.Systems
{
    [UpdateBefore(typeof(Osm.Systems.OsmGroup))]
    public partial class RemoveCollinearPoints : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Components.Building building) =>
            {
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (Geometry.Utils.IsCollinear(
                        gPSCoordinates[i].Value,
                        gPSCoordinates[(i + 1) % gPSCoordinates.Length].Value,
                        gPSCoordinates[(i + 2) % gPSCoordinates.Length].Value
                        ))
                    {
                        gPSCoordinates.RemoveAt((i + 1) % gPSCoordinates.Length);
                    }
                }

                if (gPSCoordinates[0].Value.Equals(gPSCoordinates[gPSCoordinates.Length - 1].Value))
                {
                    gPSCoordinates.RemoveAt(gPSCoordinates.Length - 1);
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
