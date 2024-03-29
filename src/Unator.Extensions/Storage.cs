﻿using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Immutable;
using System.Net;

namespace Unator.Extensions;

/// <summary>
/// Provider is used for migrations, multistorage usage, creating link to the file.
/// </summary>
public record StorageFile(string Key, string Bucket, string Provider);

public interface IStorageFileContainer
{
    public StorageFile GetStorageFile();
}

public interface IStorage
{
    /// <summary>Upload file to a storage.</summary>
    /// <param name="key">Key/path to item, including file extension.</param>
    /// <param name="fileStream">Stream of file</param>
    /// <returns>
    /// StorageFile, which contains information required to delete file.
    /// Or NULL if exception appeared.
    /// </returns>
    public Task<StorageFile?> Upload(string key, Stream fileStream);

    /// <summary>Delete file from storage.</summary>
    /// <returns>True if success, false if failed</returns>
    public Task<bool> Delete(StorageFile file);
}

public class StorageSwitch : IStorage
{
    private readonly IImmutableList<IStorage> storages;

    public StorageSwitch(IStorage storage, params IStorage[] storages)
    {
        var list = new List<IStorage>(storages.Length + 1) { storage };
        list.AddRange(storages);
        this.storages = list.ToImmutableArray();
    }

    public async Task<bool> Delete(StorageFile file)
    {
        for (int i = 0; i < storages.Count; ++i)
        {
            var storage = storages[i];
            var deleted = await storage.Delete(file);
            if (deleted) return true;
        }
        return false;
    }

    public async Task<StorageFile?> Upload(string key, Stream fileStream)
    {
        for (int i = 0; i < storages.Count; ++i)
        {
            var storage = storages[i];
            var file = await storage.Upload(key, fileStream);
            if (file != null) return file;
        }
        return null;
    }
}

/// <summary>Handle files inside storage of Storj service.</summary>
public class Storj : IStorage
{
    /// <summary>Never change this value. It's key for this provider.</summary>
    private const string provider = "storj";

    private readonly AmazonS3Client client;
    private readonly string publicKey;
    private readonly string bucket;

    /// <param name="accessKey">Access key to S3 compatible gateway.</param>
    /// <param name="secretKey">Secret key to S3 compatible gateway.</param>
    /// <param name="endpoint">Endpoint to S3 compatible gateway.</param>
    /// <param name="publicKey">Public key to create url to a public file.</param>
    public Storj(string accessKey, string secretKey, string endpoint, string publicKey, string bucket)
    {
        var config = new AmazonS3Config() { ServiceURL = endpoint };
        client = new AmazonS3Client(accessKey, secretKey, config);
        this.publicKey = publicKey;
        this.bucket = bucket;
    }

    public async Task<StorageFile?> Upload(string key, Stream fileStream)
    {
        try
        {
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = fileStream,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await client.PutObjectAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK) return null;
            return new StorageFile(bucket, key, provider);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> Delete(StorageFile file)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = file.Bucket,
                Key = file.Key
            };
            var response = await client.DeleteObjectAsync(request);
            return Http.IsSuccessful(response.HttpStatusCode);
        }
        catch
        {
            return false;
        }
    }

    public string Url(StorageFile file)
    {
        return $"https://link.storjshare.io/raw/{publicKey}/{file.Bucket}/{file.Key}";
    }
}