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

    public static async Task<T?> Data<In1, T>(Func<In1, Task<T>> func, In1 input1) where T : class
    {
        try { return await func(input1); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, T>(
        Func<In1, In2, Task<T>> func, In1 input1, In2 input2
    ) where T : class
    {
        try { return await func(input1, input2); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, In3, T>(
        Func<In1, In2, In3, Task<T>> func, In1 input1, In2 input2, In3 input3
    ) where T : class
    {
        try { return await func(input1, input2, input3); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, In3, In4, T>(
        Func<In1, In2, In3, In4, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    ) where T : class
    {
        try { return await func(input1, input2, input3, input4); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, In3, In4, In5, T>(
        Func<In1, In2, In3, In4, In5, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    ) where T : class
    {
        try { return await func(input1, input2, input3, input4, input5); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, In3, In4, In5, In6, T>(
        Func<In1, In2, In3, In4, In5, In6, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    ) where T : class
    {
        try { return await func(input1, input2, input3, input4, input5, input6); }
        catch { return null; }
    }

    public static async Task<T?> Data<In1, In2, In3, In4, In5, In6, In7, T>(
        Func<In1, In2, In3, In4, In5, In6, In7, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    ) where T : class
    {
        try { return await func(input1, input2, input3, input4, input5, input6, input7); }
        catch { return null; }
    }

    public static T? Data<In1, T>(Func<In1, T> func, In1 input1) where T : class
    {
        try { return func(input1); }
        catch { return null; }
    }

    public static T? Data<In1, In2, T>(
        Func<In1, In2, T> func, In1 input1, In2 input2
    ) where T : class
    {
        try { return func(input1, input2); }
        catch { return null; }
    }

    public static T? Data<In1, In2, In3, T>(
        Func<In1, In2, In3, T> func, In1 input1, In2 input2, In3 input3
    ) where T : class
    {
        try { return func(input1, input2, input3); }
        catch { return null; }
    }

    public static T? Data<In1, In2, In3, In4, T>(
        Func<In1, In2, In3, In4, T> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    ) where T : class
    {
        try { return func(input1, input2, input3, input4); }
        catch { return null; }
    }

    public static T? Data<In1, In2, In3, In4, In5, T>(
        Func<In1, In2, In3, In4, In5, T> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    ) where T : class
    {
        try { return func(input1, input2, input3, input4, input5); }
        catch { return null; }
    }

    public static T? Data<In1, In2, In3, In4, In5, In6, T>(
        Func<In1, In2, In3, In4, In5, In6, T> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    ) where T : class
    {
        try { return func(input1, input2, input3, input4, input5, input6); }
        catch { return null; }
    }

    public static T? Data<In1, In2, In3, In4, In5, In6, In7, T>(
        Func<In1, In2, In3, In4, In5, In6, In7, T> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    ) where T : class
    {
        try { return func(input1, input2, input3, input4, input5, input6, input7); }
        catch { return null; }
    }

    // ERROR

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

    public static async Task<Exception?> Error<In1>(Func<In1, Task> func, In1 input1)
    {
        try { await func(input1); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2>(Func<In1, In2, Task> func, In1 input1, In2 input2)
    {
        try { await func(input1, input2); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2, In3>(
        Func<In1, In2, In3, Task> func, In1 input1, In2 input2, In3 input3
    )
    {
        try { await func(input1, input2, input3); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2, In3, In4>(
        Func<In1, In2, In3, In4, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    )
    {
        try { await func(input1, input2, input3, input4); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2, In3, In4, In5>(
        Func<In1, In2, In3, In4, In5, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    )
    {
        try { await func(input1, input2, input3, input4, input5); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2, In3, In4, In5, In6>(
        Func<In1, In2, In3, In4, In5, In6, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    )
    {
        try { await func(input1, input2, input3, input4, input5, input6); return null; }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exception?> Error<In1, In2, In3, In4, In5, In6, In7>(
        Func<In1, In2, In3, In4, In5, In6, In7, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    )
    {
        try { await func(input1, input2, input3, input4, input5, input6, input7); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1>(Action<In1> func, In1 input1)
    {
        try { func(input1); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2>(Action<In1, In2> func, In1 input1, In2 input2)
    {
        try { func(input1, input2); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2, In3>(
        Action<In1, In2, In3> func, In1 input1, In2 input2, In3 input3
    )
    {
        try { func(input1, input2, input3); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2, In3, In4>(
        Action<In1, In2, In3, In4> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    )
    {
        try { func(input1, input2, input3, input4); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2, In3, In4, In5>(
        Action<In1, In2, In3, In4, In5> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    )
    {
        try { func(input1, input2, input3, input4, input5); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2, In3, In4, In5, In6>(
        Action<In1, In2, In3, In4, In5, In6> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    )
    {
        try { func(input1, input2, input3, input4, input5, input6); return null; }
        catch (Exception ex) { return ex; }
    }

    public static Exception? Error<In1, In2, In3, In4, In5, In6, In7>(
        Action<In1, In2, In3, In4, In5, In6, In7> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    )
    {
        try { func(input1, input2, input3, input4, input5, input6, input7); return null; }
        catch (Exception ex) { return ex; }
    }

    // OK

    public static async Task<bool> Ok(Func<Task> func)
    {
        try { await func(); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1>(Func<In1, Task> func, In1 input1)
    {
        try { await func(input1); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2>(Func<In1, In2, Task> func, In1 input1, In2 input2)
    {
        try { await func(input1, input2); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2, In3>(
        Func<In1, In2, In3, Task> func, In1 input1, In2 input2, In3 input3
    )
    {
        try { await func(input1, input2, input3); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2, In3, In4>(
        Func<In1, In2, In3, In4, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    )
    {
        try { await func(input1, input2, input3, input4); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2, In3, In4, In5>(
        Func<In1, In2, In3, In4, In5, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    )
    {
        try { await func(input1, input2, input3, input4, input5); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2, In3, In4, In5, In6>(
        Func<In1, In2, In3, In4, In5, In6, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    )
    {
        try { await func(input1, input2, input3, input4, input5, input6); return true; }
        catch { return false; }
    }

    public static async Task<bool> Ok<In1, In2, In3, In4, In5, In6, In7>(
        Func<In1, In2, In3, In4, In5, In6, In7, Task> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    )
    {
        try { await func(input1, input2, input3, input4, input5, input6, input7); return true; }
        catch { return false; }
    }

    public static bool Ok(Action func)
    {
        try { func(); return true; }
        catch { return false; }
    }

    public static bool Ok<In1>(Action<In1> func, In1 input1)
    {
        try { func(input1); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2>(Action<In1, In2> func, In1 input1, In2 input2)
    {
        try { func(input1, input2); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2, In3>(Action<In1, In2, In3> func, In1 input1, In2 input2, In3 input3)
    {
        try { func(input1, input2, input3); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2, In3, In4>(
        Action<In1, In2, In3, In4> func, In1 input1, In2 input2, In3 input3, In4 input4
        )
    {
        try { func(input1, input2, input3, input4); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2, In3, In4, In5>(
        Action<In1, In2, In3, In4, In5> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
        )
    {
        try { func(input1, input2, input3, input4, input5); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2, In3, In4, In5, In6>(
        Action<In1, In2, In3, In4, In5, In6> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
        )
    {
        try { func(input1, input2, input3, input4, input5, input6); return true; }
        catch { return false; }
    }

    public static bool Ok<In1, In2, In3, In4, In5, In6, In7>(
        Action<In1, In2, In3, In4, In5, In6, In7> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
        )
    {
        try { func(input1, input2, input3, input4, input5, input6, input7); return true; }
        catch { return false; }
    }

    // RESULT

    public static async Task<Result<T>> Result<T>(Func<Task<T>> func) where T : class
    {
        try { return new Result<T>(await func()); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, T>(
        Func<In1, In2, Task<T>> func, In1 input1, In2 input2
        ) where T : class
    {
        try { return new Result<T>(await func(input1, input2)); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, In3, T>(
        Func<In1, In2, In3, Task<T>> func, In1 input1, In2 input2, In3 input3
        ) where T : class
    {
        try { return new Result<T>(await func(input1, input2, input3)); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, In3, In4, T>(
        Func<In1, In2, In3, In4, Task<T>> func, In1 input1, In2 input2, In3 input3, In4 input4
        ) where T : class
    {
        try { return new Result<T>(await func(input1, input2, input3, input4)); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, In3, In4, In5, T>(
        Func<In1, In2, In3, In4, In5, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
        ) where T : class
    {
        try { return new Result<T>(await func(input1, input2, input3, input4, input5)); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, In3, In4, In5, In6, T>(
        Func<In1, In2, In3, In4, In5, In6, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
        ) where T : class
    {
        try { return new Result<T>(await func(input1, input2, input3, input4, input5, input6)); }
        catch (Exception ex) { return new Result<T>(ex); }
    }

    public static async Task<Result<T>> Result<In1, In2, In3, In4, In5, In6, In7, T>(
        Func<In1, In2, In3, In4, In5, In6, In7, Task<T>> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    ) where T : class
    {
        try { return new Result<T>(await func(input1, input2, input3, input4, input5, input6, input7)); }
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

    public static Result<Out> Result<In1, In2, Out>(
        Func<In1, In2, Out> func, In1 input1, In2 input2
        ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }

    public static Result<Out> Result<In1, In2, In3, Out>(
        Func<In1, In2, In3, Out> func, In1 input1, In2 input2, In3 input3
        ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2, input3)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }

    public static Result<Out> Result<In1, In2, In3, In4, Out>(
        Func<In1, In2, In3, In4, Out> func,
        In1 input1, In2 input2, In3 input3, In4 input4
    ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2, input3, input4)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }

    public static Result<Out> Result<In1, In2, In3, In4, In5, Out>(
        Func<In1, In2, In3, In4, In5, Out> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5
    ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2, input3, input4, input5)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }

    public static Result<Out> Result<In1, In2, In3, In4, In5, In6, Out>(
        Func<In1, In2, In3, In4, In5, In6, Out> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6
    ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2, input3, input4, input5, input6)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }

    public static Result<Out> Result<In1, In2, In3, In4, In5, In6, In7, Out>(
        Func<In1, In2, In3, In4, In5, In6, In7, Out> func,
        In1 input1, In2 input2, In3 input3, In4 input4, In5 input5, In6 input6, In7 input7
    ) where Out : class
    {
        try { return new Result<Out>(func(input1, input2, input3, input4, input5, input6, input7)); }
        catch (Exception ex) { return new Result<Out>(ex); }
    }
}