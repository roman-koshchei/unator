using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator;

public class Result<T> where T : class
{
    public T? Data { get; set; }
    public Exception? Error { get; set; }

    public Result(T data)
    {
        Data = data;
        Error = null;
    }

    public Result(Exception error)
    {
        Data = null;
        Error = error;
    }
}

public class Maybe
{
    public static async Task<T?> Data<T>(Func<Task<T>> func) where T : class
    {
        try { return await func(); }
        catch { return null; }
    }

    public static T? Data<T>(Func<T> func) where T : class
    {
        try { return func(); }
        catch { return null; }
    }

    public static async Task<Exception?> Error(Func<Task> func)
    {
        try { await func(); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error(Action func)
    {
        try { func(); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<bool> Ok(Func<Task> func)
    {
        try { await func(); return true; }
        catch { return false; }
    }

    public static bool Ok(Action func)
    {
        try { func(); return true; }
        catch { return false; }
    }

    public static async Task<Result<T>> Result<T>(Func<Task<T>> func) where T : class
    {
        try { return new Result<T>(await func()); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static Result<T> Result<T>(Func<T> func) where T : class
    {
        try { return new Result<T>(func()); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static Result<Out> Result<In, Out>(Func<In, Out> func, In input) where Out : class
    {
        try { return new Result<Out>(func(input)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }
}