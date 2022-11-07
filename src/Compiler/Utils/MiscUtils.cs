using System.Collections.ObjectModel;

namespace Lotus.Utils;

public static class MiscUtils
{
    public static int GetNumberOfDigits(int i) {
        int count = 0;

        do count++; while ((i /= 10) >= 1);

        return count;
    }

    [DebuggerStepThrough]
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        where TKey : notnull
        => new(dic);

    [DebuggerStepThrough]
    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> list)
        => new(list.ToArray());

    [DebuggerStepThrough]
        public static string Join<T>(string separator, Func<T, string> convert, IEnumerable<T> coll) {
        var count = coll.Count();

        if (count == 0) {
            return "";
        } else if (count == 1) {
            return convert(coll.First());
        } else if (count < 20) {
            var output = "";

            foreach (var item in coll) output += convert(item) + separator;

            if (separator.Length != 0)
                output = output.Remove(output.Length - separator.Length);

            return output;
        } else {
            var output = new System.Text.StringBuilder();

            foreach (var item in coll) output.Append(convert(item)).Append(separator);

            if (separator.Length != 0)
                output = output.Remove(output.Length - separator.Length, separator.Length);

            return output.ToString();
        }
    }

    public static string GetDisplayName(this Type type) {
        if (!type.IsGenericType)
            return type.Name;

        return type.Name.Remove(type.Name.Length - 2)
             + '<'
             + Join(", ", GetDisplayName, type.GenericTypeArguments)
             + '>';
    }

    public static IEnumerable<TEnum> GetMatchingValues<TEnum>(this TEnum flag) where TEnum : struct, Enum {
        // fixme(utils): hard-code this or drastically improve the performance
        foreach (var value in Enum.GetValues<TEnum>()) {
            if (flag.HasFlag(value))
                yield return value;
        }
    }

    public static bool IsAsciiDigit(in char c) => (uint)(c - '0') <= 9;
}