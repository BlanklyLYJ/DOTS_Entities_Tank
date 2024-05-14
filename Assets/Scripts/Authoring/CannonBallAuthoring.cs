using Unity.Entities;
using Unity.Rendering;

public class CannonAuthoring : UnityEngine.MonoBehaviour
{
}

class CannonBaker : Baker<CannonAuthoring>
{
    public override void Bake(CannonAuthoring authoring)
    {
        AddComponent<CannonBall>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}