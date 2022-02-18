using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace NextflowRunnerClient.Components
{
    public partial class Pipeline
    {
        [Parameter]
        [NotNull]
        public NextflowRunnerClient.Services.Pipeline? Pline { get; set; } = null;
        
    }
}
