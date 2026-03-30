namespace Algorithms;

public static class HeapSort
{
    public static List<T> Sort<T>(List<T> values)
        where T : IComparable
    {
        T[] heap = [.. values];
        heapify(heap);
        for (int end = heap.Length - 1; end > 0; end--)
        {
            (heap[0], heap[end]) = (heap[end], heap[0]);
            siftDown(heap, 0, end);
        }

        return [.. heap];
    }

    private static void heapify<T>(T[] heap)
        where T : IComparable
    {
        for (int i = parent(heap.Length - 1); i >= 0; i--)
        {
            siftDown(heap, i, heap.Length);
        }
    }

    private static void siftDown<T>(T[] heap, int index, int end)
        where T : IComparable
    {
        int maxIndex = ((IEnumerable<int>)[index, lChild(index), rChild(index)])
            .Where(i => i < end)
            .MaxBy(i => heap[i]);

        if (maxIndex == index)
            return;

        (heap[index], heap[maxIndex]) = (heap[maxIndex], heap[index]);
        siftDown(heap, maxIndex, end);
    }

    private static int parent(int index) => (index - 1) / 2;

    private static int lChild(int index) => (2 * index) + 1;

    private static int rChild(int index) => (2 * index) + 2;
}
