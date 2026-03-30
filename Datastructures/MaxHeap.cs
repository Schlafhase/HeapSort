namespace Datastructures;

public class MaxHeap<T>
    where T : IComparable
{
    private readonly List<T> _internalList;
    private int _heapEnd;

    public MaxHeap(IEnumerable<T> values)
    {
        _internalList = [.. values];
        _heapEnd = 
    }

    public MaxHeap()
    {
        _internalList = [];
    }

    public void Heapify()
    {
        int start = parent(_internalList.Count - 1);

        while (start >= 0)
        {
            SiftDown(start);
            start--;
        }
    }

    public void SiftDown(int index)
    {
        int maxIndex = ((IEnumerable<int>)[index, lChild(index), rChild(index)])
            .Where(i => i < _internalList.Count)
            .MaxBy(i => _internalList[i]);

        if (maxIndex == index)
            return;

        (_internalList[index], _internalList[maxIndex]) = (
            _internalList[maxIndex],
            _internalList[index]
        );
        SiftDown(maxIndex);
    }

    private int parent(int index) => (int)Math.Floor((index - 1d) / 2);

    private int lChild(int index) => (2 * index) + 1;

    private int rChild(int index) => (2 * index) + 2;
}
