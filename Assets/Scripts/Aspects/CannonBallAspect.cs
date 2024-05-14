using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
readonly partial struct CannonBallAspect : IAspect
{
    public readonly Entity self;

    readonly RefRW<LocalTransform> Transform;

    readonly RefRW<CannonBall> CannonBall;


    public float3 Position
    {
        get => Transform.ValueRO.Position;
        set => Transform.ValueRW.Position = value;
    }

    public float3 Speed
    {
        get => CannonBall.ValueRO.Speed;
        set => CannonBall.ValueRW.Speed = value;
    }
}