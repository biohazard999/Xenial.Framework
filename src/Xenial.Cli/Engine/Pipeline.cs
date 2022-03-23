using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelToCodeConverter.Engine;

public record PipelineContext() : IDisposable
{
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
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

public delegate Task PipelineMiddleware<TContext>(TContext context)
    where TContext : PipelineContext;

public abstract record Pipeline<TContext>
    where TContext : PipelineContext
{
    private readonly IList<Func<PipelineMiddleware<TContext>, PipelineMiddleware<TContext>>> middlewares = new List<Func<PipelineMiddleware<TContext>, PipelineMiddleware<TContext>>>();

    public Pipeline<TContext> Use(Func<TContext, Func<Task>, Task> middleware)
        => Use(next => context =>
        {
            Func<Task> simpleNext = () => next(context);
            return middleware(context, simpleNext);
        });

    public Pipeline<TContext> Use(Func<PipelineMiddleware<TContext>, PipelineMiddleware<TContext>> middleware)
    {
        middlewares.Add(middleware);
        return this;
    }

    public abstract TContext CreateContext();

    protected virtual void ConfigureMiddlewares(PipelineMiddleware<TContext> next)
    {
    }

    internal PipelineMiddleware<TContext> BuildMiddleware()
    {
        PipelineMiddleware<TContext> app = context => Task.CompletedTask;

        foreach (var middleware in middlewares.Reverse())
        {
            app = middleware(app);
        }

        return app;
    }

    public async Task Execute(TContext? context = null)
    {
        context ??= CreateContext();

        await BuildMiddleware()(context);
    }
}
