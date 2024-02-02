# Environment variables

`Env` provide small set of functions to work with environment variables. Like this:

```csharp
Env.LoadFile("/path/to/.env");
var secret = Env.GetRequired("SECRET");
```

`Env.LoadFile` will set environment variables if file exists.

`Env.GetRequired` get environment variable, but throw if one isn't found. It's rare time when I decided to use throw. I think environment variables should be loaded at the start of program. If variable isn't found then we can't start program.
