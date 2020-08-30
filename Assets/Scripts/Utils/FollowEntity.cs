using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FollowEntity : MonoBehaviour
{
    [SerializeField]
    private bool m_followX = default;

    [SerializeField]
    private bool m_followY = default;

    [SerializeField]
    private bool m_followZ = default;

    private EntityManager m_entityManager;

    public Entity EntityToFollow { get; set; }

    private void Awake()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        if (EntityToFollow == null)
        {
            return;
        }

        Vector3 newPosition = transform.position;
        Translation entityTranslation = m_entityManager.GetComponentData<Translation>(EntityToFollow);
        if (m_followX)
        {
            newPosition.x = entityTranslation.Value.x;
        }
        if (m_followY)
        {
            newPosition.y = entityTranslation.Value.y;
        }
        if (m_followZ)
        {
            newPosition.z = entityTranslation.Value.z;
        }
        transform.position = newPosition;
    }
}
