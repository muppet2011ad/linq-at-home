using LinqAtHome.Enumerables;

namespace LinqAtHome.Tests.Enumerables;

public class ReversingEnumerableTests
{
    [Test]
    public void Constructor_throws_when_source_is_null()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new ReversingEnumerable<int>(null!));
    }
    
    private static TestCaseData[] ReverseCases =>
    [
        new (new[] { 1, 2, 3 }, new[] {3, 2, 1}),
        new (new int[] { }, new int[] {}),
        new (new[] { 42 }, new [] {42}),
        new (new[] { 1, 5, 2, 4, 3 }, new[] {3, 4, 2, 5, 1}),
        new (new[] { "a", "b", "c" }, new[] { "c", "b", "a" })
    ];
    
    [TestCaseSource(nameof(ReverseCases))]
    public void GetEnumerator_returns_elements_in_reverse_order<T>(IEnumerable<T> source, IEnumerable<T> expected)
    {
        var reversingEnumerable = new ReversingEnumerable<T>(source);
        
        var result = reversingEnumerable.ToArray();
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEnumerator_lazily_evaluates_elements()
    {
        var source = new[] { 1, 2, 3 };
        var reversingEnumerable = new ReversingEnumerable<int>(source);

        using var enumerator = reversingEnumerable.GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);
    }
}