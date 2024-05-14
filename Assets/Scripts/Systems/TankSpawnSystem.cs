using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

[BurstCompile]
partial struct TankSpawnSystem : ISystem
{
    private EntityQuery m_BaseColorQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // entity阵列
        var vehicles = CollectionHelper.CreateNativeArray<Entity>(config.TankCount, Allocator.Temp);
        ecb.Instantiate(config.TankPrefab, vehicles);


        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        foreach (var vehicle in vehicles)
        {
            ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor());
        }


        state.Enabled = false;
    }
}