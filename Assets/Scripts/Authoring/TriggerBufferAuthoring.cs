using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TriggerBufferAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<TriggerBuffer>(entity);
    }
}
