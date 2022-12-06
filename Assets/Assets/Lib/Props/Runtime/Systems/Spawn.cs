using FunkySheep.Props.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FunkySheep.Props.Systems
{
    [UpdateInGroup(typeof(SpawnPropsGroup))]
    [UpdateAfter(typeof(CheckPlayerPosition))]
    public partial class SpawnSystem : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void Spawn(Transform parent, GameObject prefab)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in LocalTransform localTransform, in Components.Tags.Spawn spawn, in TileData tileData) =>
            {
                GameObject prop = GameObject.Instantiate(prefab, parent);
                prop.transform.position = localTransform.Position;
                prop.transform.localScale = new Vector3(tileData.size, tileData.size, tileData.size);
                prop.GetComponent<MeshRenderer>().material.color = new Color((float)tileData.color.x / 256, (float)tileData.color.y / 256, (float)tileData.color.z / 256 );
                buffer.DestroyEntity(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
            this.CompleteDependency();
        }
    }

}
