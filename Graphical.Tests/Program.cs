using Algorithms;
using Datastructures;
using Graphical;
using Graphical.ImageSharpRenderer;
using SixLabors.ImageSharp;

(List<int>, BHeapAnimationData animation) heapsorted = HeapSort.Sort([1, 3, 5, 2, 6, 1, 2, 8], 0.5);

heapsorted.animation.Heap.RenderToFile(
    "heapsortHeap.mp4",
    (g, p) => Renderer.Render(g).Save(p),
    fps: 144,
    width: 1920,
    height: 1080
);
heapsorted.animation.FullArray.RenderToFile(
    "heapsortArray.mp4",
    (g, p) => Renderer.Render(g).Save(p)
);
