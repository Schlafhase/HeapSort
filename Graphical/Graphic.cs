using System.Collections.Immutable;
using Graphical.Primitives;

namespace Graphical;

public record class Graphic(
    IEnumerable<Primitive>? InitialPrimitives = null,
    string? Key = null,
    Transform? Transform = null,
    Paint? Paint = null
) : Composite(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint)
{
    public ImmutableList<Primitive> Primitives { get; } =
        InitialPrimitives?.ToImmutableList() ?? [];

    public Graphic With(Primitive primitive) => new(Primitives.Add(primitive));

    public Graphic WithRange(IEnumerable<Primitive> primitives) =>
        new(Primitives.AddRange(primitives));

    public AnimatedGraphic Animate() => new(this);

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

    public Primitive? Find(string key) =>
        Primitives
            .SelectMany<Primitive, Primitive>(p =>
                p is Composite c ? [p, .. c.GetPrimitives()] : [p]
            )
            .FirstOrDefault(p => p.Key == key);

    private (Primitive? p, Composite? c) findInsideComposites(string key)
    {
        foreach (Primitive item in Primitives)
        {
            if (item.Key == key)
            {
                return (item, null);
            }
            if (item is Composite parent)
            {
                foreach (Primitive child in parent.GetPrimitives())
                {
                    if (child.Key == key)
                    {
                        return (child, parent);
                    }
                }
            }
        }
        return (null, null);
    }

    public Graphic Modify(string key, Func<Primitive, Primitive> modify)
    {
        (Primitive? p, Composite? c) = findInsideComposites(key);
        if (p is null)
        {
            return this;
        }
        if (c is null)
        {
            return Replace(key, modify(p));
        }
        // FIX: fix possible null reference
        return Replace(c.Key, c.ModifyChild(p.Key, modify));
    }

    public Graphic Modify(string key, Func<Primitive, Primitive> modify, out bool success)
    {
        (Primitive? p, Composite? c) = findInsideComposites(key);
        success = p is not null;
        if (p is null)
        {
            return this;
        }
        if (c is null)
        {
            return Replace(key, modify(p));
        }
        // FIX: fix possible null reference
        return Replace(c.Key, c.ModifyChild(p.Key, modify));
    }

    public override IEnumerable<Primitive> GetPrimitives()
    {
        return Primitives;
    }

    public override Composite ModifyChild(string target, Func<Primitive, Primitive> modify)
    {
        return Modify(target, modify);
    }
}
