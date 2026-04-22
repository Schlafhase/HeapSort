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
        InitialPrimitives?.OrderBy(p => p.Z).ToImmutableList() ?? [];

    public Graphic With(Primitive primitive) =>
        this with
        {
            Primitives = Primitives.Insert(
                Primitives.FindLastIndex(p => p.Z >= primitive.Z) is int i && i >= 0
                    ? i
                    : Primitives.Count,
                primitive
            ),
        };

    public Graphic ConditionalWith(bool predicate, Primitive primitive) =>
        predicate ? With(primitive) : this;

    public Graphic WithRange(IEnumerable<Primitive> primitives, bool sorted = false) =>
        this with
        {
            Primitives = mergeSortedLists(
                Primitives,
                sorted ? [.. primitives] : [.. primitives.OrderBy(p => p.Z)]
            ),
        };

    private static ImmutableList<Primitive> mergeSortedLists(
        ImmutableList<Primitive> a,
        ImmutableList<Primitive> b
    )
    {
        var result = ImmutableList.CreateBuilder<Primitive>();
        int i = 0,
            j = 0;
        while (i < a.Count && j < b.Count)
        {
            if (a[i].Z <= b[j].Z)
            {
                result.Add(a[i++]);
            }
            else
            {
                result.Add(b[j++]);
            }
        }
        while (i < a.Count)
        {
            result.Add(a[i++]);
        }

        while (j < b.Count)
        {
            result.Add(b[j++]);
        }

        return result.ToImmutable();
    }

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
                        p.Replace(string.Join('.', path[1..]), replacement)
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
        string[] path = key.Split('.');

        if (path.Length > 1)
        {
            Primitive? first = Find(path[0]);
            if (first is Graphic p)
            {
                return p.FindRecursive(
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
