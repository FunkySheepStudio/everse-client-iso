using FunkySheep.Geometry.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Roads.Components.Tags;
using Unity.Mathematics;

namespace FunkySheep.Roads.Systems
{
    [UpdateInGroup(typeof(RoadsGroup))]
    [UpdateAfter(typeof(CheckPlayerPosition))]
    public partial class SpawnRoads : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void Spawn(Transform root, GameObject prefab, GameObject Connector)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Road road, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                for (int i = 0; i < points.Length - 1; i++)
                {

                    GameObject roadGo = GameObject.Instantiate(prefab, root);

                    Quaternion LookAtRotation = Quaternion.identity;
                    float3 relativePos = points[i].ToFloat3() - points[i + 1].ToFloat3();
                    if (!relativePos.Equals(float3.zero))
                        LookAtRotation = Quaternion.LookRotation(relativePos);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);

                    roadGo.transform.rotation = LookAtRotationOnly_Y;
                    roadGo.transform.position = points[i].ToFloat3() + new float3(0, 0.001f, 0);
                    roadGo.transform.localScale = new Vector3(
                        roadGo.transform.localScale.x,
                        roadGo.transform.localScale.y,
                        math.distance(points[i].ToFloat3(), points[i + 1].ToFloat3())
                    );

                    GameObject roadConnector = GameObject.Instantiate(Connector, root);
                    roadConnector.transform.position = roadGo.transform.position;
                    roadConnector.transform.rotation = LookAtRotationOnly_Y;
                }

                buffer.DestroyEntity(entity);
                
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
