namespace Snow.Core.Extensions;

public static class ListExtensions
{
    public static T PopAt<T>(this List<T> list, int index)
    {
        var r = list[index];
        list.RemoveAt(index);

        return r;
    }

    public static T Pop<T>(this List<T> list) => list.PopAt(0);

    public static List<List<T>> SlidingWindow<T>(this List<T> input, int windowSize)
    {
        if (input.Count < windowSize) return new List<List<T>>();

        var first = new[] {input.Take(windowSize).ToList()};
        var rest = SlidingWindow(input.Skip(1).ToList(), windowSize);

        return first.Union(rest).ToList();
    }

    public static List<TResult> SelectToList<T, TResult>(this IEnumerable<T> list, Func<T, TResult> map) =>
        list.Select(map).ToList();
}
