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

    [SerializeField]
    private float m_smoothDuration = 0.1f;

    private EntityManager m_entityManager;
    private Vector3 m_lastDesiredPosition = Vector3.zero;
    private Vector3 m_followVelocity = Vector3.zero;

    public Entity EntityToFollow { get; set; }

    private void Awake()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_lastDesiredPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (EntityToFollow == null
            || !m_entityManager.HasComponent<Translation>(EntityToFollow))
        {
            MoveToPosition();
            return;
        }

        m_lastDesiredPosition = transform.position;
        Translation entityTranslation = m_entityManager.GetComponentData<Translation>(EntityToFollow);
        if (m_followX)
        {
            m_lastDesiredPosition.x = entityTranslation.Value.x;
        }
        if (m_followY)
        {
            m_lastDesiredPosition.y = entityTranslation.Value.y;
        }
        if (m_followZ)
        {
            m_lastDesiredPosition.z = entityTranslation.Value.z;
        }

        MoveToPosition();
    }

    private void MoveToPosition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_lastDesiredPosition, ref m_followVelocity, m_smoothDuration);
    }
}
