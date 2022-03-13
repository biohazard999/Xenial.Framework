using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DevExpress.Persistent.Base;

namespace Xenial.Framework.Deeplinks;

/// <summary>
///     Enforces single instance for an application.
/// </summary>
public class DeeplinkSingleInstance : IDisposable
{
    private const int timeout = 200;

    private readonly bool ownsMutex;
    private readonly string identifier;
    private readonly Mutex mutex;

    /// <summary>
    ///     Enforces single instance for an application.
    /// </summary>
    /// <param name="identifier">An identifier unique to this application.</param>
    public DeeplinkSingleInstance(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentNullException(nameof(identifier));
        }

        //We only do a local mutex to support TerminalServer sessions correctly
        this.identifier = $"Local\\{identifier.ToString(CultureInfo.InvariantCulture)}";

        //TODO: Mutex Watcher -> After last mutext gets released, we are the last instance
        mutex = new Mutex(true, this.identifier, out ownsMutex);

        if (Tracing.IsTracerInitialized)
        {
            Tracing.Tracer.LogText($"Created Mutex {this.identifier} (IsMutexOwner: {ownsMutex})");
        }
    }

    /// <summary>
    ///     Indicates whether this is the first instance of this application.
    /// </summary>
    public bool IsFirstInstance => ownsMutex;

    /// <summary>
    ///     Passes the given arguments to the first running instance of the application.
    /// </summary>
    /// <param name="arguments">The arguments to pass.</param>
    /// <returns>Return true if the operation succeded, false otherwise.</returns>
    private async Task<bool> PassArgumentsToFirstInstanceAsync(string[] arguments)
    {
        if (arguments is null)
        {
            return false;
        }

        if (!ownsMutex)
        {
            try
            {
                using var client = new NamedPipeClientStream(identifier);
                using var writer = new StreamWriter(client);

                await client.ConnectAsync(timeout).ConfigureAwait(false);

                foreach (var argument in arguments)
                {
                    if (!string.IsNullOrEmpty(argument))
                    {
                        await writer.WriteLineAsync(argument).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (TimeoutException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Couldn't connect to server
            catch (IOException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Pipe was broken
            catch (InvalidOperationException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Pipe was broken
        }

        return false;
    }

    /// <summary>
    ///     Passes the given arguments to the first running instance of the application.
    /// </summary>
    /// <param name="arguments">The arguments to pass.</param>
    /// <returns>Return true if the operation succeded, false otherwise.</returns>
    private bool PassArgumentsToFirstInstance(string[] arguments)
    {
        if (arguments is null)
        {
            return false;
        }

        if (!ownsMutex)
        {
            try
            {
                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogText($"Try to open named pipe {identifier} for writing arguments {string.Join(" ", arguments)}");
                }

                using var client = new NamedPipeClientStream(identifier);
                using var writer = new StreamWriter(client);

                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogText($"Try connect to named pipe {identifier} for writing arguments {string.Join(" ", arguments)}");
                }
                client.Connect(timeout);

                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogText($"Connected to named pipe {identifier} for writing arguments {string.Join(" ", arguments)}");
                }
                foreach (var argument in arguments)
                {
                    if (!string.IsNullOrEmpty(argument))
                    {
                        writer.WriteLine(argument);
                    }
                }
                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogText($"Written arguments to named pipe {identifier} {string.Join(" ", arguments)}");
                }
                return true;

            }
            catch (TimeoutException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Couldn't connect to server
            catch (IOException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Pipe was broken
            catch (InvalidOperationException ex)
            {
                LogError(ex, arguments);
                return false;
            } //Pipe was broken
        }

        return false;
    }

    private void LogError(Exception ex, IEnumerable<string> arguments)
    {
        if (Tracing.IsTracerInitialized)
        {
            Tracing.Tracer.LogError($"Failed on named pipe {identifier} arguments: {string.Join(" ", arguments)}");
            Tracing.Tracer.LogError(ex);
        }
    }

    /// <summary>
    ///     Listens for arguments being passed from successive instances of the applicaiton.
    /// </summary>
    public void ListenForArgumentsFromSuccessiveInstances()
    {
        if (ownsMutex)
        {
            ThreadPool.QueueUserWorkItem(async _ => await ListenForArgumentsAsync().ConfigureAwait(false), null);
        }
    }

    /// <summary>
    ///     Listens for arguments on a named pipe.
    /// </summary>
    private async Task ListenForArgumentsAsync()
    {
        if (ownsMutex)
        {
            try
            {
                using var server = new NamedPipeServerStream(identifier);
                using var reader = new StreamReader(server);

                await server.WaitForConnectionAsync().ConfigureAwait(false);

                var arguments = new List<string>();
                while (server.IsConnected)
                {
                    var result = await reader.ReadLineAsync().ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(result))
                    {
                        arguments.Add(result);
                    }
                }

                ThreadPool.QueueUserWorkItem(CallOnArgumentsReceived, arguments.ToArray());
            }
            catch (IOException ex)
            {
                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogError(ex);
                }
            } //Pipe was broken
            finally
            {
                await ListenForArgumentsAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Calls the OnArgumentsReceived method casting the state Object to String[].
    /// </summary>
    /// <param name="state">The arguments to pass.</param>
    private void CallOnArgumentsReceived(object? state)
    {
        if (state is string[] args)
        {
            OnArgumentsReceived(args);
        }
    }

    private static readonly object locker = new();
    private EventHandler<DeeplinkArgumentsReceivedEventArgs>? argumentsReceived;

    /// <summary>
    ///     Event raised when arguments are received from successive instances.
    /// </summary>
    public event EventHandler<DeeplinkArgumentsReceivedEventArgs> ArgumentsReceived
    {
        add
        {
            lock (locker)
            {
                if (ownsMutex)
                {
                    argumentsReceived += value;
                }
            }

        }
        remove
        {
            lock (locker)
            {
                if (argumentsReceived is not null && value is not null)
                {
                    var args = argumentsReceived;
                    args -= value;
                    if (args is not null)
                    {
                        argumentsReceived = args;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Fires the ArgumentsReceived event.
    /// </summary>
    /// <param name="arguments">The arguments to pass with the ArgumentsReceivedEventArgs.</param>
    protected virtual void OnArgumentsReceived(string[] arguments)
    {
        _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

        var eventArgs = new DeeplinkArgumentsReceivedEventArgs();
        foreach (var argument in arguments)
        {
            eventArgs.Arguments.Add(argument);
        }

        argumentsReceived?.Invoke(this, eventArgs);
    }


    private bool disposed;

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            if (mutex is not null && ownsMutex)
            {

                if (Tracing.IsTracerInitialized)
                {
                    Tracing.Tracer.LogText($"Release and disposal of mutex {identifier} (IsMutexOwner: {ownsMutex})");
                }

                mutex.ReleaseMutex();
            }
            disposed = true;
        }
        mutex?.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    ~DeeplinkSingleInstance()
        => Dispose(false);

    /// <summary>
    /// 
    /// </summary>
    public Task<bool> PassArgumentsToFirstInstanceAsync()
    {
        var arguments = OnQueryArguments();
        return PassArgumentsToFirstInstanceAsync(arguments);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool PassArgumentsToFirstInstance()
    {
        var arguments = OnQueryArguments();
        return PassArgumentsToFirstInstance(arguments);
    }

    /// <summary>
    /// 
    /// </summary>
    public static event EventHandler<DeeplinkQueryArgumentsEventArgs>? QueryArguments;

    private string[] OnQueryArguments()
    {
        var environmentArgs = Environment.GetCommandLineArgs();

        if (environmentArgs.Length >= 2) //Normally the fist argument is the application name
        {
            environmentArgs = environmentArgs.Skip(1).ToArray();
        }

        var arguments = new DeeplinkQueryArgumentsEventArgs(environmentArgs);

        QueryArguments?.Invoke(this, arguments);

        if (arguments.Handled)
        {
            return arguments.Arguments.ToArray();
        }

        return environmentArgs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string[] GetArguments() => OnQueryArguments();
}
