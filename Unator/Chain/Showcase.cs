using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Chain;

/// <summary>
/// Just showcase of use Linear Chain
/// </summary>
internal class Showcase
{
    public static void Main()
    {
        var chain = LinearChain<int, string>
            .Start(input => (input + 1).ToString())
            .Next(input => new { TestAnonymus = true })
            .Next(input => input.TestAnonymus);

        var result = chain.Run(-46);

        var filter = new LinearFilter<int, bool>()
            | IsNotNegative
            | IsLess100
            | IsEven;

        var filterResult = filter.Run(-46);

        if (filterResult.All(x => x))
        {
            Console.WriteLine("Passed all filters");
        }
    }

    public static bool IsNotNegative(int input) => input >= 0;

    public static bool IsLess100(int input) => input < 100;

    public static bool IsEven(int input) => input % 2 == 0;
}