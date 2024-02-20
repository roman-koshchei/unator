using System.Diagnostics.CodeAnalysis;
using System.Text;
using Web;

string? preheader = "Showing Jooble Unator library";

var email = Email.Html(
    preheader is not null ? Email.Preview(preheader) : Email.Empty(),

    Email.H1("Hi Jooble"),

    Email.Link(href: "https://roman-koshchei.github.io/unator", "Best C# utilities")
);

var text = new StringBuilder();
email.RenderText(text);
Console.WriteLine(text.ToString());
Console.WriteLine();

var html = new StringBuilder();
email.RenderHtml(html);
Console.WriteLine(html.ToString());
Console.WriteLine();

//var db = UmbeddedDb.Open("./umbedded");
//if (!db.HasVal)
//{
//    Console.WriteLine(db.Err.ToString());
//    return;
//}

//await db.Val.Mutate(tables =>
//{
//    tables.Add("users");
//    return Task.CompletedTask;
//});

//await db.Val.Query(tables =>
//{
//    foreach (var table in tables)
//    {
//        Console.WriteLine(table);
//    }
//    return Task.CompletedTask;
//});

public class UmbeddedDb
{
    private List<string> tables;

    private UmbeddedDb(IEnumerable<string> tables)
    {
        this.tables = tables.ToList();
    }

    public static ResultRef<UmbeddedDb> Open(string path)
    {
        try
        {
            var tables = Directory.GetFiles(path);
            return ResultRef<UmbeddedDb>.Ok(new(tables));
        }
        catch (Exception e)
        {
            return ResultRef<UmbeddedDb>.Fail(new Exception(
                $"Can't open UmbeddedDB for {path} directory.", e
            ));
        }
    }

    public async Task Mutate(Func<List<string>, Task> operation)
    {
        await operation(tables);
    }

    public async Task Query(Func<IReadOnlyList<string>, Task> operation)
    {
        await operation(tables);
    }
}

public class ResultRef<T> where T : class
{
    public T? Val { get; init; }
    public Exception? Err { get; init; }

    [MemberNotNullWhen(true, nameof(Val))]
    [MemberNotNullWhen(false, nameof(Err))]
    public bool HasVal
    {
        get => Val != null;
    }

    public ResultRef()
    { }

    public static ResultRef<T> Ok([NotNull] T value)
    {
        return new ResultRef<T>() { Val = value, Err = null };
    }

    public static ResultRef<T> Fail([NotNull] Exception error)
    {
        return new ResultRef<T>() { Err = error, Val = null };
    }
}