namespace NextflowRunner.API.Models
{
    public class ExecutionRequest
    {
        public string RunName { get; set; }
        public IDictionary<int, string> Parameters { get; set; } 
    }
}
