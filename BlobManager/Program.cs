using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

const string blobServiceEndpoint = "https://mediastormh8ae.blob.core.windows.net/";
const string storageAccountName = "mediastormh8ae";
const string storageAccountKey =
    "f7GREY+jevqmtY/2mpJWxUQLSEU/6k9aa/cmKohzSV1YUtz8WWSc9ao9XkeUwYahjZzGF9mmBw+S+ASteT4ogA==";

StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);

var client = new BlobServiceClient(new Uri(blobServiceEndpoint), accountCredentials);

var accountInfo = (await client.GetAccountInfoAsync()).Value;

await Console.Out.WriteLineAsync($"Connected to Azure Storage Account");
await Console.Out.WriteLineAsync($"Account name:\t{storageAccountName}");
await Console.Out.WriteLineAsync($"Account kind:\t{accountInfo?.AccountKind}");
await Console.Out.WriteLineAsync($"Account sku:\t{accountInfo?.SkuName}");

await EnumerateContainersAsync(client);
await EnumerateBlobsAsync(client, "raster-graphics");

BlobContainerClient containerClient = await GetContainerAsync(client, "vector-graphics");
await Console.Out.WriteLineAsync($"New container Url:\t{containerClient.Uri}");

BlobClient blobClient = await GetBlobAsync(containerClient, "graph.svg");
await Console.Out.WriteLineAsync($"Blob Url:\t{blobClient.Uri}");

var uploadedBlobName = await UploadBlobAsync(containerClient);

var newBlob = await GetBlobAsync(containerClient, uploadedBlobName);
await Console.Out.WriteLineAsync($"Blob Url:\t {newBlob.Uri}");

await ListAllBlobsInContainerAsync(containerClient);


static async Task EnumerateContainersAsync(BlobServiceClient serviceClient)
{
    await foreach (BlobContainerItem container in serviceClient.GetBlobContainersAsync())
    {
        await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
    }
}

static async Task EnumerateBlobsAsync(BlobServiceClient serviceClient, string containerName)
{
    BlobContainerClient container = serviceClient.GetBlobContainerClient(containerName);
    await Console.Out.WriteLineAsync($"Searching:\t{container.Name}");

    await foreach (BlobItem blob in container.GetBlobsAsync())
    {
        await Console.Out.WriteLineAsync($"Existing Blob:\t{blob.Name}");
    }
}

static async Task<BlobContainerClient> GetContainerAsync(BlobServiceClient serviceClient, string containerName)
{
    BlobContainerClient container = serviceClient.GetBlobContainerClient(containerName);
    await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
    
    await Console.Out.WriteLineAsync($"New Container:\t{container.Name}");
    
    return container;
}

static async Task<BlobClient> GetBlobAsync(BlobContainerClient containerClient, string blobName)
{      
    BlobClient blob = containerClient.GetBlobClient(blobName);
    
    await Console.Out.WriteLineAsync($"Blob Found:\t{blob.Name}");
    
    return blob;
}

static async Task<string> UploadBlobAsync(BlobContainerClient containerClient)
{
    var guidString = Guid.NewGuid().ToString();
    var blobName = "myblob-" + guidString + ".txt";
    var testMessage = "Testing writing some content to a stream. Guid: " + guidString;
    var messageBytes = Encoding.UTF8.GetBytes(testMessage);
    using var stream = new MemoryStream(messageBytes);
    var response = await containerClient.UploadBlobAsync(blobName, stream);
    var rawResponse = response.GetRawResponse();
    await Console.Out.WriteLineAsync($"Uploaded blob status code: {rawResponse.Status} - {rawResponse.ReasonPhrase}");

    return blobName;
}

static async Task ListAllBlobsInContainerAsync(BlobContainerClient containerClient)
{
    await Console.Out.WriteLineAsync($"Blobs in container ({containerClient.Name}):");
    
    var blobs = containerClient.GetBlobsAsync();
    await foreach (var blob in blobs)
    {
        await Console.Out.WriteLineAsync($"\tBlob name: {blob.Name}");
    }
    
}