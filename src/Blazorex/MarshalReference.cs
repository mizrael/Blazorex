using Microsoft.AspNetCore.Components;

namespace Blazorex
{
    internal readonly struct MarshalReference
    {
        public MarshalReference(string id, bool isRef) : this()
        {
            Id = id;
            IsRef = isRef;
        }

        public bool IsRef { get; }
        public string Id { get; }

        public static MarshalReference Map(ElementReference element)
            => new MarshalReference(element.Id, true);
    }
}