![unator banner](./assets/unator-banner.png)

# Unator

Breaking mistaken standards of enterprise such as Repository pattern.
My goal is to provide a set of functions/classes to make development transparent and logical.

1. [Database](#database--entity-framework-core)
2. [Environment variables](#environment-variables)
3. [Emails](#emails)
4. [Storage](#storage)

## Database / Entity Framework Core

Probably you use Entity Framework Core to access database. I do so.
But you may be taught to use `Repository` pattern and throw exceptions. Not anymore!
I see too few benefits to add complexity of Repository.

So we will use just few generic extensions for database. And our code will look like this:

### Querying data

```csharp
var products = await db.Products
  .Where(x => x.Price < 100)
  .Select(x => new { x.Id, x.Title })
  .QueryMany();

var product = await db.Products
  .Select(x => new { x.Id, x.Title })
  .QueryOne(x => x.Id == 51);
```

`QueryMany` and `QueryOne` allow to get data. You can pass where condition right into function or do it before. Both functions are configured with `ConfigureAwait(false)`. When you query one entity it returns null if entity isn't found.

The benefit is you remove complexity and query minimal amount of data by using `Select`.

### Modifying data

```csharp
var product = new Product("Title for new product");
await db.Products.AddAsync(product)
var saved = await db.Save();
```

`Save` for DbContext allow us to save changes to the database without throwing exceptions. It just returns `true` if changes saved successfully otherwise `false`.

## Environment variables

`Env` provide small set of functions to work with environment variables. Like this:

```csharp
Env.LoadFile("/path/to/.env");
var secret = Env.GetRequired("SECRET");
```

`Env.LoadFile` will set environment variables if file exists.

`Env.GetRequired` get environment variable, but throw if one isn't found. It's rare time when I decided to use throw. I think environment variables should be loaded at the start of program. If variable isn't found then we can't start program.

## Emails

That's where things go interesting. I provide `EmailGod` class, which will manage several email services. You get benefits:

- higher chance of delivering email: 1 service fails, use another one.
- more free plan resources.

At the current moment Unator supports sending emails from:

- Resend <- the best one
- Mailjet
- SendGrid
- Brevo

`EmailGod` implements same interface as email senders. I highly recommend using at least 2 services to be unstoppable.

Code looks:

```csharp
var resendApiKey = Env.Get("RESEND_API_KEY");
var brevoApiKey = Env.Get("BREVO_API_KEY");

var emailGod = new EmailGod(
  new EmailService(new Resend(resendApiKey), new DayLimiter(100)),
  new EmailService(new Brevo(brevoApiKey), new DayLimiter(300)),
);

var emailStatus = await emailGod.Send(
  fromEmail: "example@mail.com",
  fromName: "example",
  toEmails: new List<string>() { "me@mail.com" },
  subject: "Showing EmailGod",
  text: $"It's beautiful",
  html: $"<a href='https://github.com/roman-koshchei/unator'>GitHub</a>"
);

if(emailStatus == EmailStatus.Success) {
  Console.WriteLine("Success");
}
```

We may change it in future if more functionality will be required. For example, move email
information to seperate class/struct to persist it. But it's good for now.

## Storage

Almost all applications nowadays require storage to store images.
I got you. The goal is almost same as EmailGod's goal. We generalize storages to
`IStorage` interface, that allow us to upload and delete files. You will need to store
bucket, key and provider. Because `StorageGod` rely on them to detect how to delete image
from storage.

Services:

- Storj

```csharp
var storageGod = new StorageGod(
  new Storj(accessKey, secretKey, endpoint, publicKey, bucket)
);

var key = $"{Guid.New().ToString()}{fileExtension}";
var storageFile = await storageGod.Upload(key, fileStream);
if(storageFile == null) return;

var deleted = await storageGod.Delete(storageFile);
if(deleted) Console.WriteLine("File deleted");
```

StorageGod is new thing, so not many services are connected.
