using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InConsole;

internal class LinearChain<TIn, TOut>
{
    /// <summary>
    /// list of IChainItem&lt;TIn, TFirstOutput&gt;
    /// Should always be changed inside typesafe private block
    /// </summary>
    private readonly IList<dynamic> items;

    public LinearChain(Func<TIn, TOut> function)
    {
        items = new List<dynamic>() { function };
    }

    private LinearChain(IList<dynamic> items)
    {
        this.items = items;
    }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

    public TOut Run(TIn input)
    {
        dynamic val = input;
        foreach (var item in items) val = item(val);
        return val;
    }

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public LinearChain<TIn, TNext> Next<TNext>(Func<TOut, TNext> next)
    {
        items.Add(next);
        return new LinearChain<TIn, TNext>(items);
    }

    public static LinearChain<TIn, TOut> Start(Func<TIn, TOut> function) => new(function);
}