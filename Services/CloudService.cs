using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoBooth.Services
{
    class CloudService
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        public CloudService()
        {
            // Retrieve storage account from connection string.
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            _blobClient = _storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            _container = _blobClient.GetContainerReference("albums");

            // Create the container if it doesn't already exist.
            _container.CreateIfNotExists();

            _container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public void Upload(string filePath)
        {
            // Retrieve reference to a blob named like given filename.
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(filePath);

            // Create or overwrite the blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }
    }
}
