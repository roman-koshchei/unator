namespace Unator;

public static class QueryParamsExample
{
    public static void Run()
    {
        IQueryParams.Make();
    }

    public record ProductFilters(string Search, List<string> Categories, int Price) : IQueryParams;

    public struct PostFilters : IQueryParams
    {
        private readonly string search;
        public List<string> Categories { get; set; }
        public int Price { get; }

        public string Search() => search.Trim().ToLower();
    }
}

public interface IQueryParams
{
    private static readonly Type iQueryParamsInterface = typeof(IQueryParams);
    private static readonly Type iParsableInterface = typeof(IParsable<>);
    private static readonly Type iEnumerableInterface = typeof(IEnumerable<>);

    public static void Make()
    {
        Console.WriteLine("Making parsing methods for query params");

        var types = AppDomain.CurrentDomain
           .GetAssemblies()
           .SelectMany(x => x
               .GetTypes()
               .Where(type => type.IsInterface == false && type.GetInterfaces().Contains(iQueryParamsInterface))
           );

        foreach (var type in types)
        {
            Console.WriteLine($"{type.FullName}");
            CheckType(type);
        }
    }

    private static void CheckType(Type type)
    {
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var propType = property.PropertyType;

            if (propType.GetInterfaces().Any(x => x.Name == iEnumerableInterface.Name))
            {
                Console.WriteLine("Enumerable");
            }

            if (propType.GetInterfaces().Any(x => x.Name == iParsableInterface.Name))
            {
                Console.WriteLine("IParseble");
            }

            Console.WriteLine($"    {propType.Name} - {property.Name}");
        }
    }
}