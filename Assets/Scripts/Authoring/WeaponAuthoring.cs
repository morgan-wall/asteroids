using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    private GameObject m_projectilePrefab = default;

    [SerializeField]
    private float muzzleOffset = default;
    
    [SerializeField]
    private float3 muzzleDirection = default;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_projectilePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Weapon
        {
            fire = false,
            projectilePrefab = conversionSystem.GetPrimaryEntity(m_projectilePrefab),
            muzzleOffset = muzzleOffset,
            muzzleDirection = muzzleDirection,
        });
    }
}
