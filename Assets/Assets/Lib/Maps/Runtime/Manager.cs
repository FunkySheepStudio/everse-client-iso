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
        public FunkySheep.Types.Double initialLatitude;
        public FunkySheep.Types.Double initialLongitude;
        public FunkySheep.Types.Vector2 initialMercatorPosition;
        public FunkySheep.Types.Vector2 initialMapPosition;
        public FunkySheep.Types.Vector2Int initialMapPositionRounded;
        public FunkySheep.Types.Float tileSize;
        public FunkySheep.Events.GameObjectEvent OnPositionCalculated;
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
            CalculateInitialMercatorPosition();
            CalculateInitialPosition();
            CalculateTileSize();
            OnPositionCalculated.Raise(gameObject);
        }

        void CalculateInitialMercatorPosition()
        {
            initialMercatorPosition.value = Utils.toCartesianVector2(initialLongitude.value, initialLatitude.value);
        }

        void CalculateInitialPosition()
        {
            initialMapPosition.value.x = (float)((initialLongitude.value + 180.0) / 360.0 * (1 << zoomLevel.value));
            initialMapPosition.value.y = (float)((1.0 - math.log(math.tan(initialLatitude.value * math.PI / 180.0) + 1.0 / math.cos(initialLatitude.value * math.PI / 180.0)) / math.PI) / 2.0 * (1 << zoomLevel.value));

            initialMapPositionRounded.value = new Vector2Int
            {
                x = (int)math.floor(initialMapPosition.value.x),
                y = (int)math.floor(initialMapPosition.value.y)
            };

            entityManager.AddComponentData<InitialMapPosition>(entity, new InitialMapPosition
            {
                Value = new int2
                {
                    x = initialMapPositionRounded.value.x,
                    y = initialMapPositionRounded.value.y
                }
            });
        }

        void CalculateTileSize()
        {
            tileSize.value = (float)(156543.03 / math.pow(2, zoomLevel.value) * math.cos(math.PI * 2 / 360 * initialLatitude.value) * 256);

            entityManager.AddComponentData<TileSize>(entity, new TileSize
            {
                Value = tileSize.value
            });
        }
    }
}
