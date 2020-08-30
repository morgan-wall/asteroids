using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    private GameObject m_prefab = default;

    [SerializeField]
    private float m_maxOffset = default;
    
    [SerializeField]
    private float m_timeUntilNextSpawn = default;
    
    [SerializeField]
    private float m_minTimeBetweenSpawns = default;
    
    [SerializeField]
    private float m_maxTimeBetweenSpawns = default;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            prefab = conversionSystem.GetPrimaryEntity(m_prefab),
            maxOffset = m_maxOffset,
            timeUntilNextSpawn = m_timeUntilNextSpawn,
            minTimeBetweenSpawns = m_minTimeBetweenSpawns,
            maxTimeBetweenSpawns = m_maxTimeBetweenSpawns,
        });
    }
}
