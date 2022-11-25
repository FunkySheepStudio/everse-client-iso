using UnityEngine;
using Unity.Mathematics;
using FunkySheep.Geometry;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Building")]
    public class Building : MonoBehaviour
    {
        public GameObject wallPrefab;
        public GameObject roofPrefab;
        public float2[] points;
        public float floorHeight;

        private void Awake()
        {
            floorHeight = UnityEngine.Random.Range(5, 10);
        }

        private void Start()
        {
            CreateWalls();
            CreateRoof();
        }

        void CreateWalls()
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = new Vector3(
                    points[i].x,
                    0,
                    points[i].y
                );

                Vector3 nextPoint = new Vector3(
                    points[Utils.ClampListIndex(i + 1, points.Length)].x,
                    0,
                    points[Utils.ClampListIndex(i + 1, points.Length)].y
                );

                Vector3 middlePoint = (point + nextPoint) / 2;

                //Width
                // Wall width
                float wallWidth = math.distance(
                    point,
                    nextPoint
                );

                // Rotation
                Quaternion LookAtRotation = Quaternion.identity;
                Quaternion LookAtRotationOnly_Y;
                float3 relativePos = point - nextPoint;
                if (!relativePos.Equals(float3.zero))
                    LookAtRotation = Quaternion.LookRotation(relativePos);
                LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);

                GameObject wallGo = GameObject.Instantiate(wallPrefab, middlePoint, LookAtRotationOnly_Y, transform);
                wallGo.AddComponent<MeshCollider>();
                wallGo.transform.localScale = new Vector3(1, floorHeight, wallWidth);
            }
        }

        void CreateRoof()
        {
            GameObject roofGo = GameObject.Instantiate(roofPrefab, Vector3.up * floorHeight, Quaternion.Euler(90, 0, 0), transform);
            Roof roof = roofGo.AddComponent<Roof>();
            roof.contour = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                roof.contour[i] = new Vector2(points[i].x, points[i].y);
            }
        }
    }
}
