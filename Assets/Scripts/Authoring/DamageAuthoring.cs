using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class DamageAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float m_damage = default;

    [SerializeField]
    private PhysicsCategory[] m_targetedPhysicsCategories = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Build the damage mask
        uint physicCategoryMask = 0;
        for (int i = 0; i < m_targetedPhysicsCategories.Length; ++i)
        {
            physicCategoryMask |= (uint)m_targetedPhysicsCategories[i];
        }

        // Add the damage component
        dstManager.AddComponentData(entity, new Damage
        {
            value = m_damage,
            physicsCategoryMask = physicCategoryMask,
        });
    }
}
