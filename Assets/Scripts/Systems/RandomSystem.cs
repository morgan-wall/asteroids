using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RandomSystem : SystemBase
{
    public NativeArray<Unity.Mathematics.Random> RandomGenPerThread { get; private set; }

    protected override void OnCreate()
    {
        var seed = new System.Random();
        var rngs = new Random[JobsUtility.MaxJobThreadCount];
        for (int i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
        {
            rngs[i] = new Random((uint)seed.Next());
        }

        RandomGenPerThread = new NativeArray<Random>(rngs, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnDestroy()
    {
        RandomGenPerThread.Dispose();
    }
}
