using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

// [WithAll(typeof(Shooting))]
// [UpdateInGroup(typeof(LateSimulationSystemGroup))]
// partial struct TurretShootingSystem : ISystem
// {
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         
//     }
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         foreach (var (turret, localToWorld) in
//                  SystemAPI.Query<TurretAspect, RefRO<LocalToWorld>>()
//                      )
//         {
//             Entity instance = state.EntityManager.Instantiate(turret.CannonBallPrefab);
//
//             state.EntityManager.SetComponentData(instance, new LocalTransform
//             {
//                 Position = SystemAPI.GetComponent<LocalToWorld>(turret.CannonBallSpawn).Position,
//                 Rotation = quaternion.identity,
//                 Scale = SystemAPI.GetComponent<LocalTransform>(turret.CannonBallPrefab).Scale
//             });
//
//             state.EntityManager.SetComponentData(instance, new CannonBall
//             {
//                 Speed = localToWorld.ValueRO.Up * 20.0f
//             });
//             
//             state.EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor
//             {
//                 Value = turret.Color
//             });
//             
//         }
//     }
// }


partial struct TurretShootingSystem : ISystem
{
    private ComponentLookup<LocalToWorld> m_LocalToWorldTransformFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalToWorldTransformFromEntity.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var turretShootJob = new TurretShoot
        {
            LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
            ECB = ecb
        };
        turretShootJob.Schedule();
    }
}

[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct TurretShoot : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;

    void Execute(TurretAspect turret)
    {
        var instance = ECB.Instantiate(turret.CannonBallPrefab);
        var spawnLocalToWorld = LocalToWorldTransformFromEntity[turret.CannonBallSpawn];
        var cannonBallTransform = LocalTransform.FromMatrix(spawnLocalToWorld.Value);
        cannonBallTransform.Scale = 1;

        ECB.SetComponent(instance, cannonBallTransform);
        float4x4 val = LocalToWorldTransformFromEntity[turret.CannonBallPrefab].Value;
        ECB.AddComponent(instance, new PostTransformMatrix
        {
            Value = val
        });
        ECB.SetComponent(instance, new CannonBall
        {
            Speed = spawnLocalToWorld.Value.Forward() * 20f
        });
        ECB.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = turret.Color });
    }
}