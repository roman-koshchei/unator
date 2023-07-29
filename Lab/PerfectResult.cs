namespace Lab;

internal class PerfectResultTest
{
    public static void Start()
    {
        for (int i = 0; i < 10; i++) Handle();
    }

    private static void Handle()
    {
        var perfect = PerfectOperation();

        if (perfect.Data.HasValue)
        {
            Console.WriteLine(perfect.Data.Value);
            return;
        }

        if (perfect.NumEquals3Error != null)
        {
            Console.WriteLine($"NumEquals3Error: {perfect.NumEquals3Error.Num}");
            return;
        }

        if (perfect.NumEqualsError == PerfectResultError.EqualsOne)
        {
            Console.WriteLine("Equals 1");
            return;
        }

        if (perfect.NumEqualsError == PerfectResultError.EqualsTwo)
        {
            Console.WriteLine("Equals 2");
            return;
        }

        Console.WriteLine($"Exception {perfect.Exception!.Message}");
    }

    private static PerfectResult PerfectOperation()
    {
        try
        {
            Random rnd = new();
            int num = rnd.Next(4);

            if (num == 0) return new PerfectResult(num);
            if (num == 1) return new PerfectResult(PerfectResultError.EqualsOne);
            if (num == 2) return new PerfectResult(PerfectResultError.EqualsTwo);
            return new PerfectResult(new NumEquals3Error(num));
        }
        catch (Exception ex)
        {
            return new PerfectResult(ex);
        }
    }

    public enum PerfectResultError
    {
        EqualsOne, EqualsTwo
    }

    public record class NumEquals3Error(int Num);

    public class PerfectResult
    {
        public long? Data { get; }

        public PerfectResult(long data) => Data = data;

        public PerfectResultError? NumEqualsError { get; }

        public PerfectResult(PerfectResultError numEqualsError) => NumEqualsError = numEqualsError;

        public NumEquals3Error? NumEquals3Error { get; }

        public PerfectResult(NumEquals3Error numEquals3Error) => NumEquals3Error = numEquals3Error;

        public Exception? Exception { get; }

        public PerfectResult(Exception exception) => Exception = exception;
    }
}