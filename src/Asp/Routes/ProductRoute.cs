using Unator;

namespace Asp.Routes;

public class ProductRoute : AspRoute<int>
{
    public static UPath<int> Path { get; } = U.Path("products").Param<int>();

    public static string Url(int productId)
    {
        return Path.Url(productId);
    }

    public Task Handle(HttpContext state, int productId)
    {
        throw new NotImplementedException();
    }
}