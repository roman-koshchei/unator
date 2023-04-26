using InConsole;
using System.Collections.Specialized;

//DecoratorProgram.Main();
//StackCall.Run();
//ChainCall.Run();

// procedural
var r1 = IsAuthorized(null);
var r2 = Handle(r1);
var r3 = Handle(r2);
var r4 = Additional(r3, Handle(r3));
var r5 = Handle(r4);
var r6 = AfterAuthorized(r5);

// functional?
var usual = AfterAuthorized(
    Handle(
        Additional(
            Handle(
                Handle(
                    IsAuthorized(null)
                )
            ),
            Handle(100)
        )
    )
);

// oop
var linearChain = LinearChain<int?, int>
    .Start(IsAuthorized)
    .Next(Handle)
    .Next(Handle)
    .Next(hm => Additional(hm, Handle(hm)))
    .Next(Handle)
    .Next(AfterAuthorized)
    .Run(0);

var end = -1;

static int IsAuthorized(int? tmp)
{
    // complex logic
    int complexLoginResult = 10;

    return complexLoginResult;
}

static int? AfterAuthorized(int handlerResult)
{
    var wrapObj = new
    {
        Number = handlerResult
    };

    Console.WriteLine(wrapObj);
    return null;
}

static int Handle(int complexLogicResult)
{
    // some more transformations

    return complexLogicResult * 10;
}
static int Additional(int complexLogicResult, int options)
{
    // some more transformations

    return complexLogicResult * 10 * options;
}

internal class StackCall
{
    public static void Run() => Authorized(complexLogicResult =>
    {
        return complexLogicResult * 10;
    });

    private static void Authorized(Func<int, int> action)
    {
        // complex logic
        int complexLoginResult = 10;

        // old version
        int res = action(complexLoginResult);
        var wrapObj = new
        {
            Number = res
        };

        Console.WriteLine(wrapObj);
    }

    // wrap function
    // TODO: run later
    private static void CatchUnexpected(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
        }
    }
}