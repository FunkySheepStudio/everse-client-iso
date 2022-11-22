using UnityEngine;
using Unity.Mathematics;

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

        private void Awake()
        {
            tilePosition.value = Vector2.zero;
            tilePositionRouded.value = lastTilePositionRouded.value = Vector2Int.zero;
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
        }
    }
}
