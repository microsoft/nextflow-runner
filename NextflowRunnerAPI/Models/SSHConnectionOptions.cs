namespace NextflowRunner.API.Models
{
    public class SSHConnectionOptions
    {
        internal const string ConfigSection = "SSHConnection";
        public string VM_ADMIN_HOSTNAME { get; set; } = string.Empty;
        public string VM_ADMIN_USERNAME { get; set; } = string.Empty;
        public string VM_ADMIN_PASSWORD { get; set; } = string.Empty;
    }
}
