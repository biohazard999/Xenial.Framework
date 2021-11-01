
using System;

namespace Xenial.Framework.Generators;

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
