// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Collections.Concurrent;
using System.Diagnostics;

namespace ChiVariate.Internal;

/// <summary>
///     A simple, thread-safe object pool for recycling class instances to reduce GC pressure.
/// </summary>
/// <typeparam name="T">The type of object to pool. Must be a reference type.</typeparam>
internal class ChiPool<T> where T : class
{
    private readonly ConcurrentQueue<T> _recycledInstances = new();

    /// <summary>
    ///     Rents an instance from the pool. If the pool is empty, a new instance is created using the provided factory.
    /// </summary>
    /// <param name="createInstance">A factory function to create a new instance if needed.</param>
    /// <returns>A recycled or new instance of <typeparamref name="T" />.</returns>
    public T Rent(Func<T> createInstance)
    {
        return _recycledInstances.TryDequeue(out var instance)
            ? instance
            : createInstance();
    }

    /// <summary>
    ///     Rents an instance from the pool, passing an argument to the factory if a new instance is needed.
    /// </summary>
    /// <param name="arg">The argument to pass to the creation factory.</param>
    /// <param name="createInstance">A factory function that takes an argument to create a new instance if needed.</param>
    /// <returns>A recycled or new instance of <typeparamref name="T" />.</returns>
    public T Rent<TArg>(TArg arg, Func<TArg, T> createInstance)
    {
        return _recycledInstances.TryDequeue(out var instance)
            ? instance
            : createInstance(arg);
    }

    /// <summary>
    ///     Returns an instance to the pool for later reuse.
    /// </summary>
    /// <param name="instance">The instance to recycle.</param>
    public void Recycle(T? instance)
    {
        Debug.Assert(instance is not null,
            $"Attempted to recycle a null instance. Type: {typeof(T).Name}");
        Debug.Assert(!_recycledInstances.Contains(instance),
            $"Attempted to recycle an instance that is already in the pool. Type: {typeof(T).Name}");

        _recycledInstances.Enqueue(instance);
    }
}