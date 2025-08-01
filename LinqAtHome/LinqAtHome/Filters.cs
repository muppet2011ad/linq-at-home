namespace LinqAtHome;

public static class Filters
{
    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        IEnumerable<T> GetResults()
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        return GetResults();
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T other)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(other);

        return source.Where(x => x is null || !x.Equals(other));
    }
}