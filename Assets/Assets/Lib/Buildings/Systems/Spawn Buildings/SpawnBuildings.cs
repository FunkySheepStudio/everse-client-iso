using FunkySheep.Buildings.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class SpawnBuildings : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void Spawn(Transform root, GameObject prefabs)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    GameObject buildingGo = GameObject.Instantiate(prefabs, root);
                    buildingGo.transform.localPosition = new Vector3
                    (
                        points[i].Value.x,
                        0,
                        points[i].Value.y
                    );
                }

                buffer.AddComponent<SpawnBuildingOver>(entity);
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
