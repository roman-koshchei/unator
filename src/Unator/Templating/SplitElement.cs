using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Templating;

public class SplitElement(string start, string end)
{
    public string Start { get; } = start;
    public string End { get; } = end;

    public string Wrap(params string[] content)
    {
        var sb = new StringBuilder(Start);
        foreach (var item in content)
        {
            sb.Append(item);
        }
        sb.Append(End);
        return sb.ToString();
    }

    public void Deconstruct(out string start, out string end)
    {
        start = Start; end = End;
    }
}