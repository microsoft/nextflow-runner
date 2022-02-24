﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using System.Threading;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Azure;
using System.Globalization;
using Meziantou.Framework;

namespace NextflowRunnerClient.Pages
{
    public partial class FileUpload
    {
        public bool Valid { get; set; } = false;
        public string BlobURI { get; set; } = "";

        int count = 1;
        string status;
        bool fileSelected = false;
        string uploadedFile;
        private int maxAllowedSize = 200 * 1024 * 1024;

        List<FileUploadProgress> uploadedFiles = new();

        private void UpdateStatus(ChangeEventArgs e)
        {
            status = e.Value.ToString();
            count = count + 1;
            StateHasChanged();
        }

        async Task UploadFiletoAzBlobStorage(InputFileChangeEventArgs e)
        {
            string AzureStorageConnectionSAS = "[required to upload files]";
            string AzureStorageAccountName = "[required to upload files]";

            var maxAllowedFiles = 3;
            var startIndex = uploadedFiles.Count;
            var watch = new System.Diagnostics.Stopwatch();

            //var files = e.GetMultipleFiles(maxAllowedFiles).First();
            var files = e.GetMultipleFiles(maximumFileCount: maxAllowedFiles);

            if (files != null)
            {
                fileSelected = true;

                // Add all files to the UI
                foreach (var file in files)
                {
                    var progress = new FileUploadProgress(file.Name, file.Size);
                    uploadedFiles.Add(progress);
                }

                // We don't want to refresh the UI too frequently,
                // So, we use a timer to update the UI every few hundred milliseconds
                await using var timer = new Timer(_ => InvokeAsync(() => StateHasChanged()));
                timer.Change(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));

                // Upload files
                try
                {
                    foreach (var file in files)  // Coded to support multiple file uploads in single control in the future
                    {
                        var blobUri = new Uri("https://" +
                                                AzureStorageAccountName +
                                                ".blob.core.windows.net/" +
                                                "nextflowcontainer" + "/" + file.Name);

                        AzureSasCredential credential = new AzureSasCredential(AzureStorageConnectionSAS);
                        BlobClient blobClient = new BlobClient(blobUri, credential, new BlobClientOptions());

                        watch.Start();  //set timmer for debugging upload time

                        var res = await blobClient.UploadAsync(file.OpenReadStream(maxAllowedSize), new BlobUploadOptions
                        {
                            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
                            TransferOptions = new StorageTransferOptions
                            {
                                InitialTransferSize = 1024 * 1024,
                                MaximumConcurrency = 10
                            },
                            ProgressHandler = new Progress<long>((progress) =>
                            {

                                uploadedFiles[startIndex].UploadedBytes = progress;
                            })

                        });

                        watch.Stop();  //stop timmer for debugging upload time

                        BlobURI = blobUri.ToString();  //Set Property (file w/ path) to be used by parent page
                        uploadedFile = "<p>Uploaded File Path: </p><p>" + BlobURI + "</p>";  //Set var do display on control page

                        //Used for debuging - revert to empty string when not debuging.
                        //status = "";
                        status = $"Finished loading {file.Size / 1024 / 1024} MB from {file.Name} in {TimeSpan.FromMilliseconds(watch.Elapsed.TotalMilliseconds).TotalMinutes} minutes.";

                        startIndex++;
                        watch.Reset();
                    }
                }
                finally
                {
                    fileSelected = false;
                    StateHasChanged();
                }
            }

        }

        record FileUploadProgress(string FileName, long Size)
        {
            public long UploadedBytes { get; set; }
            public double UploadedPercentage => (double)UploadedBytes / (double)Size * 100d;
        }

        string FormatBytes(long value)
           => ByteSize.FromByte(value).ToString("fi2", CultureInfo.CurrentCulture);

    }
}