using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RandomGenSystem : SystemBase
{
    public NativeArray<Random> RandomGenPerThread { get; private set; }

    protected override void OnCreate()
    {
        var seed = new System.Random();
        var randomGenerators = new Random[JobsUtility.MaxJobThreadCount];
        for (int i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
        {
            randomGenerators[i] = new Random((uint)seed.Next());
        }

        RandomGenPerThread = new NativeArray<Random>(randomGenerators, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnDestroy()
    {
        RandomGenPerThread.Dispose();
    }
}
