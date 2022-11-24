using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using FunkySheep.Buildings.Systems;

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
        CheckPlayerPosition playerPositionSystem;

        private void Awake()
        {
            tilePosition.value = Vector2.zero;
            tilePositionRouded.value = lastTilePositionRouded.value = Vector2Int.zero;
            playerPositionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CheckPlayerPosition>();
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
                playerPositionSystem.CheckBuildingNearPlayer(position2D);
                OnLongPositionChanged.Raise(position2D);
                lastPosition2D = position2D;
            }
        }
    }
}
