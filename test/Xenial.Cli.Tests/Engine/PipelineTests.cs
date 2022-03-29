using FakeItEasy;

using Shouldly;

using System;
using System.Threading.Tasks;

using Xenial.Cli.Engine;

using Xunit;
using Xunit.Abstractions;

namespace Xenial.Cli.Tests.Engine;

public record TestPipeline : Pipeline<PipelineContext>
{
    public override PipelineContext CreateContext() => new();
}

public class PipelineTests
{
    private readonly ITestOutputHelper output;

    public PipelineTests(ITestOutputHelper output)
        => this.output = output;

    [Fact]
    public void Empty()
    {
        var pipe = new TestPipeline();
        Should.NotThrow(async () => await pipe.Execute());
    }

    [Fact]
    public async Task One()
    {
        var pipe = new TestPipeline();

        var call = A.Fake<Action>();

        pipe.Use((ctx, next) =>
        {
            call();
            return next();
        });

        await pipe.Execute();

        A.CallTo(call).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Middleware()
    {
        var pipe = new TestPipeline();

        var callA = A.Fake<Action>();
        var callB = A.Fake<Action>();

        pipe.Use((ctx, next) =>
        {
            callA();
            return next();
        });

        pipe.Use(next => ctx =>
        {
            callB();
            return Task.CompletedTask;
        });

        await pipe.Execute();

        A.CallTo(callA).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(callB).MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task Order()
    {
        var pipe = new TestPipeline();

        var callA = A.Fake<Action>();
        var callB = A.Fake<Action>();
        var callC = A.Fake<Action>();

        pipe.Use((ctx, next) =>
        {
            output.WriteLine("A");
            callA();
            return next();
        });

        pipe.Use(next => ctx =>
        {
            output.WriteLine("B");
            callB();
            return next(ctx);
        });

        pipe.Use(next => ctx =>
        {
            output.WriteLine("C");
            callC();
            return next(ctx);
        });

        await pipe.Execute();

        A.CallTo(callA).MustHaveHappenedOnceExactly()
           .Then(A.CallTo(callB).MustHaveHappenedOnceExactly())
           .Then(A.CallTo(callC).MustHaveHappenedOnceExactly())
        ;
    }

}
