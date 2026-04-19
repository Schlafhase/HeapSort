using Graphical.Primitives;

namespace Graphical.Animations;

public record ChangeKeys(Dictionary<string, string> NewMappings) : Animation(0)
{
    public override Graphic Apply(Graphic g, double t)
    {
        Dictionary<int, string> idxToId = [];
        foreach (string key in NewMappings.Keys)
        {
            int idx = g.Primitives.FindIndex(p => p.Key == key);
            idxToId[idx] = NewMappings[key];
        }

        List<Primitive> newPrimitives = [];

        for (int i = 0; i < g.Primitives.Count; i++)
        {
            if (idxToId.TryGetValue(i, out string? newKey))
            {
                newPrimitives.Add(g.Primitives[i] with { Key = newKey });
            }
            else
            {
                newPrimitives.Add(g.Primitives[i]);
            }
        }

        return new Graphic(newPrimitives);
    }
}
