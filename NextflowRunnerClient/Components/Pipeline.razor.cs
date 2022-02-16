using Microsoft.AspNetCore.Components;

namespace NextflowRunnerClient.Components
{
    public partial class Pipeline
    {
        [Parameter]
        public NextflowRunnerClient.Services.Pipeline Pline { get; set; } = null;
        
    }
}
