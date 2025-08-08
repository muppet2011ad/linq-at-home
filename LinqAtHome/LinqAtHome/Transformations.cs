using LinqAtHome.Enumerables;

namespace LinqAtHome;

public static class Transformations
{
    public static T[] ToArray<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        
        var results = new List<T>();

        using var enumerator = source.GetEnumerator();
        
        while (enumerator.MoveNext())
        {
            results.Add(enumerator.Current);
        }

        var resultArray = new T[results.Count];
        
        results.CopyTo(resultArray);
        
        return resultArray;
    }
    
    public static IEnumerable<T> Select<T>(this IEnumerable<T> source, Func<T, T> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        IEnumerable<T> GetResults()
        {
            foreach (var item in source)
            {
                yield return selector(item);
            }
        }

        return GetResults();
    }

    public static IEnumerable<T> Reverse<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ReversingEnumerable<T>(source);
    }
    
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new SortingEnumerable<T, T>(source, x => x);
    }
    
    public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return new SortingEnumerable<TSource, TKey>(source, keySelector);
    }
}