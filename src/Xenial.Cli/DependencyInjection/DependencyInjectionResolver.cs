using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.DependencyInjection;

internal class DependencyInjectionResolver : ITypeResolver, IDisposable
{
    private ServiceProvider ServiceProvider { get; }

    internal DependencyInjectionResolver(ServiceProvider serviceProvider) 
        => ServiceProvider = serviceProvider;

    public void Dispose() => 
        ServiceProvider.Dispose();

    public object? Resolve(Type? type) 
        => ServiceProvider.GetService(type) ?? Activator.CreateInstance(type);
}
