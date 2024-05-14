using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WithAll(typeof(Turret))]
// [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
[BurstCompile]
partial struct SafeZoneJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<Shooting> TurretActiveFromEntity;

    public float SquareRadius;

    void Execute(Entity entity, LocalToWorld transform)
    {
        // Debug.Log($"看看 {math.lengthsq(transform.Position)}>{SquareRadius}");
        TurretActiveFromEntity.SetComponentEnabled(entity, math.lengthsq(transform.Position) > SquareRadius);
    }
}


// [UpdateBefore(typeof(TurretShootingSystem))]
// [UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
partial struct SafeZoneSystem : ISystem
{
    private ComponentLookup<Shooting> m_TurretActiveFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_TurretActiveFromEntity = state.GetComponentLookup<Shooting>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
        const float debugRenderStepInDegrees = 20;

        for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees)
        {
            var a = float3.zero;
            var b = float3.zero;
            math.sincos(math.radians(angle), out a.x, out a.z);
            math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
            Debug.DrawLine(a * radius, b * radius);
        }

        m_TurretActiveFromEntity.Update(ref state);
        var safeZoneJob = new SafeZoneJob
        {
            TurretActiveFromEntity = m_TurretActiveFromEntity,
            SquareRadius = radius * radius
        };
        safeZoneJob.ScheduleParallel();
    }
}


// [UpdateBefore(typeof(TurretShootingSystem))]
// [UpdateInGroup(typeof(LateSimulationSystemGroup))]
// partial struct SafeZoneSystem : ISystem
// {
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<Config>();
//     }
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
//
//         // Debug rendering (the white circle).
//         const float debugRenderStepInDegrees = 20;
//         for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees)
//         {
//             var a = float3.zero;
//             var b = float3.zero;
//             math.sincos(math.radians(angle), out a.x, out a.z);
//             math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
//             Debug.DrawLine(a * radius, b * radius);
//         }
//
//         var safeZoneJob = new SafeZoneJob
//         {
//             SquaredRadius = radius * radius
//         };
//         safeZoneJob.ScheduleParallel();
//     }
// }
//
// [WithAll(typeof(Turret))]
// [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
// [BurstCompile]
// partial struct SafeZoneJob : IJobEntity
// {
//     public float SquaredRadius;
//
//     // Because we want the global position of a child entity, we read LocalToWorld instead of LocalTransform.
//     void Execute(in LocalToWorld transformMatrix, EnabledRefRW<Shooting> shootingState)
//     {
//         shootingState.ValueRW = math.lengthsq(transformMatrix.Position) > SquaredRadius;
//     }
// }