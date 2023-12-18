using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Cli.Engine;

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
    Start:
        try
        {
            context ??= CreateContext();

            await BuildMiddleware()(context);
        }
        catch (RestartPipelineException)
        {
            goto Start;
        }
    }
}
