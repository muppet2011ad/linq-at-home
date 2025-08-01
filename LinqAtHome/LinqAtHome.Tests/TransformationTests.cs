using System.Collections;
using LinqAtHome.Enumerables;
using NSubstitute;

namespace LinqAtHome.Tests;

public class TransformationTests
{
    private static IEnumerable Method_calls_with_null_arguments()
    {
        // ReSharper disable IteratorMethodResultIsIgnored
        yield return new TestCaseData(new TestDelegate
                (() => Transformations.ToArray<int>(null!)))
            .SetName("ToArray with null source");
        
        yield return new TestCaseData(new TestDelegate
                (() => Transformations.Select<int>(null!, x => x)))
            .SetName("Select with null source");
        
        yield return new TestCaseData(new TestDelegate
                (() => Transformations.Select([1, 2, 3], null!)))
            .SetName("Select with null selector");
        
        yield return new TestCaseData(new TestDelegate
                (() => Transformations.Reverse<int>(null!)))
            .SetName("Reverse with null source");
        // ReSharper restore IteratorMethodResultIsIgnored
    }
    
    [TestCaseSource(nameof(Method_calls_with_null_arguments))]
    public void Methods_throw_when_arguments_are_null(TestDelegate testDelegate)
    {
        Assert.Throws<ArgumentNullException>(testDelegate);
    }
    
    private static TestCaseData[] ToArrayCases =>
    [
        new (new List<int> {1, 2, 3}, new[] {1, 2, 3}),
        new (new List<int>(), new int[] {}),
        new (new List<int> { 42 }, new [] {42}),
        new (new List<string> { "a", "b", "c" }, new[] { "a", "b", "c" }),
        new (new HashSet<string> { "a", "b", "c" }, new[] { "a", "b", "c" }),
        new (new [] { "a", "b", "c" }, new[] { "a", "b", "c" }),
    ];
    
    [TestCaseSource(nameof(ToArrayCases))]
    public void ToArray_returns_elements_as_array<T>(IEnumerable<T> source, T[] expected)
    {
        var result = source.ToArray();
        
        Assert.That(result, Is.EqualTo(expected)
                              .And.TypeOf(typeof(T[])));
    }
    
    private static TestCaseData[] SelectCases =>
    [
        new (new List<int> {1, 2, 3}, new Func<int, int> (x => x * 2), new[] {2, 4, 6}),
        new (new List<int>(), new Func<int, int> (x => x * 2), new int[] {}),
        new (new List<int> { 42 }, new Func<int, int> (x => x + 1), new [] {43}),
        new (new List<string> { "a", "b", "c" }, new Func<string, string> (x => x.ToUpper()), new[] { "A", "B", "C" }),
    ];
    
    [TestCaseSource(nameof(SelectCases))]
    public void Select_applies_selector_to_each_element<T>(IEnumerable<T> source, Func<T, T> selector, T[] expected)
    {
        var result = source.Select(selector).ToArray();
        
        Assert.That(result, Is.EqualTo(expected)
                              .And.TypeOf(typeof(T[])));
    }

    [Test]
    public void Select_lazily_evaluates_elements()
    {
        var selector = Substitute.For<Func<int, int>>();
        
        var source = new[] { 1, 2, 3 };
        
        var selected = source.Select(selector);
        
        using var enumerator = selected.GetEnumerator();
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        selector.DidNotReceiveWithAnyArgs();

        _ = enumerator.Current;
        
        selector.Received(1).Invoke(1);
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        _ = enumerator.Current;
        
        selector.Received(1).Invoke(2);
        
        Assert.That(enumerator.MoveNext(), Is.True);
        
        _ = enumerator.Current;
        
        selector.Received(1).Invoke(3);
        
        Assert.That(enumerator.MoveNext(), Is.False);
    }
    
    [Test]
    public void Reverse_returns_ReversingEnumerable()
    {
        var source = new[] { 1, 2, 3 };
        
        var reversed = source.Reverse();
        
        Assert.That(reversed, Is.InstanceOf<ReversingEnumerable<int>>());
        
        var result = reversed.ToArray();
        
        Assert.That(result, Is.EqualTo(new[] { 3, 2, 1 }));
    }
}