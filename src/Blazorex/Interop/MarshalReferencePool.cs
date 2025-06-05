using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace Blazorex.Interop;

internal sealed class MarshalReferencePool
{
    private readonly ConcurrentDictionary<int, MarshalReference> _cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MarshalReference Next(ElementReference elementReference) =>
        elementReference.Id switch
        {
            null
                => throw new ArgumentException(
                    "ElementReference.Id cannot be null",
                    nameof(elementReference)
                ),
            var id when int.TryParse(id, out var hash) => GetOrCreateReference(hash, true),
            var id => GetOrCreateReference(id.GetHashCode(), true)
        };

    public MarshalReference Next(params object[] methodParams) =>
        methodParams.Length switch
        {
            0 => GetOrCreateReference(0, false),
            1 => GetOrCreateReference(methodParams[0]?.GetHashCode() ?? 0, false),
            2 => GetOrCreateReference(HashCode.Combine(methodParams[0], methodParams[1]), false),
            3
                => GetOrCreateReference(
                    HashCode.Combine(methodParams[0], methodParams[1], methodParams[2]),
                    false
                ),
            4
                => GetOrCreateReference(
                    HashCode.Combine(
                        methodParams[0],
                        methodParams[1],
                        methodParams[2],
                        methodParams[3]
                    ),
                    false
                ),
            _ => GetOrCreateReference(CreateKeyFromLargeParams(methodParams), false)
        };

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CreateKeyFromLargeParams(ReadOnlySpan<object> key)
    {
        var hash = new HashCode();
        foreach (var item in key)
            hash.Add(item);
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarshalReference GetOrCreateReference(int keyHash, bool isElementReference) =>
        _cache.GetOrAdd(
            keyHash,
            static (hash, isElement) => new MarshalReference(hash, isElement),
            isElementReference
        );
}
