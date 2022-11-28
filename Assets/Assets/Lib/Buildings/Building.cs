using UnityEngine;
using Unity.Mathematics;
using FunkySheep.Geometry;
using System.Collections.Generic;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Building")]
    public class Building : MonoBehaviour
    {
        public GameObject wallPrefab;
        public List<Texture2D> wallTextures;
        public Material wallMaterial;
        public GameObject roofPrefab;
        public List<Texture2D> roofTextures;
        public float2[] points;
        public float floorHeight;
        public float perimeter;
        public void Create()
        {
            CreateWalls();
            CreateRoof();
        }

        void CreateWalls()
        {
            int wallTextureIndice = UnityEngine.Random.Range(0, wallTextures.Count);
            Texture2D wallTexture = wallTextures[wallTextureIndice];
            wallPrefab.GetComponent<MeshRenderer>().material = wallMaterial;
            wallPrefab.GetComponent<MeshRenderer>().material.SetTexture("_DiffuseTexture", wallTexture);
            wallPrefab.GetComponent<MeshRenderer>().material.SetFloat("_Tilling", perimeter / floorHeight);

            /*Texture2D wallTexture = wallTextures[wallTextureIndice];
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
            }*/
        }

        void CreateRoof()
        {
            int roofTextureIndice = UnityEngine.Random.Range(0, roofTextures.Count);
            Texture2D roofTexture = roofTextures[roofTextureIndice];
            GameObject roofGo = GameObject.Instantiate(roofPrefab, Vector3.up * floorHeight, Quaternion.Euler(90, 0, 0), transform);
            Roof roof = roofGo.AddComponent<Roof>();
            roofGo.GetComponent<MeshRenderer>().material.SetTexture("_DiffuseTexture", roofTexture);
            roof.contour = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                roof.contour[i] = new Vector2(points[i].x, points[i].y);
            }
        }
    }
}
