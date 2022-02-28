using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NextflowRunnerClient.Components
{
    public partial class PipelineRun
    {
     [Parameter]
     public Services.PipelineRun Run { get; set; }

        public static string ConverToLocalDateTimeString(DateTimeOffset utcDateTime)
        {
            var localTime = TimeZoneInfo.ConvertTime(utcDateTime.DateTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);

            return localTime.ToString("g");
        }
    }
}
