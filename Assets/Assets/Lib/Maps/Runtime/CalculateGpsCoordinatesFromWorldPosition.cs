using UnityEngine;
using Unity.Mathematics;
using TreeEditor;

namespace FunkySheep.Maps
{
    [AddComponentMenu("FunkySheep/Maps/Get Gps Coordinates")]
    public class CalculateGpsCoordinatesFromWorldPosition : MonoBehaviour
    {
        public FunkySheep.Types.Vector2 initialMercatorPosition;
        public FunkySheep.Types.Double initialLatitude;
        public FunkySheep.Types.Double initialLongitude;

        public FunkySheep.Types.Double calculatedLatitude;
        public FunkySheep.Types.Double calculatedLongitude;

        public float distanceEventTrigger;
        Vector3 lastPosition;
        public FunkySheep.Events.SimpleEvent onGpsCoordinatesChanged;

        private void Update()
        {
            var calculatedGPS = FunkySheep.Maps.Utils.toGeoCoord(
                    new Vector2(
                        initialMercatorPosition.value.x + transform.position.x / Mathf.Cos(Mathf.Deg2Rad * (float)initialLatitude.value),
                        initialMercatorPosition.value.y + transform.position.z / Mathf.Cos(Mathf.Deg2Rad * (float)initialLongitude.value)
                        )
                );
            calculatedLatitude.value = calculatedGPS.latitude;
            calculatedLongitude.value = calculatedGPS.longitude;

            if (math.distance(lastPosition, transform.position) >= distanceEventTrigger)
            {
                onGpsCoordinatesChanged.Raise();
                lastPosition = transform.position;
            }

        }
    }
}
