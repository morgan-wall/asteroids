using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnPointManager : MonoBehaviour
{
    private static readonly int s_minSpawnRings = 1;
    private static readonly int s_minSpawnPointsPerRing = 1;
    private static readonly float3 s_basisVector = new float3(0.0f, 0.0f, 1.0f);
    private static readonly float3 s_axisVector = new float3(0.0f, 1.0f, 0.0f);

    [SerializeField]
    private GameObject m_spawnPointPrefab = default;

    [SerializeField]
    private float m_maxOffset = default;
    
    [SerializeField]
    private float m_spawnPointsPerMetre = default;
    
    [SerializeField]
    private float m_spawnRingsPerMetre = default;

    private Entity m_spawnPointEntity;
    private EntityManager m_entityManager;
    private BlobAssetStore m_blobAssetStore;

    private void Awake()
    {
        m_blobAssetStore = new BlobAssetStore();
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, m_blobAssetStore);
        m_spawnPointEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_spawnPointPrefab, settings);
    }

    private void OnDestroy()
    {
        m_blobAssetStore.Dispose();
    }

    private void Start()
    {
        int spawnRings = math.max(s_minSpawnRings, (int)math.round(m_maxOffset * m_spawnRingsPerMetre));
        for (int i = 0; i < spawnRings; ++i)
        {
            float ringRadius = math.lerp(0.0f, m_maxOffset, (i + 1) / (float)spawnRings);
            float ringCircumference = 2.0f * math.PI * ringRadius;
            int spawnPointsForRing = math.max(s_minSpawnPointsPerRing, (int)math.round(ringCircumference * m_spawnPointsPerMetre));
            for (int j = 0; j < spawnPointsForRing; ++j)
            {
                float angleOffset = math.lerp(0.0f, 360.0f, (j + 1) / (float)spawnPointsForRing);
                float3 position = math.mul(quaternion.AxisAngle(s_axisVector, angleOffset), s_basisVector) * ringRadius;
                Entity instance = m_entityManager.Instantiate(m_spawnPointEntity);
                m_entityManager.SetComponentData(instance, new Translation
                {
                    Value = position,
                });
            }
        }
    }
}
