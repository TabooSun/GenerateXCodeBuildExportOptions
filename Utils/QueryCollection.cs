using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace GenerateXCodeBuildExportOptions.Utils;

public class QueryCollection : NameObjectCollectionBase
{
    public Object? this[String key]
    {
        get => BaseGet(key);
        set => BaseSet(key, value);
    }

    public void Add(String key, Object value)
    {
        BaseAdd(key, value);
    }

    public void Remove(String key)
    {
        BaseRemove(key);
    }

    public void Remove(int index)
    {
        BaseRemoveAt(index);
    }

    public void Clear()
    {
        BaseClear();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('?');
        const string queryDelimiter = "&";
        var enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current as string;
            if (current == null) break;
            var value = this[current];

            if (value is IEnumerable<string> values)
            {
                sb.Append($"{current}={string.Join(",", values)}{queryDelimiter}");
            }
            else
            {
                sb.Append($"{current}={value?.ToString() ?? ""}{queryDelimiter}");
            }
        }

        return new Regex($"{queryDelimiter}$").Replace(sb.ToString(), "");
    }
}
