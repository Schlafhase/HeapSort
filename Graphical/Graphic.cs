using System.Collections.Immutable;
using Graphical.Primitives;

namespace Graphical;

public sealed class Graphic(IEnumerable<Primitive>? primitives = null)
{
    public ImmutableList<Primitive> Primitives { get; } = primitives?.ToImmutableList() ?? [];

    public Graphic With(Primitive primitive) => new(Primitives.Add(primitive));

    public Graphic WithRange(IEnumerable<Primitive> primitives) =>
        new(Primitives.AddRange(primitives));

    public Graphic Remove(string key)
    {
        int index = Primitives.FindIndex(p => p.Key == key);
        return index < 0 ? this : new(Primitives.RemoveAt(index));
    }

    public Graphic Replace(string key, Primitive replacement)
    {
        int index = Primitives.FindIndex(p => p.Key == key);
        return index < 0 ? this : new(Primitives.SetItem(index, replacement));
    }

    public Primitive? Find(string key) => Primitives.FirstOrDefault(p => p.Key == key);

    public Graphic Modify(string key, Func<Primitive, Primitive> modify)
    {
        Primitive? target = Find(key);
        return target is not null ? Replace(key, modify(target)) : this;
    }

    public Graphic Modify(string key, Func<Primitive, Primitive> modify, out bool success)
    {
        Primitive? target = Find(key);
        success = target is not null;
        return success ? Replace(key, modify(target!)) : this;
    }
}
