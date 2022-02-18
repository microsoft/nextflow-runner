using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using NextflowRunnerClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;


namespace NextflowRunnerClient.Components
{
    public partial class PipelineParam
    {
        [Parameter]
        public Services.PipelineParam Param { get; set; } = new Services.PipelineParam();

        [Parameter]
        public string ParamValue { get; set; } = "";

        public bool Valid { get; set; } = false;

        public void ValidateParam()
        {
            Valid = false;
            if (ParamValue.Length > 3)
            {
                Valid = true;
            }
          
          
          
        }

      
    }
}
