using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct SpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<TestSpawnData> testSpawnData in SystemAPI.Query<RefRW<TestSpawnData>>())
        {
            if (testSpawnData.ValueRO.spawnedCount >= testSpawnData.ValueRO.spawnCount)
            {
                continue;
            }

            while (SystemAPI.Time.ElapsedTime >= testSpawnData.ValueRO.lastSpawnTime + testSpawnData.ValueRO.randSpawnInterval)
            {
                if (testSpawnData.ValueRO.spawnedCount >= testSpawnData.ValueRO.spawnCount)
                    break;

                var data = testSpawnData.ValueRO;
                data.lastSpawnTime += testSpawnData.ValueRO.randSpawnInterval;
                data.spawnedCount++;

                Entity newEntity = state.EntityManager.Instantiate(data.prefab);
                var randPos = data.spawnPos + (data.randSpawnRange == 0f ? Vector3.zero : Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward * UnityEngine.Random.Range(0f, data.randSpawnRange));
                var randAngle = UnityEngine.Random.Range(0f, 360f);
                state.EntityManager.SetComponentData(newEntity, new LocalTransform() { Position = randPos, Scale = 1f, Rotation = Quaternion.Euler(0f, randAngle, 0f) });
                testSpawnData.ValueRW = data;
            }
        }
    }
}
