using Algorithms;
using Datastructures;
using Graphical;
using Graphical.ImageSharpRenderer;

(List<int>, BHeapAnimationData animation) heapsorted = HeapSort.Sort([1, 3, 5, 2, 6, 1, 2, 8], 0.3);

heapsorted.animation.Heap.RenderToFile(
    "heapsortHeap.mp4",
    Renderer.RenderAndSave,
    fps: 14,
    width: 1920,
    height: 1080
);
// heapsorted.animation.FullArray.RenderToFile(
//     "heapsortArray.mp4",
//     (g, p) => Renderer.Render(g).Save(p)
// );
