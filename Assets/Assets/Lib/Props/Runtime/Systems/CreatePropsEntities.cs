using Unity.Entities;
using Unity.Transforms;
using FunkySheep.Images.Components;
using FunkySheep.Props.Components;
using FunkySheep.Maps.Components;
using Unity.Mathematics;

namespace FunkySheep.Props.Systems
{
    [UpdateInGroup(typeof(PropsGroup))]
    public partial class CreatePropsEntities : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Pixels> pixels, in TileData tileData) =>
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    Entity tree = buffer.CreateEntity();
                    buffer.AddComponent<LocalTransform>(tree);
                    buffer.SetComponent(tree, new LocalTransform
                    {
                        Position = new Unity.Mathematics.float3
                        {
                            x = tileData.start.x + ((tileData.end.x - tileData.start.x) / 256 * (i % 256)),
                            y = 0,
                            z = tileData.start.y + ((tileData.end.y - tileData.start.y) / 256 * (i / 256))
                        }
                    });
                    buffer.AddSharedComponent(entity, new TilePosition { Value = tileData.tilePosition });

                    buffer.AddComponent<TileData>(tree);
                    buffer.SetComponent(tree, new TileData
                    {
                        size = tileData.size,
                        color = new int3 { x = pixels[i].Value.g, y = pixels[i].Value.b, z = pixels[i].Value.a }
                    });
                }

                buffer.DestroyEntity(entity);
            })
           .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
           .ScheduleParallel();
        }
    }

}
