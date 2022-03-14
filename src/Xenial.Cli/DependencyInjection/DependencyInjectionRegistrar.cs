using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

using System;
using System.Linq;

namespace Xenial.Cli.DependencyInjection;

public class DependencyInjectionRegistrar : ITypeRegistrar, IDisposable
{
    private bool disposedValue;

    private IServiceCollection Services { get; }
    private IList<IDisposable> BuiltProviders { get; }

    public DependencyInjectionRegistrar(IServiceCollection services)
    {
        Services = services;
        BuiltProviders = new List<IDisposable>();
    }

    public ITypeResolver Build()
    {
        var buildServiceProvider = Services.BuildServiceProvider();
        BuiltProviders.Add(buildServiceProvider);
        return new DependencyInjectionResolver(buildServiceProvider);
    }

    public void Register(Type service, Type implementation)
        => Services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation)
        => Services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
        => Services.AddSingleton(service, _ => factory());

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var provider in BuiltProviders)
                {
                    provider.Dispose();
                }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
