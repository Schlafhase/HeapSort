using System.Collections.Immutable;
using Graphical.Primitives;

namespace Graphical.Animations;

public record ChangeKeys(Dictionary<string, string> NewMappings) : Animation(0)
{
    public override Graphic Apply(Graphic g, double t)
    {
        // WARN: this is incredibly weird and will lead to a number of bugs
        Dictionary<int, string> topLevel = [];
        // parent path -> list of (child key, new key)
        Dictionary<string, List<(string oldKey, string newKey)>> nested = [];

        foreach ((string oldKey, string newKey) in NewMappings)
        {
            int idx = g.Primitives.FindIndex(p => p.Key == oldKey);
            if (idx >= 0)
            {
                topLevel[idx] = newKey;
                continue;
            }

            int lastDot = oldKey.LastIndexOf('.');
            if (lastDot < 0)
                continue;

            string parentPath = oldKey[..lastDot];
            string childKey = oldKey[(lastDot + 1)..];

            if (!nested.TryGetValue(parentPath, out var siblings))
            {
                siblings = [];
                nested[parentPath] = siblings;
            }
            siblings.Add((childKey, newKey));
        }

        ImmutableList<Primitive> newPrimitives = g.Primitives;

        foreach ((int idx, string newKey) in topLevel)
        {
            newPrimitives = newPrimitives.SetItem(idx, g.Primitives[idx] with { Key = newKey });
        }

        foreach ((string parentPath, List<(string oldKey, string newKey)>? siblings) in nested)
        {
            Primitive? parent = g.FindRecursive(parentPath).Value;
            if (parent is not Graphic parentGraphic)
            {
                continue;
            }

            Dictionary<string, string> siblingMappings = siblings.ToDictionary(
                s => s.oldKey,
                s => s.newKey
            );
            Graphic renamedParent = new ChangeKeys(siblingMappings).Apply(parentGraphic, t);

            int parentIdx = g.Primitives.FindIndex(p => p.Key == parentPath.Split('.')[0]);
            // For deeply nested parents we need to use Replace on a partially built graphic
            Graphic partialG = g with
            {
                Primitives = newPrimitives,
            };
            newPrimitives = (partialG.Replace(parentPath, renamedParent) with { }).Primitives;
        }

        return g with
        {
            Primitives = newPrimitives,
        };
    }
}
