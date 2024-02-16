# Storage switch

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
