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

