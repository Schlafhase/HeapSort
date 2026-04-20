using System.Collections.Immutable;
using Graphical.Primitives;

namespace Graphical;

public record Graphic(
    IEnumerable<Primitive>? InitialPrimitives = null,
    string? Key = null,
    Transform? Transform = null,
    Paint? Paint = null
) : Primitive(Key, Transform, Paint)
{
    public ImmutableList<Primitive> Primitives { get; init; } =
        InitialPrimitives?.ToImmutableList() ?? [];

    public Graphic With(Primitive primitive) =>
        this with
        {
            Primitives = Primitives.Add(primitive),
        };

    public Graphic WithRange(IEnumerable<Primitive> primitives) =>
        this with
        {
            Primitives = Primitives.AddRange(primitives),
        };

    public AnimatedGraphic Animate() => new(this);

    public Graphic Remove(string key)
    {
        int index = Primitives.FindIndex(p => p.Key == key);
        return index < 0 ? this : this with { Primitives = Primitives.RemoveAt(index) };
    }

    public Graphic Replace(string key, Primitive replacement)
    {
        string[] path = key.Split('.');
        if (path.Length > 1)
        {
            FindResult parent = FindRecursive(path[0]);
            return parent.Value is Graphic p
                ? this with
                {
                    Primitives = Primitives.SetItem(
                        parent.Index,
                        p.Replace(string.Join(',', path[1..]), replacement)
                    ),
                }
                : this;
        }

        int index = Primitives.FindIndex(p => p.Key == key);
        return index < 0 ? this : this with { Primitives = Primitives.SetItem(index, replacement) };
    }

    public Primitive? Find(string key) => Primitives.FirstOrDefault(p => p.Key == key);

    public FindResult FindRecursive(string key, FindResult? parent = null)
    {
        // TODO: introduce syntax like: "composite.primitive" for better performance and readability
        string[] path = key.Split('.');

        if (path.Length > 1)
        {
            Primitive? first = Find(path[0]);
            if (first is Graphic p)
            {
                return FindRecursive(
                    string.Join('.', path[1..]),
                    new()
                    {
                        Value = p,
                        Parent = parent,
                        Index = Primitives.IndexOf(first),
                    }
                );
            }
        }
        foreach (Primitive item in Primitives)
        {
            if (item.Key == key)
            {
                return new()
                {
                    Value = item,
                    Parent = parent,
                    Index = Primitives.IndexOf(item),
                };
            }
        }
        return new() { Value = null, Parent = null };
    }

    public Graphic Modify(string key, Func<Primitive, Primitive> modify)
    {
        FindResult r = FindRecursive(key);
        Primitive? p = r.Value;
        if (p is null)
        {
            return this;
        }
        return Replace(key, modify(p));
    }

    public Graphic Modify(string key, Func<Primitive, Primitive> modify, out bool success)
    {
        FindResult r = FindRecursive(key);
        Primitive? p = r.Value;
        success = p is not null;
        if (p is null)
        {
            return this;
        }
        return Replace(key, modify(p));
    }

    public virtual IEnumerable<Primitive> GetPrimitives()
    {
        return Primitives;
    }
}

public class FindResult
{
    public FindResult? Parent;
    public Primitive? Value;
    public int Index;
    public bool IsRoot => Parent is null;
    public bool Success => Value is not null;
}
