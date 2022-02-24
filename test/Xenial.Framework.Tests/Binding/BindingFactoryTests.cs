using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shouldly;

using Xenial.Framework.Binding;

using Xunit;

namespace Xenial.Framework.Tests.Binding;

public abstract class BindingFactoryAttribute<TDelegate> : Attribute, IBindableFunctorProvider<TDelegate>
    where TDelegate : Delegate
{
    public string? TheMethod { get; set; }
    public TDelegate? TheDelegate { get; set; }
    public Type? TheType { get; set; }

    string IBindableFunctorProvider<TDelegate>.ConventionMethodName => "Build";
    string? IBindableFunctorProvider<TDelegate>.MethodName => TheMethod;
    Type? IBindableFunctorProvider<TDelegate>.DelegatedType => TheType;
    TDelegate? IBindableFunctorProvider<TDelegate>.Delegate => TheDelegate;
}

public delegate void VoidBindable();

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class VoidBindingFactoryAttribute : BindingFactoryAttribute<VoidBindable>
{

}

public delegate object InstanceBindable(object @object);

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class InstanceBindingFactoryAttribute : BindingFactoryAttribute<InstanceBindable>
{

}

public delegate int IntBindable();

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class IntBindingFactoryAttribute : BindingFactoryAttribute<IntBindable>
{

}

public class BindingFactoryTests
{
    private BindingFactory Factory { get; } = new();

    [Fact]
    public void ShouldResovleDirectDelegate()
    {
        var attr = new VoidBindingFactoryAttribute
        {
            TheDelegate = PublicStaticVoid
        };

        var @delegate = Factory.ResovleDelegate(attr, GetType());
        @delegate.ShouldNotBeNull();
        Should.NotThrow(() => @delegate());
    }

    [Fact]
    public void ShouldResolvePublicStaticVoidDelegate()
    {
        var attr = new VoidBindingFactoryAttribute
        {
            TheMethod = nameof(PublicStaticVoid)
        };

        var @delegate = Factory.ResovleDelegate(attr, GetType());
        @delegate.ShouldNotBeNull();
        Should.NotThrow(() => @delegate());
    }

    [Fact]
    public void ShouldResolveStaticByConvention()
    {
        var attr = new VoidBindingFactoryAttribute
        {
        };

        var @delegate = Factory.ResovleDelegate(attr, GetType());
        @delegate.ShouldNotBeNull();
        Should.NotThrow(() => @delegate());
    }

    [Fact]
    public void ShouldResolvePublicInstanceVoidDelegate()
    {
        var attr = new VoidBindingFactoryAttribute
        {
            TheMethod = nameof(PublicInstanceVoid)
        };

        var @delegate = Factory.ResovleDelegate(attr, GetType());
        @delegate.ShouldNotBeNull();
        Should.NotThrow(() => @delegate());
    }


    public static void Build() { }
    public static void PublicStaticVoid() { }
    public void PublicInstanceVoid() { }

    [Fact]
    public void ShouldResolveInstanceBindable()
    {
        var attr = new InstanceBindingFactoryAttribute
        {
            TheMethod = nameof(InstanceBindable)
        };

        var @delegate = Factory.ResovleDelegate(attr, this);
        @delegate.ShouldNotBeNull();
        @delegate(this).ShouldBe(this);
    }

    public object InstanceBindable(object @object) => @object;


    [Fact]
    public void ShouldBeIntBindable()
    {
        var attr = new IntBindingFactoryAttribute
        {
            TheMethod = nameof(IntBindable)
        };

        var @delegate = Factory.ResovleDelegate(attr, this);
        @delegate.ShouldNotBeNull();

        @delegate();
        @delegate().ShouldBe(2);
    }

    public int Counter { get; set; }

    public int IntBindable()
    {
        Counter++;
        return Counter;
    }

    [Fact]
    public void ShouldBindToExternalStaticTypeByConvention()
    {
        var attr = new VoidBindingFactoryAttribute
        {
            TheType = typeof(ExternalType),
        };

        var @delegate = Factory.ResovleDelegate(attr, GetType());
        @delegate.ShouldNotBeNull();
        Should.NotThrow(() => @delegate());
    }

    public class ExternalType
    {
        public static void Build()
        {
        }
    }


    [Fact]
    public void CachedDelegates()
    {
        var attr = new VoidBindingFactoryAttribute
        {
            TheType = typeof(ExternalType),
        };

        var @delegate1 = BindingFactory.Cached.ResovleDelegate(attr, GetType());
        @delegate1.ShouldNotBeNull();

        var @delegate2 = BindingFactory.Cached.ResovleDelegate(attr, GetType());
        @delegate2.ShouldNotBeNull();

        @delegate1.ShouldBe(@delegate2);

        var @delegate3 = Factory.ResovleDelegate(attr, GetType());
        @delegate3.ShouldNotBeNull();

        var @delegate4 = Factory.ResovleDelegate(attr, GetType());
        @delegate4.ShouldNotBeNull();

        @delegate3.GetHashCode().ShouldBe(@delegate4.GetHashCode());
    }
}
