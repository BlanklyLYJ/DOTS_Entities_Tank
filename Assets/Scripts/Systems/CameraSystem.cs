using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial class CameraSystem : SystemBase
{
    Entity Target;
    Unity.Mathematics.Random Random;
    EntityQuery TanksQuery;

    protected override void OnCreate()
    {
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        TanksQuery = GetEntityQuery(typeof(Tank));
        RequireForUpdate(TanksQuery);
    }

    protected override void OnUpdate()
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var tanks = TanksQuery.ToEntityArray(Allocator.Temp);
            Target = tanks[Random.NextInt(tanks.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var tankTransform = GetComponent<LocalToWorld>(Target);
        cameraTransform.position = tankTransform.Position - 10.0f * tankTransform.Forward + new float3(0, 5f, 0);
        cameraTransform.LookAt(tankTransform.Position);
    }
}