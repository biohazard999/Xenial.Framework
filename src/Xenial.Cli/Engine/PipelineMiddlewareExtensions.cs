
using Spectre.Console;

using System;

using System.Linq;

using static Xenial.Cli.Utils.ConsoleHelper;

namespace Xenial.Cli.Engine;

public static class PipelineMiddlewareExtensions
{
    public static Pipeline<TContext> UseStatus<TContext>(this Pipeline<TContext> pipeline, string caption, Func<TContext, Func<Task>, Task> middleware)
        where TContext : PipelineContext
        => pipeline.Use(async (ctx, next) =>
            {
                var tsc = new TaskCompletionSource();

                Func<Task> newnext = () =>
                {
                    tsc.SetResult();
                    return Task.CompletedTask;
                };

                await AnsiConsole.Status().StartAsync(caption, async statusContext =>
                {
                    statusContext.Spinner(Spinner.Known.Ascii);

                    await middleware(ctx, newnext);
                });

                await tsc.Task;
                await next();
            });
    public static Pipeline<TContext> UseProgress<TContext>(this Pipeline<TContext> pipeline, string caption, Func<TContext, ProgressContext, Func<Task>, Task> middleware)
        where TContext : PipelineContext
        => pipeline.Use(async (ctx, next) =>
        {
            var tsc = new TaskCompletionSource();

            Func<Task> newnext = () =>
            {
                tsc.SetResult();
                return Task.CompletedTask;
            };

            await AnsiConsole.Progress()
                .AutoClear(true)
                .AutoRefresh(true)
                .HideCompleted(true)
                .Columns(new ProgressColumn[]
                {
                    new SpinnerColumn(Spinner.Known.Ascii),
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn()
                }
                )
                .StartAsync(async statusContext =>
            {
                await middleware(ctx, statusContext, newnext);
            });

            await tsc.Task;
            await next();
        });

    public static Pipeline<TContext> UseStatusWithTimer<TContext>(
        this Pipeline<TContext> pipeline,
        string caption,
        string id,
        Func<TContext, bool> success,
        Func<TContext, bool> warnOnFail,
        Func<TContext, Func<Task>, Task> middleware,
        Func<TContext, bool>? skippedOnFail = null,
        Func<TContext, bool>? forceRunNext = null
    ) where TContext : PipelineContext
        =>
        pipeline.Use(async (ctx, next) =>
        {
            ctx.Stopwatch.Restart();
            await next();

        }).UseStatus(caption, middleware)
        .Use(async (ctx, next) =>
        {
            var isSkipped = skippedOnFail?.Invoke(ctx) ?? false;

            if (isSkipped)
            {
                await next();
                return;
            }

            if (success(ctx))
            {
                ctx.Stopwatch.Success(id);
                await next();
                return;
            }
            else
            {
                if (warnOnFail(ctx))
                {
                    ctx.Stopwatch.Warn(id);
                    await next();
                    return;
                }
                else
                {
                    ctx.Stopwatch.Fail(id);
                    ctx.ExitCode = 1;
                }
            }

            if (forceRunNext?.Invoke(ctx) ?? false)
            {
                await next();
            }
        });

    public static Pipeline<TContext> UseStatusWithProgress<TContext>(
       this Pipeline<TContext> pipeline,
       string caption,
       string id,
       Func<TContext, bool> success,
       Func<TContext, bool> warnOnFail,
       Func<TContext, ProgressContext, Func<Task>, Task> middleware,
       Func<TContext, bool>? skippedOnFail = null,
       Func<TContext, bool>? forceRunNext = null,
       Func<TContext, bool>? skipWhen = null
   ) where TContext : PipelineContext
    {
        var wasSkipped = false;

        return pipeline.Use(async (ctx, next) =>
        {
            ctx.Stopwatch.Restart();
            await next();
        })
        .UseProgress(caption, async (ctx, progress, next) =>
        {
            if (skipWhen?.Invoke(ctx) ?? false)
            {
                wasSkipped = true;
                await next();
            }
            else
            {
                await middleware(ctx, progress, next);
            }
        })
        .Use(async (ctx, next) =>
        {
            var isSkipped = skippedOnFail?.Invoke(ctx) ?? false;

            if (isSkipped || wasSkipped)
            {
                await next();
                return;
            }

            if (success(ctx))
            {
                ctx.Stopwatch.Success(id);
                await next();
                return;
            }
            else
            {
                if (warnOnFail(ctx))
                {
                    ctx.Stopwatch.Warn(id);
                    await next();
                    return;
                }
                else
                {
                    ctx.Stopwatch.Fail(id);
                    ctx.ExitCode = 1;
                }
            }

            if (forceRunNext?.Invoke(ctx) ?? false)
            {
                await next();
            }
        });
    }
}
