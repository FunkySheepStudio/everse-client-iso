using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(SetBuildingCoordinatesGroup))]
    public class SpawnBuildingGroup : ComponentSystemGroup
    {
    }
}

