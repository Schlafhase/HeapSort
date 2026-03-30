using Graphical.Primitives;

namespace Graphical;

public sealed class Graphic
{
    private readonly List<Primitive> _primitives;

    public Graphic(IEnumerable<Primitive>? primitives = null)
    {
        _primitives = primitives?.ToList() ?? [];
    }

    public Graphic Add(Primitive primitive) => new(_primitives.Append(primitive));

    public Graphic AddRange(IEnumerable<Primitive> primitives) =>
        new(_primitives.Concat(primitives));

    public Graphic Remove(string key) => new(_primitives.Where(p => p.Key != key));

    public Graphic Replace(string key, Primitive replacement) =>
        new(_primitives.Select(p => p.Key == key ? replacement : p));

    public Primitive? Find(string key) => _primitives.FirstOrDefault(p => p.Key == key);

    public Graphic Modify(string key, Func<Primitive, Primitive> modify)
    {
        var target =
            Find(key)
            ?? throw new KeyNotFoundException(
                $"No primitive with key '{key}' exists in this Graphic."
            );
        return Replace(key, modify(target));
    }
}

