using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Player.Position
{
    [AddComponentMenu("FunkySheep/Player/Positions/Set Initial Position")]
    public class SetInitialPosition : MonoBehaviour
    {
        public FunkySheep.Types.Float tileSize;
        public FunkySheep.Types.Vector2 mapPosition;

        private void Start()
        {
            transform.position = new Vector3
            {
                x = (mapPosition.value.x - math.trunc(mapPosition.value.x)) * tileSize.value,
                y = 0,
                z = (1f - (mapPosition.value.y - math.trunc(mapPosition.value.y))) * tileSize.value, // Tile start on top
            };
        }
    }
}
