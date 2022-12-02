using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

namespace FunkySheep.Player
{
    [AddComponentMenu("FunkySheep/Player/Positions/Calculate Position")]
    public class CalculatePositions : MonoBehaviour
    {
        public FunkySheep.Types.Float tileSize;
        public FunkySheep.Types.Vector2 tilePosition;
        public FunkySheep.Types.Vector2Int tilePositionRouded;
        public FunkySheep.Types.Vector2Int lastTilePositionRouded;
        public FunkySheep.Events.Vector2IntEvent OnTilePositionChanged;
        public FunkySheep.Events.Vector2Event OnLongPositionChanged;
        float2 position2D = new float2();
        float2 lastPosition2D = new float2();
        FunkySheep.Buildings.Systems.CheckPlayerPosition playerBuildingPositionSystem;
        FunkySheep.Roads.Systems.CheckPlayerPosition playerRoadsPositionSystem;

        private void Awake()
        {
            tilePosition.value = Vector2.zero;
            tilePositionRouded.value = lastTilePositionRouded.value = Vector2Int.zero;
            playerBuildingPositionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FunkySheep.Buildings.Systems.CheckPlayerPosition>();
            playerRoadsPositionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FunkySheep.Roads.Systems.CheckPlayerPosition>();
        }

        private void Update()
        {
            tilePosition.value = tilePosition.value = new Vector2(
                transform.position.x / tileSize.value,
                transform.position.z / tileSize.value
            );

            tilePositionRouded.value = new Vector2Int(
                (int)math.floor(tilePosition.value.x),
                (int)math.floor(tilePosition.value.y)
            );

            if (tilePositionRouded.value != lastTilePositionRouded.value)
            {
                OnTilePositionChanged.Raise(tilePositionRouded.value - lastTilePositionRouded.value);
                lastTilePositionRouded.value = tilePositionRouded.value;
            }

            position2D = new float2
            {
                x = transform.position.x,
                y = transform.position.z
            };

            if (math.distance(position2D, lastPosition2D) > 10)
            {
                playerBuildingPositionSystem.CheckBuildingNearPlayer(position2D);
                playerRoadsPositionSystem.CheckRoadsNearPlayer(position2D);
                OnLongPositionChanged.Raise(position2D);
                lastPosition2D = position2D;
            }
        }
    }
}
