﻿
using StreamJsonRpc;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

using System.Linq;

using System.Threading.Tasks;

using Xenial.Design;

#pragma warning disable CA1031 // Do not catch general exception types
try
{
    var connectionId = args[0];

    var debug = args.Length > 1 ? bool.Parse(args[1]) : false;

    await NamedPipeServerAsync(connectionId, debug);

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    return 1;
}
#pragma warning restore CA1031 // Do not catch general exception types


static async Task NamedPipeServerAsync(string connectionId, bool debug)
{
    var clientId = 0;
    while (true)
    {
        await Console.Error.WriteLineAsync("Waiting for client to make a connection...");
        var stream = new NamedPipeServerStream(connectionId, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        await stream.WaitForConnectionAsync();
        var shouldDispose = await RespondToRpcRequestsAsync(stream, ++clientId, debug);
        if (shouldDispose)
        {
            Console.WriteLine($"Exiting designer process #{Process.GetCurrentProcess().Id}.");
            stream.Disconnect();
            stream.Dispose();
            break;
        }
    }
}

static async Task<bool> RespondToRpcRequestsAsync(Stream stream, int clientId, bool debug)
{
    Console.WriteLine($"Connection request #{clientId} received. Spinning off an async Task to cater to requests.");

    var server = new ModelEditorServer
    {
        Debug = debug
    };

    var jsonRpc = JsonRpc.Attach(stream, server);

    server.JsonRpc = jsonRpc;

    Console.WriteLine($"JSON-RPC listener attached to #{clientId}. Waiting for requests...");
    await jsonRpc.Completion;
    Console.WriteLine($"Connection #{clientId} terminated.");
    return true;
}
