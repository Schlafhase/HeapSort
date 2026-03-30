using System.Diagnostics;
using Algorithms;

namespace Tests;

public class HeapSortTests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Sorting([Values(0, 1, 5, 10, 100, 1000, 100_000)] int size)
    {
        Random rng = new(42 + size);

        List<int> values =
        [
            .. Enumerable.Range(0, size).Select(_ => rng.Next(int.MinValue, int.MaxValue)),
        ];

        Stopwatch sw = Stopwatch.StartNew();
        List<int> heapsorted = HeapSort.Sort(values);
        sw.Stop();

        TestContext.Out.WriteLine($"Size: {size}, Took: {sw.Elapsed.TotalMicroseconds}µs");

        Assert.That(heapsorted, Is.Ordered);
    }

    [Test]
    public void AlreadySorted() =>
        Assert.That(HeapSort.Sort([.. Enumerable.Range(0, 100)]), Is.Ordered);

    [Test]
    public void ReverseSorted() =>
        Assert.That(HeapSort.Sort([.. Enumerable.Range(0, 100).Reverse()]), Is.Ordered);

    [Test]
    public void Duplicates() =>
        Assert.That(HeapSort.Sort([11, 2, 3, 2, 4, 4, 2, 11, 15, 2, 3]), Is.Ordered);
}
