using Datastructures;
using Graphical;

namespace Algorithms;

public static class HeapSort
{
    public static (List<T>, AnimatedGraphic) Sort<T>(List<T> values)
        where T : IComparable
    {
        VisualisedArray<T> heap = new([.. values]);
        heap.StartRecording();
        heapify(heap);
        for (int end = heap.Length - 1; end > 0; end--)
        {
            heap.Swap(0, end);
            siftDown(heap, 0, end);
        }

        return ([.. heap], heap.GetRecording());
    }

    private static void heapify<T>(VisualisedArray<T> heap)
        where T : IComparable
    {
        for (int i = parent(heap.Length - 1); i >= 0; i--)
        {
            siftDown(heap, i, heap.Length);
        }
    }

    private static void siftDown<T>(VisualisedArray<T> heap, int index, int end)
        where T : IComparable
    {
        int maxIndex = ((IEnumerable<int>)[index, lChild(index), rChild(index)])
            .Where(i => i < end)
            .MaxBy(i => heap[i]);

        if (maxIndex == index)
            return;

        heap.Swap(index, maxIndex);
        siftDown(heap, maxIndex, end);
    }

    private static int parent(int index) => (index - 1) / 2;

    private static int lChild(int index) => (2 * index) + 1;

    private static int rChild(int index) => (2 * index) + 2;
}
