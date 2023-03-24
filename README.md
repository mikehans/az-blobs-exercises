# az-blobs-exercise
This set of exercises is derived from https://github.com/MicrosoftLearning/AZ-204-DevelopingSolutionsforMicrosoftAzure/blob/master/Instructions/Labs/AZ-204_lab_03.md
and extended with activities to upload a new blob and list all blobs from https://learn.microsoft.com/en-us/training/modules/work-azure-blob-storage/

## Key classes
| Class      | Description                                                                |
|------------|----------------------------------------------------------------------------|
| BlobServiceClient | Gives access to your Azure Storage resources                               |
| BlobContainerClient | Use BlobServiceClient to obtain a reference to a container                 |
| BlobClient | Uses BlobContainerClient to access blobs (individually or as a collection) |
| StorageSharedKeyCredential | Provides the shared key to the BlobServiceClient                           |
