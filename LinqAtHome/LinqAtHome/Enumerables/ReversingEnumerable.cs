using System.Collections;

namespace LinqAtHome.Enumerables;

internal class ReversingEnumerable<T>(IEnumerable<T> source) : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        using var sourceEnumerator = _mSource.GetEnumerator();

        var elements = new List<T>();
        
        while (sourceEnumerator.MoveNext())
        {
            elements.Add(sourceEnumerator.Current);
        }
        
        for (var i = elements.Count - 1; i >= 0; i--)
        {
            yield return elements[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private readonly IEnumerable<T> _mSource = source ?? throw new ArgumentNullException(nameof(source));
}