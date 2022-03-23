
using Spectre.Console;

using StreamJsonRpc;

using System;
using System.IO;
using System.IO.Pipes;

using System.Linq;

using System.Threading.Tasks;

using Xenial.Design;

try
{
    var connectionId = args[0];

    await NamedPipeServerAsync(connectionId);

    return 0;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    return 1;
}


static async Task NamedPipeServerAsync(string connectionId)
{
    var clientId = 0;
    while (true)
    {
        await Console.Error.WriteLineAsync("Waiting for client to make a connection...");
        using (var stream = new NamedPipeServerStream(connectionId, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
        {
            await stream.WaitForConnectionAsync();
            _ = RespondToRpcRequestsAsync(stream, ++clientId);
        }
    }
}

static async Task RespondToRpcRequestsAsync(Stream stream, int clientId)
{
    AnsiConsole.WriteLine($"Connection request #{clientId} received. Spinning off an async Task to cater to requests.");
    var jsonRpc = JsonRpc.Attach(stream, new Server());
    AnsiConsole.WriteLine($"JSON-RPC listener attached to #{clientId}. Waiting for requests...");
    await jsonRpc.Completion;
    AnsiConsole.WriteLine($"Connection #{clientId} terminated.");
}

