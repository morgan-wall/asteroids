using UnityEngine;
using Unity.Entities;
using Unity.Assertions;

public class GameManager : MonoBehaviour
{
    private static readonly System.Type[] s_systemTypes = new System.Type[]
    {
        typeof(CollectionSystem),
        typeof(CollisionSystem),
        typeof(ConstantForceSystem),
        typeof(DamageSystem),
        typeof(EnemySystem),
        typeof(FreezePositionSystem),
        typeof(GameStateSystem),
        typeof(MovableSystem),
        typeof(PlayerSystem),
        typeof(RandomGenSystem),
        typeof(SpawnSystem),
        typeof(WeaponSystem),
    };

    private void Awake()
    {
        var gameStateSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameStateSystem>();
        Assert.IsTrue(gameStateSystem != null, "Unable to retrieve GameStateSystem");
        gameStateSystem.OnGameOver += OnGameOver;
    }

    private void Start()
    {
    }

    private void EnableSystems(bool enabled)
    {
        foreach (System.Type type in s_systemTypes)
        {
            var system = World.DefaultGameObjectInjectionWorld.GetExistingSystem(type);
            if (system != null)
            {
                system.Enabled = enabled;
            }
        }
    }

    private void OnGameOver()
    {
        EnableSystems(false);
        Debug.Log("Game over!");
    }
}
