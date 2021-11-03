
using System;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xenial.Framework.Generators
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //            var source = @"using System;
            //public static class HelloWorld
            //{
            //    public static void SayHello()
            //    {
            //        Console.WriteLine(""Hello from generated code!"");
            //    }
            //}";
            //            context.AddSource("helloWorldGenerator", SourceText.From(source, Encoding.UTF8));

            //            var descriptor = new DiagnosticDescriptor(
            //                id: "theId",
            //                title: "the title",
            //                messageFormat: "the message from {0}",
            //                category: "the category",
            //                DiagnosticSeverity.Info,
            //                isEnabledByDefault: true);

            //            var location = Location.Create(
            //                "theFile",
            //                new TextSpan(1, 2),
            //                new LinePositionSpan(
            //                    new LinePosition(1, 2),
            //                    new LinePosition(3, 4)));
            //            var diagnostic = Diagnostic.Create(descriptor, location, "hello world generator");
            //            context.ReportDiagnostic(diagnostic);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }


    public class Class1
    {
        public abstract class Class1Builder<TClass, TBuilder>
            where TClass : Class1
            where TBuilder : Class1Builder<TClass, TBuilder>
        {

        }
    }

    public class Class2 : Class1
    {
        public class Class2Builder : Class2Builder<Class2, Class2Builder> { }
        public abstract partial class Class2Builder<TClass, TBuilder> : Class1Builder<TClass, TBuilder>
            where TClass : Class2
            where TBuilder : Class2Builder<TClass, TBuilder>
        {

        }

        public partial class Class2Builder<TClass, TBuilder>
        {

        }
    }

}
