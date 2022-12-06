using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsCenter))]
    public class SpawnBuildingGroup : ComponentSystemGroup
    {
    }
}

