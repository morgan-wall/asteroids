using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    private GameObject m_projectilePrefab = default;

    [SerializeField]
    private float3 muzzleOffset = default;
    
    [SerializeField]
    private float3 muzzleDirection = default;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_projectilePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var weapon = new Weapon
        {
            fire = false,
            projectilePrefab = conversionSystem.GetPrimaryEntity(m_projectilePrefab),
            muzzleOffset = muzzleOffset,
            muzzleDirection = muzzleDirection,
        };
        dstManager.AddComponentData(entity, weapon);
    }
}
