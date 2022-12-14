using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Types;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Systems;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Spawner")]
    public class Spawner : MonoBehaviour
    {
        public GameObject prefab;
        SpawnBuildings spawnBuildings;
        RandomizeStructure randomizeStructure;

        private void Awake()
        {
            spawnBuildings = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SpawnBuildings>();
            randomizeStructure = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RandomizeStructure>();
        }

        private void Update()
        {
            //spawnBuildings.Spawn(transform, prefab);
            randomizeStructure.Debug();
        }
    }
}
