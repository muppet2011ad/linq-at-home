using System.Collections;

namespace LinqAtHome.Enumerables;

public class SortingEnumerable<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keyFunction) : IEnumerable<TSource>
{
    public IEnumerator<TSource> GetEnumerator()
    {
        var elements = new List<TSource>();

        using var sourceEnumerator = _mSource.GetEnumerator();
        while (sourceEnumerator.MoveNext())
        {
            elements.Add(sourceEnumerator.Current);
        }

        elements.Sort((x, y) => Comparer<TKey>.Default.Compare(_mKeyFunction(x), _mKeyFunction(y)));

        foreach (var element in elements)
        {
            yield return element;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    private readonly IEnumerable<TSource> _mSource = source ?? throw new ArgumentNullException(nameof(source));
    
    private readonly Func<TSource, TKey> _mKeyFunction = keyFunction ?? throw new ArgumentNullException(nameof(keyFunction));
}