using Datastructures;

namespace Algorithms;

public static class HeapSort
{
    public static (List<int>, BHeapAnimationData) Sort(List<int> values)
    {
        VisualisedBHeap heap = new([.. values]);
        heap.StartRecording();
        heapify(heap);
        for (int end = heap.Length - 1; end > 0; end--)
        {
            heap.End = end;
            heap.Swap(0, end);
            siftDown(heap, 0, end);
        }

        return ([.. heap], heap.GetRecording());
    }

    private static void heapify(VisualisedBHeap heap)
    {
        for (int i = parent(heap.Length - 1); i >= 0; i--)
        {
            siftDown(heap, i, heap.Length);
        }
    }

    private static void siftDown(VisualisedBHeap heap, int index, int end)
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
