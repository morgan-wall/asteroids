using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class DamageAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float m_damage = default;

    [SerializeField]
    private PhysicsCategory[] m_targetedPhysicCategories = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Build the damage mask
        uint physicCategoryMask = 0;
        for (int i = 0; i < m_targetedPhysicCategories.Length; ++i)
        {
            physicCategoryMask |= (uint)m_targetedPhysicCategories[i];
        }

        // Add the damage component
        var damage = new Damage
        {
            value = m_damage,
            physicCategoryMask = physicCategoryMask,
        };
        dstManager.AddComponentData(entity, damage);
    }
}
