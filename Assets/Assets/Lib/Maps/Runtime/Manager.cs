using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using FunkySheep.Maps.Components;
using static UnityEngine.EventSystems.EventTrigger;

namespace FunkySheep.Maps
{
    [AddComponentMenu("FunkySheep/Maps/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.Int32 zoomLevel;
        public FunkySheep.Types.Double latitude;
        public FunkySheep.Types.Double longitude;
        public FunkySheep.Types.Vector2 mapPosition;
        public FunkySheep.Types.Vector2Int mapPositionRounded;
        public FunkySheep.Events.GameObjectEvent OnPositionCalculated;
        public FunkySheep.Types.Float tileSize;
        EntityManager entityManager;
        Entity entity;

        private void Awake()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entity = entityManager.CreateEntity();
            entityManager.AddComponentData<ZoomLevel>(entity, new ZoomLevel
            {
                Value = zoomLevel.value
            });
        }

        private void Start()
        {
            CalculatePosition();
            CalculateSize();
            OnPositionCalculated.Raise(gameObject);
        }

        void CalculatePosition()
        {
            mapPosition.value.x = (float)((longitude.value + 180.0) / 360.0 * (1 << zoomLevel.value));
            mapPosition.value.y = (float)((1.0 - math.log(math.tan(latitude.value * math.PI / 180.0) + 1.0 / math.cos(latitude.value * math.PI / 180.0)) / math.PI) / 2.0 * (1 << zoomLevel.value));

            mapPositionRounded.value = new Vector2Int
            {
                x = (int)math.floor(mapPosition.value.x),
                y = (int)math.floor(mapPosition.value.y)
            };

            entityManager.AddComponentData<InitialMapPosition>(entity, new InitialMapPosition
            {
                Value = new int2
                {
                    x = mapPositionRounded.value.x,
                    y = mapPositionRounded.value.y
                }
            });
        }

        void CalculateSize()
        {
            tileSize.value = (float)(156543.03 / math.pow(2, zoomLevel.value) * math.cos(math.PI * 2 / 360 * latitude.value) * 256);

            entityManager.AddComponentData<TileSize>(entity, new TileSize
            {
                Value = tileSize.value
            });
        }
    }
}
