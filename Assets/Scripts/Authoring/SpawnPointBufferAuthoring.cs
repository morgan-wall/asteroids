using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SpawnPointBufferAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<SpawnPointBuffer>(entity);
    }
}
