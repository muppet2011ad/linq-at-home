using System.Collections;
using NSubstitute;

namespace LinqAtHome.Tests;

public class FiltersTests
{
    private static IEnumerable Method_calls_with_null_arguments()
    {
        // ReSharper disable IteratorMethodResultIsIgnored
        yield return new TestCaseData(new TestDelegate
                (() => Filters.Where<int>(null!, x => x > 0)))
            .SetName("Where with null source");
        
        yield return new TestCaseData(new TestDelegate
                (() => Filters.Where([1, 2, 3], null!)))
            .SetName("Where with null predicate");
        
        yield return new TestCaseData(new TestDelegate
                (() => Filters.Except(null!, 42)))
            .SetName("Except with null source");
        
        yield return new TestCaseData(new TestDelegate
                (() => Filters.Except(["1", "2", "3"], null!)))
            .SetName("Except with null other");
        // ReSharper restore IteratorMethodResultIsIgnored
    }
    
    [TestCaseSource(nameof(Method_calls_with_null_arguments))]
    public void Methods_throw_when_arguments_are_null(TestDelegate testDelegate)
    {
        Assert.Throws<ArgumentNullException>(testDelegate);
    }
    
    private static TestCaseData[] WhereCases =>
    [
        new (new List<int> {1, 2, 3}, new Func<int, bool> (x => x > 1), new[] {2, 3}),
        new (new List<int>(), new Func<int, bool> (x => x > 0), new int[] {}),
        new (new List<int> { 42 }, new Func<int, bool> (x => x == 42), new [] {42}),
        new (new List<string> { "a", "b", "c" }, new Func<string, bool> (x => x != "b"), new[] { "a", "c" }),
        new (new List<int> {1, 2, 3}, new Func<int, bool> (x => x > 4), new int[] {}),
    ];
    
    [TestCaseSource(nameof(WhereCases))]
    public void Where_returns_elements_matching_predicate<T>(IEnumerable<T> source, Func<T, bool> predicate, IEnumerable<T> expected)
    {
        var result = source.Where(predicate);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Where_lazily_evaluates_elements()
    {
        var source = new[] { 1, 2, 3 };
        
        var predicate = Substitute.For<Func<int, bool>>();

        predicate.Invoke(0).ReturnsForAnyArgs(true);
        
        var whereEnumerable = source.Where(predicate);
        
        using var enumerator = whereEnumerable.GetEnumerator();
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        predicate.DidNotReceiveWithAnyArgs();
        
        _ = enumerator.Current;
        
        predicate.Received(1).Invoke(1);
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        _ = enumerator.Current;
        
        predicate.Received(1).Invoke(2);
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        _ = enumerator.Current;
        
        predicate.Received(1).Invoke(3);
        
        Assert.That(enumerator.MoveNext(), Is.False);
    }
    
    private static TestCaseData[] ExceptCases =>
    [
        new (new List<int> {1, 2, 3}, 2, new[] {1, 3}),
        new (new List<int>(), 42, new int[] {}),
        new (new List<int> { 42 }, 42, new int[] {}),
        new (new List<string> { "a", "b", "c" }, "b", new[] { "a", "c" }),
        new (new List<int> {1, 2, 3}, 4, new[] {1, 2, 3}),
    ];
    
    [TestCaseSource(nameof(ExceptCases))]
    public void Except_returns_elements_not_equal_to_other<T>(IEnumerable<T> source, T other, IEnumerable<T> expected)
    {
        var result = source.Except(other);
        
        Assert.That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Except_lazily_evaluates_elements()
    {
        var element = Substitute.For<IList<int>>();

        var source = new[] { element };
        
        var exceptEnumerable = source.Except([42]);

        _ = element.DidNotReceiveWithAnyArgs().Equals(null);

        _ = exceptEnumerable.ToArray();
        
        _ = element.ReceivedWithAnyArgs(1).Equals(null);
    }
}