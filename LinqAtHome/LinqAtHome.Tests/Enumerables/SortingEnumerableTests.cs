using System.Collections;
using LinqAtHome.Enumerables;
using NSubstitute;

namespace LinqAtHome.Tests.Enumerables;

public class SortingEnumerableTests
{
    private static IEnumerable Constructor_calls_with_null_arguments()
    {
        // ReSharper disable ObjectCreationAsStatement
        yield return new TestCaseData(new TestDelegate
                (() => new SortingEnumerable<int, int>(null!, x => x)))
            .SetName("Null source");
        
        yield return new TestCaseData(new TestDelegate
                (() => new SortingEnumerable<int, int>([1, 2, 3], null!)))
            .SetName("Null key function");
        // ReSharper restore ObjectCreationAsStatement
    }
    
    [TestCaseSource(nameof(Constructor_calls_with_null_arguments))]
    public void Constructor_throws_when_arguments_are_null(TestDelegate testDelegate)
    {
        Assert.Throws<ArgumentNullException>(testDelegate);
    }
    
    private static TestCaseData[] SortCases =>
    [
        new (new List<int> {3, 1, 2}, new Func<int, int>(x => x), new[] {1, 2, 3}),
        new (new List<int> {1, 2, 2}, new Func<int, int>(x => x), new[] {1, 2, 2}),
        new (new List<int> {3, 1, 2}, new Func<int, int>(x => -x), new[] {3, 2, 1}),
        new (new List<int>(), new Func<int, int>(x => x), new int[] {}),
        new (new List<int> {42}, new Func<int, int>(x => x), new [] {42}),
        new (new List<string> { "c", "a", "b" }, new Func<string, string>(x => x), new[] { "a", "b", "c" }),
        new (new List<int> {5, 4, 3, 2, 1}, new Func<int, int>(x => x), new[] {1, 2, 3, 4, 5}),
    ];
    
    [TestCaseSource(nameof(SortCases))]
    public void GetEnumerator_returns_elements_sorted_by_key<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keyFunction, IEnumerable<TSource> expected)
    {
        var sortingEnumerable = new SortingEnumerable<TSource, TKey>(source, keyFunction);
        
        var result = sortingEnumerable.ToArray();
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void GetEnumerator_lazily_evaluates_elements()
    {
        var source = new[] { 1, 2, 3 };
        var keyFunction = Substitute.For<Func<int, int>>();
        
        keyFunction.Invoke(Arg.Any<int>()).ReturnsForAnyArgs(x => x[0]);
        
        var sortingEnumerable = new SortingEnumerable<int, int>(source, keyFunction);

        keyFunction.DidNotReceiveWithAnyArgs().Invoke(0);

        _ = sortingEnumerable.ToArray();
        
        keyFunction.ReceivedWithAnyArgs().Invoke(0);
    }
}