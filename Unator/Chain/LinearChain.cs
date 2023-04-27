using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Chain;

/// <summary>
/// Instead of calling the next chain item inside of the current,
/// it returns a result and passes it to the next chain item.
/// When you call the next chain item inside, keep variables of the current chain item alive while they are unnecessary.
/// And you fill stack like in recursion. There is dynamic stuff under the hood,
/// but because input and addition of new chain items are typesafe it's ok.
/// I didn't find a way to make it without dynamics and linear at the same time.
/// </summary>
/// <remarks>
/// TODO:
/// - consider something like context
/// </remarks>
/// <typeparam name="TIn">Type of initial input</typeparam>
/// <typeparam name="TOut">Type of the last output</typeparam>
public class LinearChain<TIn, TOut>
{
    /// <summary>
    /// list of IChainItem&lt;TIn, TFirstOutput&gt;
    /// Should always be changed inside typesafe private block
    /// </summary>
    private readonly IList<dynamic> items;

    /// <summary> First chain item </summary>
    public LinearChain(Func<TIn, TOut> function) => items = new List<dynamic>() { function };

    /// <summary> Second and all next items </summary>
    private LinearChain(IList<dynamic> items) => this.items = items;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

    /// <summary>
    /// Do linear execution of the chain
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public TOut Run(TIn input)
    {
        dynamic val = input;
        foreach (var item in items) val = item(val);
        return val;
    }

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.

    public LinearChain<TIn, TNext> Next<TNext>(Func<TOut, TNext> next)
    {
        items.Add(next);
        return new LinearChain<TIn, TNext>(items);
    }

    public static LinearChain<TIn, TOut> Start(Func<TIn, TOut> function) => new(function);
}