using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class FreezePositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private bool x = default;

    [SerializeField]
    private bool y = default;

    [SerializeField]
    private bool z = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Determine the freeze position
        float3 position = float3.zero;
        if (dstManager.HasComponent<Translation>(entity))
        {
            position = dstManager.GetComponentData<Translation>(entity).Value;
        }

        // Setup the component
        dstManager.AddComponentData(entity, new FreezePosition
        {
            x = x,
            y = y,
            z = z,
            position = position,
        });
    }
}
