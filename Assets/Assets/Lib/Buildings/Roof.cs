using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using dousi96.Geometry;
using dousi96.Geometry.Triangulator;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Roof")]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Roof : MonoBehaviour
    {
        public Vector2[] contour;
        MeshFilter filter;

        private void Start()
        {
            filter = GetComponent<MeshFilter>();

            TestEarClippingWithJob();
        }

        private void TestEarClippingWithJob()
        {
            Vector2[][] holes = new Vector2[][]{};

            SinglePolygonData polygon = new SinglePolygonData(Allocator.TempJob, contour, holes);

            int totNumVerts = polygon.HolesNum * 2 + polygon.VerticesNum;
            int ntris = (totNumVerts - 2) * 3;
            NativeArray<int> outTriangles = new NativeArray<int>(ntris, Allocator.TempJob);

            ECTriangulatorJob triangulatorJob = new ECTriangulatorJob()
            {
                Polygon = polygon,
                OutTriangles = outTriangles
            };

            //schedule the jobs
            JobHandle handleTriangulatorJob = triangulatorJob.Schedule();
            handleTriangulatorJob.Complete();

            //get the job results
            Vector3[] vertices = new Vector3[polygon.VerticesNum];
            for (int i = 0; i < polygon.VerticesNum; ++i)
            {
                vertices[i] = new Vector3(polygon[i].x, polygon[i].y, 0f);
            }

            int[] triangles = new int[outTriangles.Length];
            for (int i = 0; i < outTriangles.Length; ++i)
            {
                triangles[i] = outTriangles[i];
            }

            polygon.Dispose();
            outTriangles.Dispose();

            filter.sharedMesh = new Mesh();
            filter.sharedMesh.vertices = vertices;
            filter.sharedMesh.triangles = triangles;

            filter.sharedMesh.RecalculateNormals();

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = filter.sharedMesh;
        }
    }
}
