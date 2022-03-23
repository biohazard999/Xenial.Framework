
using StreamJsonRpc;

using System;
using System.IO;
using System.IO.Pipes;

using System.Linq;

using System.Threading.Tasks;

using Xenial.Design;

#pragma warning disable CA1031 // Do not catch general exception types
try
{
    var connectionId = args[0];

    await NamedPipeServerAsync(connectionId);

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    return 1;
}
#pragma warning restore CA1031 // Do not catch general exception types


static async Task NamedPipeServerAsync(string connectionId)
{
    var clientId = 0;
    while (true)
    {
        await Console.Error.WriteLineAsync("Waiting for client to make a connection...");
        var stream = new NamedPipeServerStream(connectionId, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        await stream.WaitForConnectionAsync();
        _ = RespondToRpcRequestsAsync(stream, ++clientId);
    }
}

static async Task RespondToRpcRequestsAsync(Stream stream, int clientId)
{
    Console.WriteLine($"Connection request #{clientId} received. Spinning off an async Task to cater to requests.");
    var jsonRpc = JsonRpc.Attach(stream, new Server());
    Console.WriteLine($"JSON-RPC listener attached to #{clientId}. Waiting for requests...");
    await jsonRpc.Completion;
    Console.WriteLine($"Connection #{clientId} terminated.");
}

