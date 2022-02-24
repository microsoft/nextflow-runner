using Microsoft.AspNetCore.Components;

namespace NextflowRunnerClient.Components;

public partial class Pipeline
{
    [Parameter]
    public Services.Pipeline Pline { get; set; } = null;

    public static string ConverToLocalDateTimeString(DateTimeOffset utcDateTime)
    {
        var localTime = TimeZoneInfo.ConvertTime(utcDateTime.DateTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);

        return localTime.ToString("g");
    }
}