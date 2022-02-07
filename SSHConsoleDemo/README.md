# SSH Console Demo
.NET 6 console application to demonstrate usage of SSH.NET SSHClient connecting to remote Azure VM.

## Getting Started
1. Clone the repo to your local machine
1. Update `Program.cs` with:
	- Location of your private-key file to connect with Azure VM ([Generate SSH Keys](https://docs.microsoft.com/en-us/azure/virtual-machines/ssh-keys-portal))
	- IP address of Azure VM
	- Azure VM username

``` C#
using Renci.SshNet;

var privateKey = new PrivateKeyFile("privatekey.pem");
var sshClient = new SshClient("1.2.3.4", "username", privateKey);
try
{
    sshClient.Connect();
    using var command = sshClient.CreateCommand("./nextflow ./hello/main.nf");
    Console.Write(command.Execute());
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    sshClient.Disconnect();
    sshClient.Dispose();
}

Console.ReadLine();
```
3. Run the app by pressing "F5"
