using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NextflowRunnerClient.Models;
using NextflowRunnerClient.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Azure;
using System.Globalization;
using Meziantou.Framework;

namespace NextflowRunnerClient.Components;

public partial class PipelineParamFile
{
    [Parameter]
    public ViewParam Param { get; set; } = new ();

    public string AzKey { get; set; }

    [Inject]
    protected NextflowAPI NfAPI { get; set; }

    public bool Valid { get; set; } = false;

    string AzureStorageConnectionSAS = "";
    string AzureStorageAccountName = "";


    int count = 1;
    string status;
    bool fileSelected = false;
    string uploadedFileDetails;
    string fileName;
    private readonly int maxAllowedSize = 200 * 1024 * 1024;

    List<FileUploadProgress> uploadedFiles = new();


    protected override void OnInitialized()
    {
        Param.Value = Param.ParamExample;
    }


    private void UpdateStatus(ChangeEventArgs e)
    {
        status = e.Value.ToString();
        count++;
        StateHasChanged();
    }

    private void UpateAZKey(ChangeEventArgs e)
    {
        AzKey = e.Value.ToString();
    }


    private async Task ValidateAZKey(string azKey)
    {
        if (azKey != "")
        {
            var azureContainerOptions = await NfAPI.GetAzureContainerOptionsAsync(azKey);

            AzureStorageConnectionSAS = azureContainerOptions.AzurE_STORAGE_SAS;
            AzureStorageAccountName = azureContainerOptions.AzurE_STORAGE_ACCOUNTNAME;            
        }
    }

    async Task UploadFiletoAzBlobStorage(InputFileChangeEventArgs e)
    {
        await ValidateAZKey(AzKey);

        if (AzureStorageConnectionSAS != "")  //Will be populated by UpdateAZKey
        {
            var maxAllowedFiles = 1;
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
                        fileName = string.Format(@"{0}" + Path.GetExtension(file.Name), Guid.NewGuid());
                        var blobUri = new Uri("https://" +
                                                AzureStorageAccountName +
                                                ".blob.core.windows.net/" +
                                                "nextflowcontainer" + "/" + fileName);

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

                        Param.Value = blobUri.ToString(); //Set Property (file w/ path) to be used by parent page
                        uploadedFileDetails = "<p>Uploaded File Path: </p><p>" + Param.Value + "</p>";  //Set var do display on control page

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
        else
        {
            status = "Please provide a valid security key!";
            fileSelected = false;
            StateHasChanged();
        }    
    }

    record FileUploadProgress(string FileName, long Size)
    {
        public long UploadedBytes { get; set; }
        public double UploadedPercentage => (double)UploadedBytes / (double)Size * 100d;
    }

    static string FormatBytes(long value)
       => ByteSize.FromByte(value).ToString("fi2", CultureInfo.CurrentCulture);



}