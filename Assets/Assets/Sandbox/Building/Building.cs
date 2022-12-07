using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum WaveType : int
{
    None = 0,
    Sin = 1,
    Cos = 2,
    Random = 3
}

public class Building : MonoBehaviour
{
    [Range(1f, 50f)]
    public int stagesCount = 5;
    public float stagesSize = 2.5f;
    public List<Transform> floorPoints = new List<Transform>();
    [Range(1, 10)]
    public int GenerationWaitTime = 3;
    float nextGenerateTime = 0f;
    [Range(0f, 20f)]
    public float wave = 0;
    public WaveType waveType = WaveType.None;

    private void OnDrawGizmos()
    {
        if (Time.realtimeSinceStartup >= nextGenerateTime)
        {
            Vector3[,] structure = CreateStructure();
            JoinOddPoints(structure, wave, waveType);
            RenderBuilding(structure);
            nextGenerateTime = Time.realtimeSinceStartup + GenerationWaitTime;
        }
    }

    public Vector3[,] CreateStructure()
    {
        Vector3[,] points = new Vector3[stagesCount, floorPoints.Count];

        for (int i = 0; i < stagesCount; i++)
        {
            for (int j = 0; j < floorPoints.Count; j++)
            {
                points[i, j] = floorPoints[j].position + Vector3.up * i * stagesSize;
            }
        }

        return points;
    }

    public void RenderBuilding(Vector3[,] structure)
    {
        for (int i = 0; i < stagesCount; i++)
        {
            for (int j = 0; j < floorPoints.Count; j++)
            {
                Debug.DrawLine(structure[i, j], structure[i, (j + 1) % floorPoints.Count], Color.red, GenerationWaitTime);

                if (i != stagesCount - 1)
                {
                    Debug.DrawLine(structure[i, j], structure[i + 1, j], Color.red, GenerationWaitTime);
                }
            }
        }
    }

    public void Join2Points(Vector3[,] structure, int joinIndex = 0, float wave = 0f, WaveType waveType = WaveType.None)
    {
        for (int i = 0; i < stagesCount; i++)
        {
            float stageRatio = (float)i / (float)stagesCount;

            if (waveType == WaveType.Cos)
            {
                stageRatio = Mathf.Cos(stageRatio * wave);
            }

            if (waveType == WaveType.Sin)
            {
                stageRatio = Mathf.Sin(stageRatio * wave);
            }

            Vector3 centerPoint = (structure[i, joinIndex] + structure[i, (joinIndex + 1) % floorPoints.Count]) / 2;

            structure[i, joinIndex] = Vector3.Lerp(structure[i, joinIndex], centerPoint, stageRatio);
            structure[i, (joinIndex + 1) % floorPoints.Count] = Vector3.Lerp(structure[i, (joinIndex + 1) % floorPoints.Count], centerPoint, stageRatio);
        }
    }

    public void JoinOddPoints(Vector3[,] structure, float wave = 0f, WaveType waveType = WaveType.None)
    {
        for (int i = 0; i < floorPoints.Count; i+=2)
        {
            if (waveType == WaveType.Random)
            {
                wave = Random.Range(0, 20);
                Join2Points(structure, i, wave, (WaveType)Random.Range(0, 2));
            } else
            {
                Join2Points(structure, i, wave, waveType);
            }
        }
    }
}
