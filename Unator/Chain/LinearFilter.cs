namespace Unator.Chain;

/// <summary>
/// All functions get TIn and return TOut
/// </summary>
public class LinearFilter<TIn, TOut>
{
    private readonly IList<Func<TIn, TOut>> items;

    public LinearFilter() => items = new List<Func<TIn, TOut>>();

    public LinearFilter(Func<TIn, TOut> function) => items = new List<Func<TIn, TOut>>() { function };

    private LinearFilter(IList<Func<TIn, TOut>> items) => this.items = items;

    public List<TOut> Run(TIn input)
    {
        List<TOut> val = new(items.Count);
        foreach (var item in items) val.Add(item(input));
        return val;
    }

    public static LinearFilter<TIn, TOut> operator |(LinearFilter<TIn, TOut> filter, Func<TIn, TOut> next)
    {
        filter.items.Add(next);
        return new LinearFilter<TIn, TOut>(filter.items);
    }

    public static explicit operator LinearFilter<TIn, TOut>(Func<TIn, TOut> func) => new(func);

    public static LinearFilter<TIn, TOut> Start(Func<TIn, TOut> function) => new(function);
}