using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public partial struct TestSpawnData : IComponentData
{
    //Input Data
    public Entity prefab;
    public Vector3 spawnPos;
    public int spawnCount;
    public float randSpawnRange;
    public float randSpawnInterval;

    //Manage Data
    public int spawnedCount;
    public float lastSpawnTime;
}

public class TestSpawnAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 spawnPos;
    public int spawnCount;
    public float randSpawnRange;
    public float randSpawnInterval;
}

public partial class TestSpawnBaker : Baker<TestSpawnAuthoring>
{
    public override void Bake(TestSpawnAuthoring authoring)
    {
        AddComponent(new TestSpawnData()
        {
            prefab = GetEntity(authoring.prefab),
            spawnPos = authoring.spawnPos,
            spawnCount = authoring.spawnCount,
            randSpawnRange = authoring.randSpawnRange,
            randSpawnInterval = authoring.randSpawnInterval,
        });
    }
}