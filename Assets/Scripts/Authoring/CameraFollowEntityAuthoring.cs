using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CameraFollowEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        var cameraFollowEntity = camera.GetComponent<FollowEntity>();
        if (cameraFollowEntity == null)
        {
            return;
        }

        cameraFollowEntity.EntityToFollow = entity;
    }
}
