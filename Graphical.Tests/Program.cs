using Datastructures;
using Graphical;
using Graphical.ImageSharpRenderer;
using SixLabors.ImageSharp;

VisualisedArray<int> varray = new([1, 2, 3, 4, 5]);

varray.StartRecording();
varray.Swap(1, 3);
varray.Swap(1, 4);

varray.GetRecording().RenderToFile("out.mp4", (g, p) => Renderer.Render(g).Save(p));
