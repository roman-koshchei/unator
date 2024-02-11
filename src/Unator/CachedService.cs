using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator;

public class CachedServiceExample
{
    public static void Run()
    {
        var loader = new Loader();

        loader.Service.Get.PrintServiceUsageCount();
        loader.Service.Get.PrintServiceUsageCount();
        loader.Service.Get.PrintServiceUsageCount();
    }

    public class Loader
    {
        // services isn't created during new() invocation
        public CachedService<ExampleService> Service { get; } = new();
    }

    public class ExampleService
    {
        private int count = 1;

        public void PrintServiceUsageCount()
        {
            Console.WriteLine($"Service used {count} times");
            count += 1;
        }
    }
}

/// <summary>
/// OOP-only version of Cached Service
/// Doesn't use lambda
/// Turned out to have the best performance
/// </summary>
public class CachedService<T> where T : new()
{
    private T? service;
    private Func<T> getter;

    public CachedService()
    {
        getter = FirstTimeGet;
    }

    private T FirstTimeGet()
    {
        service = new T();
        getter = RegularGet;
        return service;
    }

    private T RegularGet()
    {
#pragma warning disable CS8603 // at this stage service isn't null
        return service;
#pragma warning restore CS8603
    }

    public T Get => getter();
}

public class MixCachedService<T> where T : new()
{
    private Func<T> getter;

    public MixCachedService()
    {
        getter = () =>
        {
            var service = new T();
            getter = () => service;
            return service;
        };
    }

    public T Get => getter();
}