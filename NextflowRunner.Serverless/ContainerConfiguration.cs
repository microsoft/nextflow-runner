﻿namespace NextflowRunner.Serverless;

public class ContainerConfiguration
{
    public const string ConfigSection = "ContainerConfiguration";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
    public string ResourceGroupName { get; set; } = string.Empty;

    public string StorageName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string BatchRegion { get; set; } = string.Empty;
    public string BatchAccountName { get; set; } = string.Empty;
    public string BatchKey { get; set; } = string.Empty;
}