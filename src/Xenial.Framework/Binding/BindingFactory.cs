using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.Framework.Binding;

/// <summary>
/// 
/// </summary>
public interface IBindableFunctorProvider<T>
    where T : Delegate
{
    /// <summary>
    /// 
    /// </summary>
    string ConventionMethodName { get; }

    /// <summary>
    /// 
    /// </summary>
    string? MethodName { get; }

    /// <summary>
    /// 
    /// </summary>
    Type? DelegatedType { get; }

    /// <summary>
    /// 
    /// </summary>
    T? Delegate { get; }
}

/// <summary>
/// 
/// </summary>
public interface IBindingFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <param name="provider"></param>
    /// <param name="targetInstance"></param>
    /// <returns></returns>
    TDelegate ResovleDelegate<TDelegate>(
        IBindableFunctorProvider<TDelegate> provider,
        object targetInstance
    ) where TDelegate : Delegate;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <param name="provider"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public TDelegate ResovleDelegate<TDelegate>(
        IBindableFunctorProvider<TDelegate> provider,
        Type targetType
    ) where TDelegate : Delegate;
}
/// <summary>
/// 
/// </summary>
public sealed class BindingFactory : IBindingFactory
{
    internal sealed class CachedBindingFactory : IBindingFactory
    {
        private readonly IBindingFactory bindingFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingFactory"></param>
        public CachedBindingFactory(IBindingFactory bindingFactory!!)
            => this.bindingFactory = bindingFactory;

        private record struct InstanceDelegate(object Provider, object TargetInstance);
        private record struct TypedDelegate(object Provider, Type TargetType);

        private Dictionary<InstanceDelegate, Delegate> InstanceDelegateCache { get; } = new();
        private Dictionary<TypedDelegate, Delegate> TypedDelegateCache { get; } = new();

        TDelegate IBindingFactory.ResovleDelegate<TDelegate>(
            IBindableFunctorProvider<TDelegate> provider,
            object targetInstance
        )
        {
            var instanceDelegateKey = new InstanceDelegate(provider, targetInstance);
            if (InstanceDelegateCache.TryGetValue(instanceDelegateKey, out var cachedDelegate))
            {
                return (TDelegate)cachedDelegate;
            }

            var newDelegate = bindingFactory.ResovleDelegate(provider, targetInstance);
            InstanceDelegateCache[instanceDelegateKey] = newDelegate;
            return newDelegate;
        }

        TDelegate IBindingFactory.ResovleDelegate<TDelegate>(
            IBindableFunctorProvider<TDelegate> provider,
            Type targetType)
        {
            var typedDelegateKey = new TypedDelegate(provider, targetType);
            if (TypedDelegateCache.TryGetValue(typedDelegateKey, out var cachedDelegate))
            {
                return (TDelegate)cachedDelegate;
            }

            var newDelegate = bindingFactory.ResovleDelegate(provider, targetType);
            TypedDelegateCache[typedDelegateKey] = newDelegate;
            return newDelegate;
        }

        public void Clear()
        {
            InstanceDelegateCache.Clear();
            TypedDelegateCache.Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static IBindingFactory Cached { get; } = new CachedBindingFactory(new BindingFactory());

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <param name="provider"></param>
    /// <param name="targetInstance"></param>
    /// <returns></returns>
    public TDelegate ResovleDelegate<TDelegate>(
        IBindableFunctorProvider<TDelegate> provider!!,
        object targetInstance!!
    ) where TDelegate : Delegate
        => CreateDelegate(provider, targetInstance.GetType(), targetInstance);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <param name="provider"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public TDelegate ResovleDelegate<TDelegate>(
        IBindableFunctorProvider<TDelegate> provider!!,
        Type targetType!!
    ) where TDelegate : Delegate
        => CreateDelegate(provider, targetType);

    private static TDelegate CreateDelegate<TDelegate>(
        IBindableFunctorProvider<TDelegate> provider,
        Type targetType,
        object? targetInstance = null
    )
        where TDelegate : Delegate
    {

        if (provider.Delegate is null)
        {
            var delegatedType = provider.DelegatedType is null
                ? targetType // modelDetailView.ModelClass.TypeInfo.Type;
                : provider.DelegatedType;

            var methodName = string.IsNullOrEmpty(provider.MethodName)
                ? provider.ConventionMethodName
                : provider.MethodName;

            if (delegatedType is not null)
            {
                var method = delegatedType.GetMethod(methodName);
                if (method is not null)
                {
                    if (method.IsStatic)
                    {
                        var @delegate = Delegate.CreateDelegate(typeof(TDelegate), method);
                        return (TDelegate)@delegate;
                    }
                    else
                    {
                        //TODO: Cleanup instance and factory
                        var generatorInstance = targetInstance is null
                            ? Activator.CreateInstance(delegatedType)
                            : targetInstance;

                        var @delegate = Delegate.CreateDelegate(typeof(TDelegate), generatorInstance, method);
                        return (TDelegate)@delegate;
                    }
                }
            }
        }

        if (provider.Delegate is not null)
        {
            return provider.Delegate;
        }

        throw new InvalidOperationException($"Can not find any bindable delegate for '{provider.GetType()}' and target type '{targetType}'");
    }
}
