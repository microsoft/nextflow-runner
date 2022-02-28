namespace NextflowRunner.API.Models
{
    public class AzureContainerOptions
    {
        internal const string ConfigSection = "AzureStorage";
        public string AZURE_STORAGE_SAS { get; set; } = string.Empty;
        public string AZURE_STORAGE_ACCOUNTNAME { get; set; } = string.Empty;
        public string AZURE_STORAGE_KEY { get; set; } = string.Empty;
    }
}